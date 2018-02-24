// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.ImplementationBuilders;
    using Serpent.InterfaceProxy.Validation;

    public class ProxyTypeBuilder : TypeCloneBuilder<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>
    {
        private readonly Type parentType;

        private readonly Func<Type, Type> proxyBaseTypeFactoryFunc;

        public ProxyTypeBuilder(Type parentType, Func<Type, Type> parentTypeFactoryFunc)
            : this()
        {
            this.parentType = parentType;
            this.proxyBaseTypeFactoryFunc = parentTypeFactoryFunc ?? throw new ArgumentNullException(nameof(parentTypeFactoryFunc));
        }

        public override GenerateTypeResult GenerateTypeClone(GenerateTypeParameters parameters)
        {
            parameters.CreatedTypeName = parameters.CreatedTypeName ?? this.ProxyTypeNameSelectorFunc(parameters.SourceType, this.Namespace);
            parameters.CreateConstructors = true;
            parameters.ParentType = this.parentType;
            return base.GenerateTypeClone(parameters);
        }

        private ProxyTypeBuilder()
        {
            // Default delegates for names
            this.ClosureTypeNameSelector = (@interface, methodInfo, @namespace) =>
                @namespace + "." + @interface.Name + "." + methodInfo.Name + "_Closure_" + Guid.NewGuid().ToString("N");

            this.ProxyTypeNameSelectorFunc = (@interface, @namespace) => @namespace + "." + @interface.Name + "+proxy";
        }

        public ProxyTypeBuilder(Type parentType)
            : this()
        {
            Validator.Default.IsNotNull(parentType, nameof(parentType));

            this.parentType = parentType;

            if (parentType.ContainsGenericParameters)
            {
                var parentGenericArguments = parentType.GetGenericArguments();

                if (parentGenericArguments.Length != 1)
                {
                    throw new Exception("Parent type has more than one generic argument. Use the constructor with a factory function instead.");
                }

                this.proxyBaseTypeFactoryFunc = t => parentType.MakeGenericType(t);
            }
            else
            {
                this.proxyBaseTypeFactoryFunc = t => parentType;
            }
        }

        public Func<Type, MethodInfo, string, string> ClosureTypeNameSelector { get; set; }

        public Func<Type, string, string> ProxyTypeNameSelectorFunc { get; set; }


        public static IEnumerable<MethodInfo> GetExecuteAsyncMethods(Type parentType)
        {
            return parentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.Name == "ExecuteAsync" || method.Name == "Execute");
        }

        protected override MethodContext CreateMethodContext(
            TypeContext typeContext,
            Type @interface,
            TypeBuilder typeBuilder,
            MethodInfo sourceMethodInfo,
            ParameterInfo[] parameters,
            Type[] genericArguments)
        {
            TypeBuilder createClosureTypeFunc(Type interfaceType, MethodInfo method)
            {
                var closureTypeName = this.ClosureTypeNameSelector(interfaceType, method, this.Namespace);
                return ClosureBuilder.CreateClosureTypeBuilder(this.ModuleBuilder, closureTypeName);
            }

            var finalClosureType = GetFinalClosureType(@interface, createClosureTypeFunc, parameters, sourceMethodInfo, genericArguments);
            var finalDelegateMethod = GetFinalDelegateMethod(typeBuilder, @interface, sourceMethodInfo, finalClosureType, genericArguments);

            return new MethodContext(typeContext)
                       {
                           ClosureFinalType = finalClosureType,
                           FinalDelegateMethodInfo = finalDelegateMethod
                       };
        }

        protected override TypeContext CreateTypeContext(GenerateTypeParameters parameters, TypeBuilder typeBuilder)
        {
            return new TypeContext(parameters, typeBuilder);
        }

        protected override void EmitMethodImplementation(
            Type interfaceType,
            MethodInfo sourceMethodInfo,
            MethodBuilder methodBuilder,
            MethodContext context,
            Type parentType)
        {
            var closureFinalType = context.ClosureFinalType;

            var generator = methodBuilder.GetILGenerator();
            var parameters = sourceMethodInfo.GetParameters();

            var hasParameters = parameters.Length != 0;

            var isAsyncMethod = typeof(Task).IsAssignableFrom(sourceMethodInfo.ReturnType);

            var returnValueGenericArguments = sourceMethodInfo.ReturnType.GenericTypeArguments;
            var hasGenericReturnValueArguments = returnValueGenericArguments.Length != 0;

            var hasAsyncReturnValue = isAsyncMethod && returnValueGenericArguments.Length == 1;
            var hasNonAsyncReturnValue = !isAsyncMethod && typeof(void) != sourceMethodInfo.ReturnType;
            var hasReturnValue = hasAsyncReturnValue || hasNonAsyncReturnValue;
            var returnsVoid = sourceMethodInfo.ReturnType == typeof(void);

            var returnType = sourceMethodInfo.ReturnType;

            if (hasReturnValue && isAsyncMethod)
            {
                returnType = sourceMethodInfo.ReturnType.GenericTypeArguments[0];
            }

            // Start of call to the proxy method (being called in the end of this method). Moved ldarg_0 here to optimize
            generator.Emit(OpCodes.Ldarg_0); // this (from the current method)

            // only emit instantiating the closure if it's needed
            if (hasParameters)
            {
                var closureConstructor = closureFinalType.GetConstructors().Single();

                generator.Emit(OpCodes.Newobj, closureConstructor);

                // Populate the closure
                var closureFields = closureFinalType.GetFields();

                var parameterCount = parameters.Length;
                for (var i = 0; i < parameterCount; i++)
                {
                    generator.Emit(OpCodes.Dup);

                    generator.LdArg(i + 1);

                    generator.Emit(OpCodes.Stfld, closureFields[i]);
                }
            }

            // Second parameter to the Func<*> constructor
            // get the address of this.DelegateMethodAsync
            generator.Emit(OpCodes.Ldarg_0); // this.
            generator.Emit(OpCodes.Ldftn, context.FinalDelegateMethodInfo); // the delegate method

            // Instantiate the Func<*>
            var delegateType = GetDelegateType(interfaceType, sourceMethodInfo, closureFinalType, hasParameters, returnsVoid);

            var delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            generator.Emit(OpCodes.Newobj, delegateConstructor);

            var proxyMethod = GetProxyMethod(closureFinalType, parentType, hasParameters, hasReturnValue, returnType, hasGenericReturnValueArguments, isAsyncMethod);

            generator.EmitCall(OpCodes.Call, proxyMethod, null); // call the proxy method
            generator.Emit(OpCodes.Ret);
        }

        private static Type GetDelegateType(Type interfaceType, MethodInfo sourceMethodInfo, Type closureFinalType, bool hasParameters, bool returnsVoid)
        {
            Type delegateType;
            if (hasParameters)
            {
                if (!returnsVoid)
                {
                    delegateType = typeof(Func<,,>).MakeGenericType(closureFinalType, interfaceType, sourceMethodInfo.ReturnType);
                }
                else
                {
                    delegateType = typeof(Action<,>).MakeGenericType(closureFinalType, interfaceType);
                }
            }
            else
            {
                if (!returnsVoid)
                {
                    delegateType = typeof(Func<,>).MakeGenericType(interfaceType, sourceMethodInfo.ReturnType);
                }
                else
                {
                    delegateType = typeof(Action<>).MakeGenericType(interfaceType);
                }
            }

            return delegateType;
        }

        private static Type GetFinalClosureType(
            Type @interface,
            Func<Type, MethodInfo, TypeBuilder> createClosureTypeFunc,
            ParameterInfo[] parameters,
            MethodInfo method,
            Type[] genericArguments)
        {
            Type closureFinalType = null;

            if (parameters.Length > 0)
            {
                var closureTypeBuilder = createClosureTypeFunc(@interface, method);
                closureFinalType = ClosureBuilder.CreateClosureType(closureTypeBuilder, method).MakeGenericTypeIfNecessary(genericArguments);
            }

            return closureFinalType;
        }

        private static MethodInfo GetFinalDelegateMethod(TypeBuilder typeBuilder, Type @interface, MethodInfo method, Type closureFinalType, Type[] genericArguments)
        {
            var delegateMethodName = method.Name + "_delegate_" + Guid.NewGuid().ToString("N");
            var delegateMethodBuilder = DelegateBuilder.CreateDelegateMethod(delegateMethodName, typeBuilder, method, closureFinalType, @interface);
            return delegateMethodBuilder.MakeGenericMethodIfNecessary(genericArguments);
        }

        private static MethodInfo GetProxyMethod(
            Type closureFinalType,
            Type parentType,
            bool hasParameters,
            bool hasReturnValue,
            Type returnType,
            bool hasGenericReturnValueArguments,
            bool isAsyncMethod)
        {
            var proxyMethods = GetExecuteAsyncMethods(parentType);

            var proxyMethodGenericParameters = ImmutableArray<Type>.Empty;

            if (hasParameters)
            {
                proxyMethodGenericParameters = proxyMethodGenericParameters.Add(closureFinalType);
            }

            if (hasReturnValue)
            {
                proxyMethodGenericParameters = proxyMethodGenericParameters.Add(returnType);

                if (hasGenericReturnValueArguments)
                {
                    proxyMethods = proxyMethods.Where(method => method.ReturnType.ContainsGenericParameters);
                }

                proxyMethods = proxyMethods.Where(method => method.ReturnType != typeof(void) && method.ReturnType != typeof(Task));
            }
            else
            {
                proxyMethods = proxyMethods.Where(method => method.ReturnType == typeof(void) || method.ReturnType == typeof(Task));
            }

            // get Task<> return type if needed
            proxyMethods = proxyMethods.Where(method => typeof(Task).IsAssignableFrom(method.ReturnType) == isAsyncMethod);

            var executeAsyncParameterCount = 1 + (hasParameters ? 1 : 0);

            proxyMethods = proxyMethods.Where(method => method.GetParameters().Length == executeAsyncParameterCount);

            var executeAsync = proxyMethods.Single();

            if (proxyMethodGenericParameters.Length > 0)
            {
                executeAsync = executeAsync.MakeGenericMethod(proxyMethodGenericParameters.ToArray());
            }

            return executeAsync;
        }

        public class MethodContext
        {
            public MethodContext(TypeContext typeContext)
            {
                this.TypeContext = typeContext;
            }

            public Type ClosureFinalType { get; set; }

            public MethodInfo FinalDelegateMethodInfo { get; set; }

            public TypeContext TypeContext { get; }
        }

        public class TypeContext : BaseTypeCloneTypeContext
        {
            public TypeContext(GenerateTypeParameters parameters, TypeBuilder typeBuilder) : base(parameters, typeBuilder)
            {
            }
        }
    }
}
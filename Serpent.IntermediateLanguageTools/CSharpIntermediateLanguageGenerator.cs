namespace Serpent.IntermediateLanguageTools
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    using Mono.Reflection;

    using Serpent.IntermediateLanguageTools.Helpers;
    using Serpent.IntermediateLanguageTools.Models;

    public class CSharpIntermediateLanguageGenerator
    {
        public string CreateMethodILGenerator(CreateMethodILGeneratorParameters createMethodIlGeneratorParameters)
        {
            var instructions = createMethodIlGeneratorParameters.Method.GetInstructions();
            var builder = new StringBuilder();


            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "public void Create_" + createMethodIlGeneratorParameters.Method.Name + "_Code(System.Reflection.Emit.ILGenerator generator)");
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "{");

            // builder.AppendLine("// TODO: Implement the method declaration (in in relation to type generics as well)");
            // builder.AppendLine(GetMethodInformation(method));
            createMethodIlGeneratorParameters.TabCount++;

            // Local variables
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "// The local variables");
            var localVariables = GetLocalVariables(instructions).OrderBy(variable => variable.LocalIndex);
            builder.Append(GetDefineLocalsText(localVariables, "generator", createMethodIlGeneratorParameters.TabCount));

            // add labels
            var labels = instructions.Where(i => i.Operand is Instruction).Select(i => ((Instruction)i.Operand).Offset).Distinct().OrderBy(i => i);

            var labelSet = new HashSet<int>(labels);

            builder.AppendLine();
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "// Add labels");

            foreach (var label in labels)
            {
                builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, $"var label_{label:X2} = generator.DefineLabel();");
            }

            builder.AppendLine();
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "// The code");

            foreach (var instruction in instructions)
            {
                if (createMethodIlGeneratorParameters.Verbose)
                {
                    builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, $"// {instruction}");
                }

                if (labelSet.Contains(instruction.Offset))
                {
                    builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, $"generator.MarkLabel(label_{instruction.Offset:X2});");
                }

                builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, GetInstructionEmitText(instruction, createMethodIlGeneratorParameters.DotNetType) + ";");

                if (createMethodIlGeneratorParameters.Verbose)
                {
                    builder.AppendLine();
                }
            }

            createMethodIlGeneratorParameters.TabCount--;

            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "}");
            return builder.ToString();
        }

        private static string GetDefineLocalsText(IEnumerable<LocalVariableInfo> localVariables, string generatorName, int tabCount = 0)
        {
            var builder = new StringBuilder();

            foreach (var variable in localVariables)
            {
                builder.AppendLineTabbed(
                    tabCount,
                    "var local_"
                    + variable.LocalIndex
                    + " = "
                    + (string.IsNullOrWhiteSpace(generatorName) == false ? generatorName + "." : string.Empty)
                    + GetDefineLocalText(variable)
                    + ";");
            }

            return builder.ToString();
        }

        private static string GetDefineLocalText(LocalVariableInfo variable)
        {
            var declareLocalStart = "DeclareLocal(typeof(" + variable.LocalType.GetCSharpName(true, true) + ")";

            if (variable.IsPinned)
            {
                return declareLocalStart + ", true)";
            }

            return declareLocalStart + ")";
        }

        private static string GetInstructionEmitText(Instruction instruction, Type declaringType = null)
        {
            var result = "generator.Emit(System.Reflection.Emit.OpCodes." + GetOpCodeText(instruction.OpCode);

            if (instruction.Operand != null)
            {
                if (instruction.Operand is LocalVariableInfo localVariable)
                {
                    return $"{result}, local_{localVariable.LocalIndex})";
                }

                if (instruction.Operand is TypeInfo typeInfo)
                {
                    return $"{result}, typeof({typeInfo.GetCSharpName(true, true)}))";
                }

                if (instruction.Operand is MethodInfo methodInfo)
                {
                    ////var method = methodInfo.DeclaringType.GetMethod(
                    ////    methodInfo.Name,
                    ////    BindingFlags.Public | BindingFlags.NonPublic,
                    ////    null,
                    ////    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(),
                    ////    null);

                    // TODO: Make it possible to reference a method not created yet, in a type not created yet
                    var parameters = methodInfo.GetParameters();

                    string parameterTypes;

                    if (parameters.Length == 0)
                    {
                        parameterTypes = "System.Type.EmptyTypes";
                    }
                    else
                    {
                        parameterTypes = $"new[] {{ {string.Join(", ", parameters.Select(p => $"typeof({p.ParameterType.GetCSharpName(true, true)})"))} }}";
                    }

                    return
                        $"{result}, typeof({methodInfo.DeclaringType.GetCSharpName(true, true)}).GetMethod(\"{methodInfo.Name}\", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, {parameterTypes}, null))";
                }

                if (instruction.Operand is FieldInfo fieldInfo)
                {
                    // TODO: Make it possible to reference a field not created yet, in a type not created yet
                    var field = fieldInfo.DeclaringType.GetField(fieldInfo.Name, BindingFlags.Public | BindingFlags.NonPublic);

                    return
                        $"{result}, typeof({fieldInfo.DeclaringType.GetCSharpName(true, true)}).GetField(\"{fieldInfo.Name}\", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))";
                }

                if (instruction.Operand is Instruction labelInstruction)
                {
                    return result + $", label_{labelInstruction.Offset:X2})";
                }

                var operandType = instruction.Operand?.GetType();

                Debug.WriteLine("unknown operand type: " + operandType.FullName);
                throw new NotImplementedException("Can't generate parameter of: " + instruction.Operand);

                return result + ")";
            }

            return result + ")";
        }

        private static HashSet<LocalVariableInfo> GetLocalVariables(IEnumerable<Instruction> instructions)
        {
            var variables = new HashSet<LocalVariableInfo>();

            foreach (var instruction in instructions)
            {
                if (instruction.Operand is LocalVariableInfo localVariableInfo)
                {
                    variables.Add(localVariableInfo);
                }
            }

            return variables;
        }

        private static string GetOpCodeText(OpCode opCode)
        {
            return opCode.Name.ToUpperFirst().Replace(".", "_").ToUpperFirstAfterFirstInstance('_');

            // switch (opCode.Name)
            // {
            // case "nop":
            // return "Nop";
            // case "ldloca.s":
            // return "Ldloca_s";
            // case "initobj":
            // return "Initobj";
            // default:
            // throw new NotImplementedException(opCode.ToString());
            // }
        }

        private struct MethodContext
        {
            public HashSet<LocalVariableInfo> LocalVariables { get; set; }
        }
    }

    public static class StringExtensions
    {
        public static string ToUpperFirst(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper();
            }

            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public static string ToUpperFirst(this string text, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper(culture);
            }

            return text.Substring(0, 1).ToUpper(culture) + text.Substring(1);
        }

        public static string ToUpperFirstAfter(this string text, int startIndex)
        {
            if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
            {
                return text;
            }

            return text.Substring(0, startIndex) + text.Substring(startIndex, 1).ToUpper() + (text.Length <= startIndex + 1 ? string.Empty : text.Substring(startIndex + 1));
        }

        public static string ToUpperFirstAfter(this string text, int startIndex, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper(culture);
            }

            return text.Substring(0, startIndex) + text.Substring(startIndex, 1).ToUpper(culture) + (text.Length <= startIndex + 1 ? string.Empty : text.Substring(startIndex + 1));
        }

        public static string ToUpperFirstAfterFirstInstance(this string text, char firstInstance, CultureInfo culture)
        {
            var index = text.IndexOf(firstInstance);
            if (index == -1)
            {
                return text;
            }

            return text.ToUpperFirstAfter(index + 1, culture);
        }

        public static string ToUpperFirstAfterFirstInstance(this string text, char firstInstance)
        {
            var index = text.IndexOf(firstInstance);
            if (index == -1)
            {
                return text;
            }

            return text.ToUpperFirstAfter(index + 1);
        }
    }
}
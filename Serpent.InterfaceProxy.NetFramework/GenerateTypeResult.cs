namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.InterfaceProxy.Extensions;

    public struct GenerateTypeResult
    {
        public GenerateTypeResult(Type generatedType, IEnumerable<Type> interfacesImplemented, IEnumerable<Delegate> factories)
        {
            this.GeneratedType = generatedType;
            this.InterfacesImplemented = interfacesImplemented;
            this.Factories = factories;
        }

        public IEnumerable<Delegate> Factories { get; }

        public Type GeneratedType { get; }

        public IEnumerable<Type> InterfacesImplemented { get; }

        public Func<TResult> GetFactory<TResult>()
        {
            return (Func<TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<TResult>>());
        }

        public Func<T1, TResult> GetFactory<T1, TResult>()
        {
            return (Func<T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T1, TResult>>());
        }

        public Func<T2, T1, TResult> GetFactory<T2, T1, TResult>()
        {
            return (Func<T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T2, T1, TResult>>());
        }

        public Func<T3, T2, T1, TResult> GetFactory<T3, T2, T1, TResult>()
        {
            return (Func<T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T3, T2, T1, TResult>>());
        }

        public Func<T4, T3, T2, T1, TResult> GetFactory<T4, T3, T2, T1, TResult>()
        {
            return (Func<T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T4, T3, T2, T1, TResult>>());
        }

        public Func<T5, T4, T3, T2, T1, TResult> GetFactory<T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T6, T5, T4, T3, T2, T1, TResult> GetFactory<T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(f => f.GetType().Is<Func<T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }

        public Func<T16, T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult> GetFactory<T16, T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>()
        {
            return (Func<T16, T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>)this.Factories.FirstOrDefault(
                f => f.GetType().Is<Func<T16, T15, T14, T13, T12, T11, T10, T9, T8, T7, T6, T5, T4, T3, T2, T1, TResult>>());
        }
    }

    public struct GenerateProxyResult<TInterface>
    {
        public GenerateProxyResult(Type generatedType, IEnumerable<Type> interfacesImplemented, Func<TInterface, TInterface> factory)
        {
            this.GeneratedType = generatedType;
            this.InterfacesImplemented = interfacesImplemented;
            this.Factory = factory;
        }

        public Func<TInterface, TInterface> Factory { get; }

        public Type GeneratedType { get; }

        public IEnumerable<Type> InterfacesImplemented { get; }
    }
}
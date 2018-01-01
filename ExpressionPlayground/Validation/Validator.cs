namespace ExpressionPlayground.Validation
{
    using System;

    public class Validator
    {
        public static Validator Default { get; } = new Validator();

        public Validator IsNotNull<T>(T value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return this;
        }

        public Validator IsInterface(Type type, string argumentName, string errorMessage = null)
        {
            if (type.IsInterface == false)
            {
                throw new ArgumentException(argumentName + (errorMessage ?? " must be of interface type"));
            }

            return this;
        }

    }
}
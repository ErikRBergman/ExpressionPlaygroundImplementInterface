namespace ExpressionPlayground.Validation
{
    using System;

    public class Validator
    {
        public static Validator Default { get; } = new Validator();

        public Validator AreEqual<T>(T value, T value2)
        {
            if (value == null && value2 == null)
            {
                return this;
            }

            if (value == null)
            {
                throw new Exception("Values are not equal");
            }

            if (value.Equals(value2))
            {
                return this;
            }

            return this;
        }

        public Validator IsNotNull<T>(T value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return this;
        }

        public Validator IsTrue(bool? value, string argumentName)
        {
            if (value != true)
            {
                throw new ArgumentException("Value must be true", argumentName);
            }

            return this;
        }

        public Validator IsFalse(bool? value, string argumentName)
        {
            if (value != false)
            {
                throw new ArgumentException("Value must be false", argumentName);
            }

            return this;
        }

        public Validator IsInterface(Type type, string argumentName, string errorMessage = null)
        {
            if (type.IsInterface == false)
            {
                throw new ArgumentException(argumentName + (errorMessage ?? " must be of interface type"), argumentName);
            }

            return this;
        }

    }
}
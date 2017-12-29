namespace ExpressionPlayground
{
    using System;
    using System.Threading.Tasks;

    public class Program
    {
        private static void Main(string[] args)
        {
            var g = new ImplementInterface();
            g.GenerateProxy();
        }

    }
}
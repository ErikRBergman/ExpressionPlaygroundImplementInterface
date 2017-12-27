namespace ExpressionPlayground
{
    using System;
    using System.Threading.Tasks;

    public class Program
    {
        private static void Main(string[] args)
        {
            var t = Task.Run(() => throw new Exception("YOYOYO"));
            t.Wait();
        }
    }
}
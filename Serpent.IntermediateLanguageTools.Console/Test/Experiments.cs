namespace API.Controllers
{
    using System;
    using System.Threading.Tasks;

    using FG.CallContext;

    using StatelessService1.Interfaces;

    public class Experiments : IStatelessService1
    {
        private readonly IStatelessService1Clone inner;

        private readonly Func<InsertedCorrelationData> generateCorrelationDataFunc;

        public Experiments(IStatelessService1Clone inner, Func<InsertedCorrelationData> generateCorrelationDataFunc)
        {
            this.inner = inner;
            this.generateCorrelationDataFunc = generateCorrelationDataFunc;
        }

        public void LotsAndLots()
        {
            string nullString = null;

            byte a = 1;
            sbyte b = 2;
            short c = 1;
            ushort d = 1;
            int e = 1;
            uint f = 1;
            long g = 1;
            ulong h = 1;
            float i = 1;
            double j = 1;
            decimal k = 1;
            string l = "1 string";


            //var zzz = new object[] { a, b, c, d, e, f, g, h, i, j, k, l };
        }

        public Task<ReturnDataModel> DoItStatic(RequestDataModel request, string text)
        {
            var xx = request;
            return null;
        }

        public Task<ReturnDataModel> DoItAsync(RequestDataModel request, string text)
        {
            return this.inner.DoItAsync(this.generateCorrelationDataFunc(), request, text);
        }

        public async Task DoNothingAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnDataModel> NoParametersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
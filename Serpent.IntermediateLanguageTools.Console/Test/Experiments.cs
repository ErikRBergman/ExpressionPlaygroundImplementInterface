namespace Serpent.IntermediateLanguageTools.Console.Test
{
    using System;
    using System.Threading.Tasks;

    using FG.CallContext;

    using StatelessService1.Interfaces;

    public class Experiments : IStatelessService1
    {
        private readonly IStatelessService1Clone inner;

        public Experiments(IStatelessService1Clone inner)
        {
            this.inner = inner;
        }

        public Task<ReturnDataModel> DoItAsync(RequestDataModel request, string text)
        {
            var correlationData = new InsertedCorrelationData
                                      {
                                          CorrelationId = FG.CallContext.CallContext.Current.CorrelationId()
                                      };

            return this.inner.DoItAsync(correlationData, request, text);
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
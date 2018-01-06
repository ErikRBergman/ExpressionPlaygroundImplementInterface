////namespace ExpressionPlayground
////{
////    using System;
////    using System.Threading;
////    using System.Threading.Tasks;

////    using Microsoft.ServiceFabric.Actors.Remoting.V1.Builder;

////    internal class MockMethodDispatcher : ActorMethodDispatcherBase
////    {
////        public override Task<object> DispatchAsync(object objectImplementation, int methodId, object requestBody, CancellationToken cancellationToken)
////        {
////            return Task.FromResult<object>(null);
////        }

////        public override void Dispatch(object objectImplementation, int methodId, object messageBody)
////        {
////        }

////        protected override object CreateResponseBody(int methodId, object retval)
////        {
////            throw new NotImplementedException();
////        }

////        protected override Task<object> OnDispatchAsync(int methodId, object remotedObject, object requestBody, CancellationToken cancellationToken)
////        {
////            throw new NotImplementedException();
////        }

////        protected override void OnDispatch(int methodId, object remotedObject, object messageBody)
////        {
////            throw new NotImplementedException();
////        }
////    }
////}
namespace WCFServices.OrdersService
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;
    using WCFServices.DataContracts;
    using WCFServices.Infrastructure;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class OrdersService : BaseOrdersService, IOrdersService, IOrdersSubscriptionService
    {
        #region IOrdersService members

        public IEnumerable<OrderDTO> GetAll()
        {
            return this.GetAllOrders();
        }

        public OrderDTO GetById(int orderId)
        {
            try
            {
                return this.GetOrderById(orderId);
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public int CreateNewOrder(OrderDTO order)
        {
            try
            {
                return this.Create(order);
            }
            catch (BusinessException exception)
            {
                throw new FaultException(exception.Message, new FaultCode("Error"));
            }
        }

        public void UpdateOrder(OrderDTO order)
        {
            try
            {
                this.Update(order);
            }
            catch (BusinessException exception)
            {
                throw new FaultException(exception.Message, new FaultCode("Error"));
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public void ProcessOrder(int orderId)
        {
            try
            {
                this.Process(orderId);
            }
            catch (BusinessException exception)
            {
                throw new FaultException(exception.Message, new FaultCode("Error"));
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public void CloseOrder(int orderId)
        {
            try
            {
                this.Close(orderId);
            }
            catch (BusinessException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public int DeleteOrder(int orderId)
        {
            try
            {
                return this.Delete(orderId);
            }
            catch (BusinessException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public void SimulateLongRunningOperation(byte delayInSeconds)
        {
            Thread.Sleep(delayInSeconds * 1000);
        }

        #endregion

        #region IOrdersSubscriptionService members

        public bool Subscribe(string clientIdentifier)
        {
            if (string.IsNullOrWhiteSpace(clientIdentifier))
            {
                return false;
            }

            if (Callbacks.ContainsKey(clientIdentifier))
            {
                return false;
            }

            var callbackChannel = OperationContext.Current.GetCallbackChannel<IBroadcastCallback>();
            return Callbacks.TryAdd(clientIdentifier, callbackChannel);
        }

        public bool Unsubscribe(string clientIdentifier)
        {
            if (string.IsNullOrWhiteSpace(clientIdentifier))
            {
                return false;
            }

            if (!Callbacks.ContainsKey(clientIdentifier))
            {
                return false;
            }

            IBroadcastCallback callbackChannel;
            return Callbacks.TryRemove(clientIdentifier, out callbackChannel);
        }

        #endregion
    }
}
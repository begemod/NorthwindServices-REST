namespace WCFServices.OrdersService
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL.Entities;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;
    using WCFServices.DataContracts;
    using WCFServices.Infrastructure;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class OrdersService : BaseOrdersService, IOrdersService, IOrdersSubscriptionService
    {
        private static readonly ConcurrentDictionary<string, IBroadcastCallback> Callbacks = new ConcurrentDictionary<string, IBroadcastCallback>();

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
                var sourceOrder = DataService.GetById(orderId);

                if (!Mapper.Map<Order, OrderDTO>(sourceOrder).OrderState.Equals(OrderState.New))
                {
                    throw new FaultException(new FaultReason("Only Order in New status can be processed to InWork state."), new FaultCode("Error"));
                }

                var orderDate = DateTime.Now;

                if (sourceOrder.RequiredDate < orderDate)
                {
                    throw new FaultException(new FaultReason("Order's Required Date is expired."), new FaultCode("Error"));
                }

                sourceOrder.OrderDate = orderDate;

                DataService.UpdateOrder(sourceOrder);

                // raise on order status changed event
                this.OnOrderStatusChanged(orderId);
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
                var sourceOrder = DataService.GetById(orderId);

                if (!Mapper.Map<Order, OrderDTO>(sourceOrder).OrderState.Equals(OrderState.InWork))
                {
                    throw new FaultException(new FaultReason("Only Order in InWork status can be closed."), new FaultCode("Error"));
                }

                sourceOrder.ShippedDate = DateTime.Now;

                DataService.UpdateOrder(sourceOrder);

                // raise on order status changed event
                this.OnOrderStatusChanged(orderId);
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

        private void OnOrderStatusChanged(int orderId)
        {
            Task.Factory.StartNew(() =>
                {
                    var clientKeys = Callbacks.Keys.ToArray();
                    foreach (var clientKey in clientKeys)
                    {
                        IBroadcastCallback callback;
                        if (Callbacks.TryGetValue(clientKey, out callback))
                        {
                            if (callback != null)
                            {
                                try
                                {
                                    callback.OrderStatusIsChanged(orderId);
                                }
                                catch (TimeoutException)
                                {
                                    // suppose that connection to client has been lost
                                    // and callback should be removed from list
                                    IBroadcastCallback faultedCallback;
                                    var faultedClientId = clientKey;
                                    Callbacks.TryRemove(faultedClientId, out faultedCallback);
                                }
                            }
                        }
                    }
                });
        }
    }
}
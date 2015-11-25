namespace WCFServices.OrdersService
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL.DataServices;
    using DAL.Entities;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;
    using WCFServices.Cotracts.DataContracts;
    using WCFServices.DataContracts;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class OrdersService : IOrdersService, IOrdersSubscriptionService, IRestOrdersService
    {
        private static readonly ConcurrentDictionary<string, IBroadcastCallback> Callbacks = new ConcurrentDictionary<string, IBroadcastCallback>();
        private readonly OrdersDataService ordersDataService;

        public OrdersService()
        {
            var connectionFactory = new NortwindDbConnectionFactory();
            this.ordersDataService = new OrdersDataService(connectionFactory);

            this.ConfigureInMapping();
            this.ConfigureOutMapping();
        }

        #region IOrdersService members

        public IEnumerable<OrderDTO> GetAll()
        {
            return this.ordersDataService.GetAll().Select(Mapper.Map<Order, OrderDTO>);
        }

        public OrderDTO GetById(int orderId)
        {
            var orderById = this.ordersDataService.GetById(orderId);

            var result = Mapper.Map<Order, OrderDTO>(orderById);

            return result;
        }

        public OrderDTO GetById(string id)
        {
            int orderId = 0;

            int.TryParse(id, out orderId);

            try
            {
                return this.GetById(orderId);
            }
            catch (EntityNotFoundException exception)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public int CreateNewOrder(OrderDTO order)
        {
            if (order == null)
            {
                throw new FaultException(new FaultReason("Order should be defined."), new FaultCode("Error"));
            }

            order.OrderDate = null;
            order.ShippedDate = null;

            var orderId = this.ordersDataService.InsertOrder(Mapper.Map<OrderDTO, Order>(order));

            return orderId;
        }

        public void UpdateOrder(OrderDTO order)
        {
            if (order == null)
            {
                throw new FaultException(new FaultReason("Order should be defined."), new FaultCode("Error"));
            }

            try
            {
                var orderBO = Mapper.Map<OrderDTO, Order>(order);

                var sourceOrder = this.ordersDataService.GetById(order.OrderId);

                if (!Mapper.Map<Order, OrderDTO>(sourceOrder).OrderState.Equals(OrderState.New))
                {
                    throw new FaultException(new FaultReason("Only Order in New status can be modified."), new FaultCode("Error"));
                }

                // fields OrderDate and ShippedDate can not be modified directly
                // so restore these fields from source object
                orderBO.OrderDate = sourceOrder.OrderDate;
                orderBO.ShippedDate = sourceOrder.ShippedDate;

                this.ordersDataService.UpdateOrder(orderBO);
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
                var sourceOrder = this.ordersDataService.GetById(orderId);

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

                this.ordersDataService.UpdateOrder(sourceOrder);

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
                var sourceOrder = this.ordersDataService.GetById(orderId);

                if (!Mapper.Map<Order, OrderDTO>(sourceOrder).OrderState.Equals(OrderState.InWork))
                {
                    throw new FaultException(new FaultReason("Only Order in InWork status can be closed."), new FaultCode("Error"));
                }

                sourceOrder.ShippedDate = DateTime.Now;

                this.ordersDataService.UpdateOrder(sourceOrder);

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
                var orderById = this.GetById(orderId);

                if (orderById.OrderState.Equals(OrderState.Closed))
                {
                    throw new FaultException(new FaultReason("The order in Closed state can not be deleted."), new FaultCode("Error"));
                }

                return this.ordersDataService.DeleteOrder(orderId);
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public int DeleteOrder(string id)
        {
            int orderId = 0;

            int.TryParse(id, out orderId);

            return this.DeleteOrder(orderId);
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

        private void ConfigureInMapping()
        {
            Mapper.CreateMap<OrderDTO, Order>();
            Mapper.CreateMap<OrderDetailDTO, OrderDetail>();
            Mapper.CreateMap<ProductDTO, Product>();
        }

        private void ConfigureOutMapping()
        {
            Mapper.CreateMap<Order, OrderDTO>().ForMember(
                d => d.OrderState,
                o => o.MapFrom(src => src.OrderDate == null ? OrderState.New : src.ShippedDate == null ? OrderState.InWork : OrderState.Closed));

            Mapper.CreateMap<OrderDetail, OrderDetailDTO>();
            Mapper.CreateMap<Product, ProductDTO>();
        }
    }
}
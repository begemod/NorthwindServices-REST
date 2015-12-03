namespace WCFServices.OrdersService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    using AutoMapper;
    using DAL.DataServices;
    using DAL.Entities;
    using DAL.Infrastructure;
    using WCFServices.Cotracts.DataContracts;
    using WCFServices.DataContracts;
    using WCFServices.Infrastructure;

    public class BaseOrdersService
    {
        private OrdersDataService ordersDataService;

        protected BaseOrdersService()
        {
            this.ConfigureInMapping();
            this.ConfigureOutMapping();
        }

        protected OrdersDataService DataService
        {
            get
            {
                if (this.ordersDataService == null)
                {
                    var connectionFactory = new NortwindDbConnectionFactory();
                    this.ordersDataService = new OrdersDataService(connectionFactory);
                }

                return this.ordersDataService;
            }
        }

        protected IEnumerable<OrderDTO> GetAllOrders()
        {
            return this.DataService.GetAll().Select(Mapper.Map<Order, OrderDTO>);
        }

        protected OrderDTO GetOrderById(int orderId)
        {
            var orderById = this.DataService.GetById(orderId);

            var result = Mapper.Map<Order, OrderDTO>(orderById);

            return result;
        }

        protected int Delete(int orderId)
        {
            var orderById = this.GetOrderById(orderId);

            if (orderById.OrderState.Equals(OrderState.Closed))
            {
                throw new BusinessException("The order in Closed state can not be deleted.");
            }

            return this.DataService.DeleteOrder(orderId);
        }

        protected int Create(OrderDTO order)
        {
            if (order == null)
            {
                throw new BusinessException("Order should be defined.");
            }

            order.OrderDate = null;
            order.ShippedDate = null;

            var orderId = this.DataService.InsertOrder(Mapper.Map<OrderDTO, Order>(order));

            return orderId;
        }

        protected void Update(OrderDTO order)
        {
            if (order == null)
            {
                throw new BusinessException("Order should be defined.");
            }

            var orderEntity = Mapper.Map<OrderDTO, Order>(order);

            var sourceOrder = this.DataService.GetById(order.OrderId);

            if (!Mapper.Map<Order, OrderDTO>(sourceOrder).OrderState.Equals(OrderState.New))
            {
                throw new BusinessException("Only Order in New status can be modified.");
            }

            // fields OrderDate and ShippedDate can not be modified directly
            // so restore these fields from source object
            orderEntity.OrderDate = sourceOrder.OrderDate;
            orderEntity.ShippedDate = sourceOrder.ShippedDate;

            this.DataService.UpdateOrder(orderEntity);
        }

        #region Setup mapping

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

        #endregion
    }
}

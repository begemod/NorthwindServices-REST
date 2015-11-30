namespace WCFServices.OrdersService
{
    using System.Collections.Generic;
    using System.Linq;
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

        protected int DeleteOrderById(int orderId)
        {
            var orderById = this.GetOrderById(orderId);

            if (orderById.OrderState.Equals(OrderState.Closed))
            {
                throw new BusinessException("The order in Closed state can not be deleted.");
            }

            return this.DataService.DeleteOrder(orderId);
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

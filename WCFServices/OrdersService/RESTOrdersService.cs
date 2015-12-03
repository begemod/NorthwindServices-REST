namespace WCFServices.OrdersService
{
    using System.Collections.Generic;
    using System.Net;
    using System.ServiceModel.Web;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;
    using WCFServices.DataContracts;
    using WCFServices.Infrastructure;

    public class RESTOrdersService : BaseOrdersService, IRestOrdersService
    {
        public IEnumerable<OrderDTO> GetAll()
        {
            return this.GetAllOrders();
        }

        public OrderDTO GetById(string id)
        {
            int orderId = 0;

            int.TryParse(id, out orderId);

            try
            {
                return this.GetOrderById(orderId);
            }
            catch (EntityNotFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public int DeleteOrder(string id)
        {
            int orderId = 0;

            int.TryParse(id, out orderId);

            try
            {
                return this.Delete(orderId);
            }
            catch (BusinessException)
            {
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
            catch (EntityNotFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public int CreateNewOrder(OrderDTO order)
        {
            try
            {
                return this.Create(order);
            }
            catch (BusinessException)
            {
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
        }

        public void UpdateOrder(OrderDTO order)
        {
            try
            {
                this.Update(order);
            }
            catch (BusinessException)
            {
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
            catch (EntityNotFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }
    }
}

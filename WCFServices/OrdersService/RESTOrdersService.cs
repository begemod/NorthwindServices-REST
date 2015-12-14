namespace WCFServices.OrdersService
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.ServiceModel.Web;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;
    using WCFServices.DataContracts;
    using WCFServices.Infrastructure;

    public class RESTOrdersService : BaseOrdersService, IRestOrdersService
    {
        #region IRestOrdersService members

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

        public void ProcessOrder(string id, string status)
        {
            int orderId = 0;

            int.TryParse(id, out orderId);

            try
            {
                var processAction = this.GetProcessAction(status);

                if (processAction == null)
                {
                    throw new WebFaultException(HttpStatusCode.InternalServerError);
                }

                processAction.Invoke(orderId);
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

        #endregion

        #region Private methods

        private Action<int> GetProcessAction(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new WebFaultException(HttpStatusCode.MethodNotAllowed);
            }

            OrderState orderState;

            if (!Enum.TryParse(status, true, out orderState))
            {
                throw new WebFaultException(HttpStatusCode.MethodNotAllowed);
            }

            if (orderState == OrderState.InWork)
            {
                return this.Process;
            }

            if (orderState == OrderState.Closed)
            {
                return this.Close;
            }

            return null;
        }

        #endregion
    }
}

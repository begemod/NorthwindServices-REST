namespace WCFServices.Cotracts
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using WCFServices.DataContracts;

    [ServiceContract(Namespace = "http://epam.com/NorthwindService")]
    public interface IRestOrdersService
    {
        [WebGet(UriTemplate = "orders")]
        IEnumerable<OrderDTO> GetAll();

        [WebGet(UriTemplate = "orders/{id}")]
        OrderDTO GetById(string id);

        [WebInvoke(UriTemplate = "orders/{id}", Method = "DELETE")]
        int DeleteOrder(string id);

        [WebInvoke(UriTemplate = "orders", Method = "POST")]
        int CreateNewOrder(OrderDTO order);

        [WebInvoke(UriTemplate = "orders", Method = "PUT")]
        void UpdateOrder(OrderDTO order);

        [WebInvoke(UriTemplate = "orders?id={id}&status={status}", Method = "PUT")]
        void ProcessOrder(string id, string status);
    }
}
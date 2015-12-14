namespace WCFServices.Cotracts
{
    using System.ServiceModel.Web;

    public interface IRestOrdersSubscriptionService
    {
        [WebInvoke(UriTemplate = "orders/subscriptions/{id}", Method = "POST")]
        bool Subscribe(string id);

        [WebInvoke(UriTemplate = "orders/subscriptions/{id}", Method = "DELETE")]
        bool Unsubscribe(string id);
    }
}

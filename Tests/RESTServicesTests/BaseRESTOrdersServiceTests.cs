namespace Tests.RESTServicesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using RestSharp;
    using WCFServices.DataContracts;

    public class BaseRestOrdersServiceTests
    {
        protected string BaseServiceAddress { get; set; }

        #region Tests

        protected void BaseGetAllTest()
        {
            var response = this.GetAllOrders();

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        protected void BaseGetByIdFaultTest()
        {
            var response = this.GetOrderById(Guid.NewGuid().ToString());

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
        }

        protected void BaseGetByIdTest()
        {
            var orders = this.GetAllOrders().Deserialize<IEnumerable<OrderDTO>>();

            var orderId = orders.First().OrderId;

            var response = this.GetOrderById(orderId.ToString());

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        protected void BaseCreateNewOrderTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var newOrder = this.CreateNewOrder();

            var request = new RestRequest("orders", Method.POST) { RequestFormat = DataFormat.Json };

            var objSer = JsonConvert.SerializeObject(newOrder);

            request.AddBody(newOrder);

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        #endregion

        #region Private methods

        private IRestResponse GetOrderById(string orderId)
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders/{id}", Method.GET);

            request.AddUrlSegment("id", orderId);

            return client.Execute(request);
        }

        private IRestResponse GetAllOrders()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders", Method.GET);

            return client.Execute(request);
        }

        private OrderDTO CreateNewOrder()
        {
            var order = this.GetExistingOrder();

            order.OrderId = 0;
            order.RequiredDate = DateTime.Now.AddDays(1);

            return order;
        }

        private OrderDTO GetExistingOrder(Func<OrderDTO, bool> predicate = null)
        {
            var allOrders = this.GetAllOrders().Deserialize<IEnumerable<OrderDTO>>();

            predicate = predicate ?? (dto => true);

            return allOrders.First(predicate);
        }

    #endregion
    }

    internal static class RestResponseExtensions
    {
        public static T Deserialize<T>(this IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}

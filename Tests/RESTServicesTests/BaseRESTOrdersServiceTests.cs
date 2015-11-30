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
        #region Tests

        protected void GetAllTest(string baseAddress)
        {
            var response = this.GetAllOrders(baseAddress);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        protected void GetByIdFaultTest(string baseAddress)
        {
            var response = this.GetOrderById(baseAddress, Guid.NewGuid().ToString());

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
        }

        protected void GetByIdTest(string baseAddress)
        {
            var orders = this.GetAllOrders(baseAddress)
                             .Deserialize<IEnumerable<OrderDTO>>();

            var orderId = orders.First().OrderId;

            var response = this.GetOrderById(baseAddress, orderId.ToString());

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        #endregion

        #region Private methods

        private IRestResponse GetOrderById(string baseAddress, string orderId)
        {
            var client = new RestClient(baseAddress);

            var request = new RestRequest("orders/{id}", Method.GET);

            request.AddUrlSegment("id", orderId);

            return client.Execute(request);
        }

        private IRestResponse GetAllOrders(string baseAddress)
        {
            var client = new RestClient(baseAddress);

            var request = new RestRequest("orders", Method.GET);

            return client.Execute(request);
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

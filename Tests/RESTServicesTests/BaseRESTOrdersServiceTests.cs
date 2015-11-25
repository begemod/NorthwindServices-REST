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
            var orders = this.GetAllOrders<IEnumerable<OrderDTO>>(baseAddress);

            var orderId = orders.First().OrderId;

            var response = this.GetOrderById(baseAddress, orderId.ToString());

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        private static T GetDeserializedData<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private T GetAllOrders<T>(string baseAddress)
        {
            var response = this.GetAllOrders(baseAddress);

            return GetDeserializedData<T>(response);
        }

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
    }
}

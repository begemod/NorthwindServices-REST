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
        private IEnumerable<OrderDTO> allOrders;

        protected string BaseServiceAddress { get; set; }

        private IEnumerable<OrderDTO> AllOrders
        {
            get
            {
                return this.allOrders ?? (this.allOrders = this.GetAllOrders());
            }
        }

        #region Tests

        protected void BaseGetAllTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders", Method.GET);
            var response = client.Execute(request);

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
            var orderId = this.GetOrder().OrderId;

            var response = this.GetOrderById(orderId.ToString());

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        protected void BaseDeleteFaultTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders/{id}", Method.DELETE);
            request.AddUrlSegment("id", Guid.NewGuid().ToString());

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
        }

        protected void BaseDeleteTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders/{id}", Method.DELETE);

            var orderNotInCloseState = this.GetOrder(o => o.OrderState != OrderState.Closed);

            request.AddUrlSegment("id", orderNotInCloseState.OrderId.ToString());

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        protected void BaseProcessOrderTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders?id={id}&status={status}", Method.PUT);

            var orderInNewStatus = this.GetOrder(o => o.OrderState == OrderState.New && o.RequiredDate > DateTime.Now);

            request.AddUrlSegment("id", orderInNewStatus.OrderId.ToString());
            request.AddUrlSegment("status", OrderState.InWork.ToString());

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            var processedOrder = this.GetOrderById(orderInNewStatus.OrderId.ToString())
                                     .Deserialize<OrderDTO>();

            Assert.IsTrue(processedOrder.OrderState == OrderState.InWork);
        }

        protected void BaseCloseOrderTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders?id={id}&status={status}", Method.PUT);

            var orderInWorkStatus = this.GetOrder(o => o.OrderState == OrderState.InWork);

            request.AddUrlSegment("id", orderInWorkStatus.OrderId.ToString());
            request.AddUrlSegment("status", OrderState.Closed.ToString());

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            var closedOrder = this.GetOrderById(orderInWorkStatus.OrderId.ToString())
                                  .Deserialize<OrderDTO>();

            Assert.IsTrue(closedOrder.OrderState == OrderState.Closed);
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

        private IEnumerable<OrderDTO> GetAllOrders()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("orders", Method.GET);

            return client.Execute(request).Deserialize<IEnumerable<OrderDTO>>();
        }

        private OrderDTO GetOrder(Func<OrderDTO, bool> predicate = null)
        {
            predicate = predicate ?? (dto => true);

            return this.AllOrders.First(predicate);
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

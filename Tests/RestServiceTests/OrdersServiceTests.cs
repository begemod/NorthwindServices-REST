namespace Tests.RestServiceTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;
    using WCFServices.DataContracts;

    [TestClass]
    public class OrdersServiceTests
    {
        private const string OrdersServiceBaseAddress = "http://localhost/NorthwindWCFServices/OrdersService.svc/rest";
        private static RestClient client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            client = new RestClient(OrdersServiceBaseAddress);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var request = new RestRequest("orders", Method.GET);

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Content));
        }

        [TestMethod]
        public void GetByIdFaultTest()
        {
            var request = new RestRequest("orders/{id}", Method.GET);

            request.AddUrlSegment("id", Guid.NewGuid().ToString());

            var response = client.Execute<OrderDTO>(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Error);
        }
    }
}

namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OrdersServiceTestsIISHosting : BaseRestOrdersServiceTests
    {
        private const string OrdersServiceBaseAddressIIS = "http://localhost/NorthwindWCFServices/OrdersServiceREST.svc/";

        [TestMethod]
        public void GetAllTest()
        {
            this.GetAllTest(OrdersServiceBaseAddressIIS);
        }

        [TestMethod]
        public void GetByIdFaultTest()
        {
            this.GetByIdFaultTest(OrdersServiceBaseAddressIIS);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            this.GetByIdTest(OrdersServiceBaseAddressIIS);
        }
    }
}

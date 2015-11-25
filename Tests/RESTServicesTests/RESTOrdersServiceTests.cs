namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RestOrdersServiceTests : BaseRestOrdersServiceTests
    {
        private const string OrdersServiceBaseAddressIIS = "http://localhost/NorthwindWCFServices/OrdersService.svc/rest";
        private const string OrdersServiceBaseAddressSH = "http://epruizhw0228:8733/Design_Time_Addresses/NorthwindWCFServices/OrdersService/rest";

        [TestMethod]
        public void GetAllTest()
        {
            this.GetAllTest(OrdersServiceBaseAddressIIS);
            this.GetAllTest(OrdersServiceBaseAddressSH);
        }

        [TestMethod]
        public void GetByIdFaultTest()
        {
            this.GetByIdFaultTest(OrdersServiceBaseAddressIIS);
            this.GetByIdFaultTest(OrdersServiceBaseAddressSH);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            this.GetByIdTest(OrdersServiceBaseAddressIIS);
            this.GetByIdTest(OrdersServiceBaseAddressSH);
        }
    }
}

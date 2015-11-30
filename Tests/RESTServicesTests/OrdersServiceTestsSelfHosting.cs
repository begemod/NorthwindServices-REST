namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OrdersServiceTestsSelfHosting : BaseRestOrdersServiceTests
    {
        private const string OrdersServiceBaseAddressSH = "http://epruizhw0228:8733/Design_Time_Addresses/NorthwindWCFServices/OrdersService/rest";

        [TestMethod]
        public void GetAllTest()
        {
            this.GetAllTest(OrdersServiceBaseAddressSH);
        }

        [TestMethod]
        public void GetByIdFaultTest()
        {
            this.GetByIdFaultTest(OrdersServiceBaseAddressSH);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            this.GetByIdTest(OrdersServiceBaseAddressSH);
        }
    }
}

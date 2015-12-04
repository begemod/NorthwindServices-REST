namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OrdersServiceTestsSelfHosting : BaseRestOrdersServiceTests
    {
        public OrdersServiceTestsSelfHosting()
        {
            this.BaseServiceAddress = "http://epruizhw0228:8733/Design_Time_Addresses/NorthwindWCFServices/OrdersService/rest";
        }

        [TestMethod]
        public void GetAllTest()
        {
            this.BaseGetAllTest();
        }

        [TestMethod]
        public void GetByIdFaultTest()
        {
            this.BaseGetByIdFaultTest();
        }

        [TestMethod]
        public void GetByIdTest()
        {
            this.BaseGetByIdTest();
        }

        [TestMethod]
        public void DeleteFaultTest()
        {
            this.BaseDeleteFaultTest();
        }

        [TestMethod]
        public void DeleteTest()
        {
            this.BaseDeleteTest();
        }
    }
}

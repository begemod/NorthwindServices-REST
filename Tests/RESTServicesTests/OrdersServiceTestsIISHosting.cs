namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OrdersServiceTestsIISHosting : BaseRestOrdersServiceTests
    {
        public OrdersServiceTestsIISHosting()
        {
            this.BaseServiceAddress = "http://localhost/NorthwindWCFServices/OrdersServiceREST.svc/";
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

        [TestMethod]
        public void ProcessOrderTest()
        {
            this.BaseProcessOrderTest();
        }
    }
}

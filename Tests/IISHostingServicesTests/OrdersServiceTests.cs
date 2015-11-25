namespace Tests.IISHostingServicesTests
{
    using System.ServiceModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tests.BaseTests;

    [TestClass]
    public class OrdersServiceTests : BaseOrdersServiceTests
    {
        private const string BasicHttpBindingIOrdersService = "BasicHttpBinding_IOrdersService_IIS";
        private const string NetTcpBindingIOrdersService = "NetTcpBinding_IOrdersService_IIS";
        private const string WsDualHttpBindingIOrdersSubscriptionService = "WSDualHttpBinding_IOrdersSubscriptionService_IIS";

        [TestMethod]
        public void GetAllTest()
        {
            this.GetAllTest(BasicHttpBindingIOrdersService);
            this.GetAllTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            this.GetByIdTest(BasicHttpBindingIOrdersService);
            this.GetByIdTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void GetByIdFaultTest()
        {
            this.GetByIdFaultTest(BasicHttpBindingIOrdersService);
            this.GetByIdFaultTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void CreateNewOrderFaultTest()
        {
            this.CreateNewOrderFaultTest(BasicHttpBindingIOrdersService);
            this.CreateNewOrderFaultTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        public void CreateNewOrderTest()
        {
            this.CreateNewOrderTest(BasicHttpBindingIOrdersService);
            this.CreateNewOrderTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void UpdateOrderFaultOnNullParameterTest()
        {
            this.UpdateOrderFaultOnNullParameterTest(BasicHttpBindingIOrdersService);
            this.UpdateOrderFaultOnNullParameterTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void UpdateOrderFaultOnAttemptNotInNewStateTest()
        {
            this.UpdateOrderFaultOnAttemptNotInNewStateTest(BasicHttpBindingIOrdersService);
            this.UpdateOrderFaultOnAttemptNotInNewStateTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void DeleteOrderFaultOnAttemptToDeleteNotExistingOrderTest()
        {
            this.DeleteOrderFaultOnAttemptToDeleteNotExistingOrderTest(BasicHttpBindingIOrdersService);
            this.DeleteOrderFaultOnAttemptToDeleteNotExistingOrderTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void DeleteOrderFaultOnAttemptToDeleteClosedOrderTest()
        {
            this.DeleteOrderFaultOnAttemptToDeleteClosedOrderTest(BasicHttpBindingIOrdersService);
            this.DeleteOrderFaultOnAttemptToDeleteClosedOrderTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        public void DeleteOrderTest()
        {
            this.DeleteOrderTest(BasicHttpBindingIOrdersService);
            this.DeleteOrderTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void ProcessOrderFaultTest()
        {
            this.ProcessOrderFaultTest(BasicHttpBindingIOrdersService);
            this.ProcessOrderFaultTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        public void ProcessOrderTest()
        {
            this.ProcessOrderTest(BasicHttpBindingIOrdersService);
            this.ProcessOrderTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void CloseOrderFaultTest()
        {
            this.CloseOrderFaultTest(BasicHttpBindingIOrdersService);
            this.CloseOrderFaultTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        public void CloseOrderTest()
        {
            this.CloseOrderTest(BasicHttpBindingIOrdersService);
            this.CloseOrderTest(NetTcpBindingIOrdersService);
        }

        [TestMethod]
        public void SubscribeUnsubscribeOnOrderStatusChangingEventsTest()
        {
            this.SubscribeUnsubscribeOnOrderStatusChangingEventsTest(WsDualHttpBindingIOrdersSubscriptionService);
        }
    }
}
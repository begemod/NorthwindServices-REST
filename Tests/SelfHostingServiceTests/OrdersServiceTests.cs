﻿namespace Tests.SelfHostingServiceTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tests.BaseTests;
    using Tests.OrdersServiceSelfHosting;

    [TestClass]
    public class OrdersServiceTests : BaseOrdersServiceTests
    {
        private const string BasicHttpBindingIOrdersService = "BasicHttpBinding_IOrdersService1";

        [TestMethod]
        public void GetAllTest()
        {
            this.GetAllTest(BasicHttpBindingIOrdersService);
        }

        [TestMethod]
        public void SimulateLongRunningOperationTest()
        {
            using (var client = new OrdersServiceClient(BasicHttpBindingIOrdersService))
            {
                const byte OperationRunningDurationInSeconds = 20;

                var startAt = DateTime.Now;
                Console.WriteLine(DateTime.Now.ToLongTimeString());


                client.SimulateLongRunningOperation(OperationRunningDurationInSeconds);

                var endAt = DateTime.Now;
                Console.WriteLine(DateTime.Now.ToLongTimeString());

                var duration = endAt - startAt;

                Assert.IsTrue(duration.TotalSeconds >= OperationRunningDurationInSeconds);
            }
        }
    }
}
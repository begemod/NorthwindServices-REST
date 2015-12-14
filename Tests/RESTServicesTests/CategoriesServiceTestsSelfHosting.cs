namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CategoriesServiceTestsSelfHosting : BaseRESTCategoriesServiceTests
    {
        public CategoriesServiceTestsSelfHosting()
        {
            this.BaseServiceAddress = "http://epruizhw0228:8733/Design_Time_Addresses/NorthwindWCFServices/CategoriesService/rest";
        }

        [TestMethod]
        public void GetCategoryNamesTest()
        {
            this.BaseGetCategoryNamesTest();
        }
    }
}

namespace Tests.RESTServicesTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CategoriesServiceTestsIISHosting : BaseRESTCategoriesServiceTests
    {
        public CategoriesServiceTestsIISHosting()
        {
            this.BaseServiceAddress = "http://localhost/NorthwindWCFServices/CategoriesServiceREST.svc/";
        }

        [TestMethod]
        public void GetCategoryNamesTest()
        {
            this.BaseGetCategoryNamesTest();
        }
    }
}

namespace Tests.RESTServicesTests
{
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;

    public class BaseRESTCategoriesServiceTests
    {
        protected string BaseServiceAddress { get; set; }

        protected void BaseGetCategoryNamesTest()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("categories/names", Method.GET);

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
    }
}

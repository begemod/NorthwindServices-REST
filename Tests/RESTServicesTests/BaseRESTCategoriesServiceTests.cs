namespace Tests.RESTServicesTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;

    public class BaseRESTCategoriesServiceTests
    {
        protected string BaseServiceAddress { get; set; }

        protected void BaseGetCategoryNamesTest()
        {
            var response = this.GetCategoryNames();

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        protected void BaseGetCategoryImageTest()
        {
            var allNames = this.GetCategoryNames()
                               .Deserialize<IEnumerable<string>>();

            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("/categories/image/{categoryName}", Method.GET);
            request.AddUrlSegment("categoryName", allNames.First());

            var response = client.Execute(request);

            Assert.IsTrue(response.ResponseStatus == ResponseStatus.Completed);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(response.Content));
        }

        private IRestResponse GetCategoryNames()
        {
            var client = new RestClient(this.BaseServiceAddress);

            var request = new RestRequest("/categories/names", Method.GET);

            return client.Execute(request);
        }
    }
}

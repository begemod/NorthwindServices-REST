namespace WCFServices.Cotracts
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract(Namespace = "http://epam.com/NorthwindService")]
    public interface IRESTCategoriesService
    {
        [WebGet(UriTemplate = "categories/names")]
        IEnumerable<string> GetCategoryNames();
    }
}

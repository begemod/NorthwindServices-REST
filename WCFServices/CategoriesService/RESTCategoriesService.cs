namespace WCFServices.CategoriesService
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.ServiceModel.Web;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;

    public class RESTCategoriesService : BaseCategoriesService, IRESTCategoriesService
    {
        public IEnumerable<string> GetCategoryNames()
        {
            return this.GetNames();
        }

        public Stream GetCategoryImage(string categoryName)
        {
            try
            {
                return this.GetImage(categoryName);
            }
            catch (EntityNotFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }

        public void SaveCategoryImage(string categoryName, Stream image)
        {
            try
            {
                this.SaveImage(categoryName, image);
            }
            catch (EntityNotFoundException)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
        }
    }
}

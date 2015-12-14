namespace WCFServices.CategoriesService
{
    using System.Collections.Generic;
    using WCFServices.Cotracts;

    public class RESTCategoriesService : BaseCategoriesService, IRESTCategoriesService
    {
        public IEnumerable<string> GetCategoryNames()
        {
            return this.GetNames();
        }
    }
}

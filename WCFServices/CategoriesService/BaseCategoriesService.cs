namespace WCFServices.CategoriesService
{
    using System.Collections.Generic;
    using DAL.DataServices;
    using DAL.Infrastructure;

    public class BaseCategoriesService
    {
        private CategoriesDataService categoriesDataService;

        protected CategoriesDataService DataService
        {
            get
            {
                if (this.categoriesDataService == null)
                {
                    var connectionFactory = new NortwindDbConnectionFactory();
                    this.categoriesDataService = new CategoriesDataService(connectionFactory);
                }

                return this.categoriesDataService;
            }
        }

        protected IEnumerable<string> GetNames()
        {
            return this.categoriesDataService.GetCategoryNames();
        }
    }
}

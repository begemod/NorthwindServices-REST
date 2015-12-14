namespace WCFServices.CategoriesService
{
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel;
    using DAL.Infrastructure;
    using WCFServices.Cotracts;
    using WCFServices.DataContracts;

    public class CategoriesService : BaseCategoriesService, ICategoriesService
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
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }

        public void SaveCategoryImage(SendingCategory sendingCategory)
        {
            try
            {
                this.SaveImage(sendingCategory.CategoryName, sendingCategory.CategoryImage);
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }
    }
}

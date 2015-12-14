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
            const int BuffSize = 1000;

            if (sendingCategory == null)
            {
                return;
            }

            this.Validate(sendingCategory);

            var buffer = new byte[BuffSize];
            var memoryStream = new MemoryStream();

            var readed = sendingCategory.CategoryImage.Read(buffer, 0, BuffSize);

            while (readed != 0)
            {
                memoryStream.Write(buffer, 0, readed);
                readed = sendingCategory.CategoryImage.Read(buffer, 0, BuffSize);
            }

            var sourceCategory = this.DataService.GetByCategoryName(sendingCategory.CategoryName);
            sourceCategory.Picture = memoryStream.ToArray();

            this.DataService.UpdateCategoryPicture(sourceCategory);
        }

        private void Validate(SendingCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new FaultException(new FaultReason("Category name is not defined."), new FaultCode("Error"));
            }

            try
            {
                 this.DataService.GetByCategoryName(category.CategoryName);
            }
            catch (EntityNotFoundException exception)
            {
                throw new FaultException(new FaultReason(exception.Message), new FaultCode("Error"));
            }
        }
    }
}

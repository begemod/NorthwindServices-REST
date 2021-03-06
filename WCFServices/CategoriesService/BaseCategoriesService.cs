﻿namespace WCFServices.CategoriesService
{
    using System.Collections.Generic;
    using System.IO;
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
            return this.DataService.GetCategoryNames();
        }

        protected Stream GetImage(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return null;
            }

            var category = this.DataService.GetByCategoryName(categoryName);

            var categoryImage = category.Picture;

            var imageStream = new MemoryStream(categoryImage, 78, categoryImage.Length - 78);

            return imageStream;
        }

        protected void SaveImage(string categoryName, Stream categoryImage)
        {
            const int BuffSize = 1000;

            var buffer = new byte[BuffSize];
            var memoryStream = new MemoryStream();

            var readed = categoryImage.Read(buffer, 0, BuffSize);

            while (readed != 0)
            {
                memoryStream.Write(buffer, 0, readed);
                readed = categoryImage.Read(buffer, 0, BuffSize);
            }

            var sourceCategory = this.DataService.GetByCategoryName(categoryName);
            sourceCategory.Picture = memoryStream.ToArray();

            this.DataService.UpdateCategoryPicture(sourceCategory);
        }
    }
}

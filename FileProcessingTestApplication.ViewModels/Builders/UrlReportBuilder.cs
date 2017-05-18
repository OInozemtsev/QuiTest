using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FileProcessingTestApplication.ViewModels.Extensions;
using FileProcessingTestApplication.ViewModels.Services;

namespace FileProcessingTestApplication.ViewModels.Builders
{
    public class UrlReportBuilder
    {
        #region Поля

        /// <summary>
        /// Сервис, читающий информацию из файла
        /// </summary>
        private readonly UrlFileReadService mUrlFileReadService;

        #endregion//Поля

        public UrlReportBuilder()
        {
            this.mUrlFileReadService = new UrlFileReadService();
        }

        #region Открытые методы

        /// <summary>
        /// Получение списка url
        /// </summary>
        /// <param name="urlFilePath"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UrlReportViewModel>> GetUrlReportsAsync(string urlFilePath, MaxReferenceCountSelectDelegate maxReferenceSelectDelegate)
        {
            Func<Task<IEnumerable<string>>> getUrlsFunc = async () => await mUrlFileReadService.GetUrlsAsync(urlFilePath);
            IEnumerable<string> urls = await getUrlsFunc.WithTry(Enumerable.Empty<string>, (str)=>MessageBox.Show(str, "Ошибка чтения файла", MessageBoxButton.OK, MessageBoxImage.Error));
            return urls.Select(x => new UrlReportViewModel(maxReferenceSelectDelegate) { Url = x });
        }

        #endregion//Открытые методы

    }
}

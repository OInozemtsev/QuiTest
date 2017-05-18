using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileProcessingTestApplication.ViewModels.Services
{
    public class UrlFileReadService
    {
        /// <summary>
        /// Получение коллекции url
        /// </summary>
        /// <param name="urlsFilePath"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetUrlsAsync(string urlsFilePath)
        {
            return await Task.Run(async () =>
            {
                List<string> urls = new List<string>();
                using (StreamReader reader = new StreamReader(urlsFilePath))
                {
                    string url;
                    while ((url = await reader.ReadLineAsync()) != null)
                        urls.Add(url);
                }
                return urls;
            });
        }
    }
}

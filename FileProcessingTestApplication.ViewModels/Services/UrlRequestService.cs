using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileProcessingTestApplication.ViewModels.Services
{
    public class UrlRequestService
    {
        /// <summary>
        /// Обработка url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> RequestUrl(string url)
        {
            string result;
            using (HttpClient httpClient = new HttpClient())
            {
                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage))
                    {
                        result = await httpResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            return result;
        }
    }
}

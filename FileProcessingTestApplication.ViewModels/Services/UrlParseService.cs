using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileProcessingTestApplication.ViewModels.Services
{
    public class UrlParseService
    {
        /// <summary>
        /// Подсчет количества ссылок
        /// </summary>
        /// <param name="htmMessage"></param>
        /// <returns></returns>
        public async Task<int> GetReferenceCount(string htmMessage)
        {
            return await Task.Run(() =>
            {
                MatchCollection matchCollection = Regex.Matches(htmMessage, Constants.cReferenceTag);
                return matchCollection.Count;
            });
        }
    }
}

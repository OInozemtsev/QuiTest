using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FileProcessingTestApplication.ViewModels.Abstract;
using FileProcessingTestApplication.ViewModels.Builders;
using FileProcessingTestApplication.ViewModels.Commands;

namespace FileProcessingTestApplication.ViewModels
{
    public delegate Task MaxReferenceCountSelectDelegate();

    public class MainViewModel : BindableBase
    {
        #region Поля

        /// <summary>
        /// Строитель отчета по url
        /// </summary>
        private UrlReportBuilder mUrlReportBuilder;

        #endregion//Поля

        #region Свойства

        /// <summary>
        /// Путь к файлу с url
        /// </summary>
        private string mUrlFilePath;
        public string UrlFilePath
        {
            get { return mUrlFilePath; }
            set { Set(() => UrlFilePath, ref mUrlFilePath, value); }
        }

        /// <summary>
        /// Результат подсчета ссылк ресурса
        /// </summary>
        public ObservableCollection<UrlReportViewModel> ReportCollection { get; set; }

        #endregion//Свойства

        public MainViewModel()
        {
            ReportCollection = new ObservableCollection<UrlReportViewModel>();
            mUrlReportBuilder = new UrlReportBuilder();
            UrlFilePath = Constants.cUrlFilePath;
        }

        #region Команды

        /// <summary>
        /// Команда начала обработки
        /// </summary>
        private ICommand mStartProcessingCommand;
        public ICommand StartProcessingCommand
            => mStartProcessingCommand ?? (mStartProcessingCommand = new RelayCommand<object>(async parameter => await StartProcessingCommandHandler(parameter)));

        /// <summary>
        /// Обработка нажатия кнопки "НАЧАТЬ"
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private async Task StartProcessingCommandHandler(object parameter)
        {
            ReportCollection.ToList().ForEach(x => x.CancelRequest());
            ReportCollection.Clear();
            IEnumerable<UrlReportViewModel> urls = await mUrlReportBuilder.GetUrlReportsAsync(UrlFilePath, SelectMaxReferenceResult);
            List<UrlReportViewModel> urlList = urls as List<UrlReportViewModel> ?? urls.ToList();
            urlList.ForEach(async x =>
            {
                ReportCollection.Add(x);
                await x.RequestUrl();
            });
        }

        /// <summary>
        /// Определение url с максималоьным числом ссылок
        /// </summary>
        private async Task SelectMaxReferenceResult()
        {
            UrlReportViewModel maxReferenceUrl = await Task.Run(() =>
            {
                List<UrlReportViewModel> urls = ReportCollection.ToList();
                return urls.FirstOrDefault(x => x.IsMaxReference);
            });
            if (maxReferenceUrl != null)
                maxReferenceUrl.IsMaxReference = false;
            maxReferenceUrl = await Task.Run(() =>
            {
                List<UrlReportViewModel> urlsWithValue = ReportCollection.ToList().Where(x => x.ReferenceCount.HasValue).ToList();
                return urlsWithValue.Aggregate(urlsWithValue.First(),
                    (curr, next) => next.ReferenceCount > curr.ReferenceCount ? next : curr);
            });
            maxReferenceUrl.IsMaxReference = true;
        }

        #endregion//Команды
    }
}

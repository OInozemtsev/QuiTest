using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileProcessingTestApplication.ViewModels.Abstract;
using FileProcessingTestApplication.ViewModels.Services;
using FileProcessingTestApplication.ViewModels.Wrappers;

namespace FileProcessingTestApplication.ViewModels
{

    public class UrlReportViewModel : BindableBase
    {
        #region Поля

        /// <summary>
        /// Обработка url
        /// </summary>
        private UrlRequestService mUrlRequestService;

        /// <summary>
        /// Сервис парсинга html
        /// </summary>
        private UrlParseService mUrlParseService;

        /// <summary>
        /// Рандомное значение
        /// </summary>
        private static Random mRandom;

        /// <summary>
        /// Коллекция url
        /// </summary>
        private IEnumerable<UrlReportViewModel> mUrlCollection;

        /// <summary>
        /// Делегат выбора url с максимальным числом ссылок
        /// </summary>
        private MaxReferenceCountSelectDelegate mMaxReferenceCountSelectDelegateInvoker;

        /// <summary>
        /// Отмена обработки
        /// </summary>
        private CancellationTokenSource mCancellationTokenSource;

        #endregion//Поля

        #region Свойства

        /// <summary>
        /// Наименование ресурса
        /// </summary>
        private string mUrl;
        public string Url
        {
            get { return mUrl; }
            set { Set(() => Url, ref mUrl, value); }
        }

        /// <summary>
        /// Количество ссылок
        /// </summary>
        private int? mReferenceCount;
        public int? ReferenceCount
        {
            get { return mReferenceCount; }
            set { Set(() => ReferenceCount, ref mReferenceCount, value); }
        }

        /// <summary>
        /// Враппер обработки url
        /// </summary>
        public WrapAsyncResult<int?> RequestWrapper { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        private string mErrorMessage;
        public string ErrorMessage
        {
            get { return mErrorMessage; }
            set { Set(() => ErrorMessage, ref mErrorMessage, value); }
        }

        /// <summary>
        /// Флаг максимального числа ссылок
        /// </summary>
        private bool mIsMaxReference;
        public bool IsMaxReference
        {
            get { return mIsMaxReference; }
            set { Set(() => IsMaxReference, ref mIsMaxReference, value); }
        }

        #endregion//Свойства

        #region Конструкторы

        static UrlReportViewModel()
        {
            mRandom = new Random(DateTime.Now.Millisecond);
        }

        public UrlReportViewModel(MaxReferenceCountSelectDelegate maxReferenceCountSelectDelegate)
        {
            mMaxReferenceCountSelectDelegateInvoker = maxReferenceCountSelectDelegate;
            mUrlRequestService = new UrlRequestService();
            mUrlParseService = new UrlParseService();
            this.RequestWrapper = new WrapAsyncResult<int?>((source, completionSource) =>
            {
                mCancellationTokenSource = source;
            });
        }

        #endregion//Конструкторы

        #region Открытые методжы

        /// <summary>
        /// Подсчет ссылок
        /// </summary>
        /// <returns></returns>
        public async Task RequestUrl()
        {
            ErrorMessage = string.Empty;
            await RequestWrapper.AwaitAsync(
                cancelableAction: async token =>
                {
                    int delay = mRandom.Next(1, 10);
                    await Task.Delay(TimeSpan.FromSeconds(delay), token);
                    string htmlMessage = await mUrlRequestService.RequestUrl(Url);
                    return await mUrlParseService.GetReferenceCount(htmlMessage);
                },
                callbackAction: result =>
                {
                    if (mCancellationTokenSource.IsCancellationRequested) return;
                    ReferenceCount = result;
                    mMaxReferenceCountSelectDelegateInvoker();
                },
                alertAction: exception =>
                {
                    ErrorMessage = exception.Message;
                }, scheduler: TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Запрос на отмену операции обработки
        /// </summary>
        public void CancelRequest()
        {
            mCancellationTokenSource.Cancel();
            mMaxReferenceCountSelectDelegateInvoker = null;
        }

        #endregion//Открытые методжы
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FileProcessingTestApplication.ViewModels.Wrappers
{
    /// <summary>
    /// Обёртка над асинхронными операциями, позволяющая легко обрабатывать любое состояние асинхронной операции в представлнии. Некоторый методы реализованы под текучим интерфейсом для 
    /// того что бы их можно было дублировать и комбинировать.
    /// </summary>
    /// <typeparam name="T">Любой тип</typeparam>
    public class WrapAsyncResult<T> : INotifyPropertyChanged
    {
        private CancellationTokenSource mCancell;
        private Task<T> mExecution;
        private Action<CancellationTokenSource, TaskCompletionSource<T>> mCancelAction;

        /// <summary>
        /// TaskSource, связанный с отменой асинхронного дейтсия
        /// </summary>
        private TaskCompletionSource<T> mAwaitCancelTaskSource;

        /// <summary>
        /// TaskSource, связанный с окончанием выполнения асинхронного действия
        /// </summary>
        public TaskCompletionSource<T> AwaitResultAsyncSource;

        /// <summary>
        /// Результат операции
        /// </summary>
        public T Result
        {
            get { return mExecution != null && mExecution.Status.Equals(TaskStatus.RanToCompletion) ? mExecution.Result : default(T); }
        }

        /// <summary>
        /// True- операция завершилась,False-операция не завершилась или не инициализированна
        /// </summary>
        public bool IsCompleted
        {
            get { return (mExecution != null && mExecution.IsCompleted) || mCancell.IsCancellationRequested; }
        }
        /// <summary>
        /// True-операция завершилась с ошибкой
        /// </summary>
        public bool IsFaulted { get { return mExecution != null && mExecution.IsFaulted; } }

        /// <summary>
        /// True-операция не завершилась или не инициализированна
        /// </summary>
        public bool IsNotCompleted { get { return mExecution == null || !mExecution.IsCompleted; } }

        /// <summary>
        /// Текущее состояние асинхронной операции
        /// </summary>
        public TaskStatus Status { get { return mExecution == null ? TaskStatus.WaitingToRun : mExecution.Status; } }

        /// <summary>
        /// Операция завершилась буз ошибок
        /// </summary>
        public bool IsSuccessfullyCompleted { get { return Status == TaskStatus.RanToCompletion; } }

        public WrapAsyncResult(Action<CancellationTokenSource, TaskCompletionSource<T>> cancelAction)
        {
            mExecution = Task.FromResult(default(T));
            AwaitResultAsyncSource = new TaskCompletionSource<T>();
            mCancelAction = cancelAction;
        }

        /// <summary>
        /// Враппер, адаптированный для XAMARIN
        /// </summary>
        /// <param name="cancelableAction">Асинхронное действие</param>
        /// <param name="scheduler"></param>
        /// <param name="alertAction">Действие, вызываемое при возникновении ошибки</param>
        /// <param name="callbackAction"></param>
        /// <returns></returns>
        public async Task<T> AwaitAsync(Func<CancellationToken, Task<T>> cancelableAction, Action<T> callbackAction, Action<Exception> alertAction = null, TaskScheduler scheduler = null)
        {
            mCancell = mCancell != null && !mCancell.IsCancellationRequested ? mCancell : new CancellationTokenSource();
            if (AwaitResultAsyncSource == null || AwaitResultAsyncSource.Task.Status.Equals(TaskStatus.RanToCompletion))
                AwaitResultAsyncSource = new TaskCompletionSource<T>();
            scheduler = scheduler ?? TaskScheduler.FromCurrentSynchronizationContext();
            if (mAwaitCancelTaskSource == null || mAwaitCancelTaskSource.Task.Status.Equals(TaskStatus.RanToCompletion))
                mAwaitCancelTaskSource = new TaskCompletionSource<T>();
            mCancelAction(mCancell, mAwaitCancelTaskSource);
            mExecution = Task.WhenAny(mAwaitCancelTaskSource.Task, cancelableAction(mCancell.Token).ContinueWith(t => t.Result))
                .ContinueWith(x =>
                {
                    NotifyUpdateProperies();
                    return x.Result.Result;
                }, scheduler)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        Debug.Assert(x.Exception != null, "x.Exception != null");
                        alertAction?.Invoke(GetSourceException(x.Exception));
                        return default(T);
                    }
                    if (mCancell.IsCancellationRequested) return default(T);
                    callbackAction(x.Result);
                    return x.Result;
                }, scheduler);
            NotifyUpdateProperies();
            await mExecution.ContinueWith(o =>
            {
                NotifyUpdateProperies();
                AwaitResultAsyncSource.TrySetResult(o.Result);
            }, TaskScheduler.Default);
            return await AwaitResultAsyncSource.Task;
        }

        /// <summary>
        /// Обновляет UI
        /// </summary>
        private void NotifyUpdateProperies()
        {
            OnPropertyChanged("Status");
            OnPropertyChanged("Result");
            OnPropertyChanged("IsCompleted");
            OnPropertyChanged("IsFaulted");
            OnPropertyChanged("IsNotCompleted");
            OnPropertyChanged("IsSuccessfullyCompleted");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Получение основного исключения
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private Exception GetSourceException(Exception exception)
        {
            return exception.InnerException == null
                ? exception
                : GetSourceException(exception.InnerException);
        }

    }
}

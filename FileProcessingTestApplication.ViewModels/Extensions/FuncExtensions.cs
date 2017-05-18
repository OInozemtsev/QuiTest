using System;
using System.Threading.Tasks;

namespace FileProcessingTestApplication.ViewModels.Extensions
{
    public static class FuncExtensions
    {
        /// <summary>
        /// Выполнение таска в блоке try-cacth
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="defaultResultFunc"></param>
        /// <returns></returns>
        public static async Task<T> WithTry<T>(this Func<Task<T>> func, Func<T> defaultResultFunc, Action<string> alertAction)
        {
            try
            {
                return await func.Invoke();
            }
            catch (Exception ex)
            {
                alertAction.Invoke(ex.Message);
            }
            return defaultResultFunc == null ? await Task.FromResult(default(T)) : defaultResultFunc.Invoke();
        }
    }
}

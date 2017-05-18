using System;
using System.Windows.Input;

namespace FileProcessingTestApplication.ViewModels.Commands
{
    public class RelayCommand<T> : ICommand
    {
        #region Поля

        private readonly Action<T> mExecute = null;
        private readonly Predicate<T> mCanExecute = null;

        #endregion//Поля


        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            mExecute = execute;
            mCanExecute = canExecute;
        }

        #region Открытые методы

        /// <summary>
        /// Получение значение возмодности выполнения команды 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return mCanExecute?.Invoke((T)parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mExecute((T)parameter);
        }

        #endregion//Открытые методы
    }
}

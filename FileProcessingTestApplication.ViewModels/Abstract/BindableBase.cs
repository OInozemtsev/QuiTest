using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace FileProcessingTestApplication.ViewModels.Abstract
{
    public abstract class BindableBase : INotifyPropertyChanged, IDisposable
    {
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            PropertyChangedEventHandler changedEventHandler = PropertyChanged;
            if (changedEventHandler == null)
                return;
            string propertyName = GetPropertyName(propertyExpression);
            changedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(@"Неправильный аргумент", nameof(propertyExpression));

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(@"Неправильный аргумент", nameof(propertyExpression));
            return propertyInfo.Name;
        }

        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, EventHandler raisedEvent = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            RaisePropertyChanged(propertyExpression);
            raisedEvent?.Invoke(this, new EventArgs());
            return true;
        }

        /// <summary>
        /// Установка значения свойства и обновление всех заинтересованных команд
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            RaisePropertyChanged(propertyExpression);
            return true;
        }

        #region INotifyProperties
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void Dispose()
        {
            DisposeManagedResources();
        }

        protected virtual void DisposeManagedResources() { }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace FileProcessingTestApplication.ViewModels.Converters.Abstract
{
    public abstract class ConverterBase<T> : MarkupExtension, IValueConverter
    where T : class, new()
    {
        public abstract object Convert(object value, Type targetType, object parameter,
            CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region MarkupExtension members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows;
using FileProcessingTestApplication.View.Converters.Abstract;

namespace FileProcessingTestApplication.View.Converters
{
    public  class NullOrEmptyToVisibleConverter : ConverterBase<NullOrEmptyToVisibleConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            return string.IsNullOrEmpty(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

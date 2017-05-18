using System;
using System.Globalization;
using System.Windows;
using FileProcessingTestApplication.ViewModels.Converters.Abstract;

namespace FileProcessingTestApplication.ViewModels.Converters
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

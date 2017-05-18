using System;
using System.Globalization;
using System.Windows;
using FileProcessingTestApplication.View.Converters.Abstract;

namespace FileProcessingTestApplication.View.Converters
{
    public class BoolToVisibleConverter : ConverterBase<BoolToVisibleConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            if (parameter != null && parameter.Equals("Hidden")) visibility = Visibility.Hidden;
            return (bool)value ? Visibility.Visible : visibility;
        }
    }
}

using System;
using System.Globalization;
using FileProcessingTestApplication.ViewModels.Converters.Abstract;

namespace FileProcessingTestApplication.ViewModels.Converters
{
    public  class InvertNullOrEmptyToBoolConverter : ConverterBase<InvertNullOrEmptyToBoolConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return true;
            return !string.IsNullOrEmpty(value.ToString());
        }
    }
}

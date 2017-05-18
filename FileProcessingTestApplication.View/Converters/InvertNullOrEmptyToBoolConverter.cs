using System;
using System.Globalization;
using FileProcessingTestApplication.View.Converters.Abstract;

namespace FileProcessingTestApplication.View.Converters
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Microsoft.Data.Sqlite;

namespace FastResearch.Converter
{
    public class NullableBooleanToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as bool?) == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new Nullable<bool>((bool)value);
        }
    }
}

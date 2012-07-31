using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace SurveyTool
{
    class RadioButtonEnumConverter : IValueConverter
    {
         // Convert enum [value] to boolean, true if matches [param]
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            return value.Equals(param);
        }

        // Convert boolean to enum, returning [param] if true
        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            return (bool)value ? param : Binding.DoNothing;
        }
    }
}

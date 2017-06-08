﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SourceCounterEx.Converter
{
    public class ColSpanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

      
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace HbbCompetitiePlanner.Helpers {
    class AvondConverter : IValueConverter {

        public static AvondConverter Current { get; } = new AvondConverter();

        private AvondConverter() {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (DayOfWeek)value == DayOfWeek.Wednesday ? "Woensdag" : "Donderdag";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}

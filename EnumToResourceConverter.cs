using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace Common.Wpf
{
    public class EnumToResourceConverter : IValueConverter
    {
        public ResourceManager ResourceManager { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var resourceKey = value.GetType().Name + "_" + value;

            var resourceValue = ResourceManager.GetString(resourceKey);

            return resourceValue ?? string.Format("EnumToResourceConverter: {0} not found", resourceKey);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

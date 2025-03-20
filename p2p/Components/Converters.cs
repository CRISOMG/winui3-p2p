using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace p2p.Components
{

    public class GenericValueConverter<TInput, TOutput> : IValueConverter
    {
        private readonly Func<TInput?, TOutput?> convert;
        private readonly Func<TOutput?, TInput?> convertBack;

        public GenericValueConverter(Func<TInput?, TOutput?> convert, Func<TOutput?, TInput?> convertBack = null)
        {
            this.convert = convert ?? throw new ArgumentNullException(nameof(convert));
            this.convertBack = convertBack;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TInput input)
                return convert(input);
            return default(TOutput);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (convertBack != null && value is TOutput output)
                return convertBack(output);
            return default(TInput);
        }
    }

    class Converters
    {
        public static readonly IValueConverter NullOrEmptyToCollapsed =
           new GenericValueConverter<string, Visibility>(
               value => string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible
           );
        public static readonly IValueConverter BoolToVisibility =
            new GenericValueConverter<bool, Visibility>(
                value => value ? Visibility.Visible : Visibility.Collapsed,
                value => value == Visibility.Visible
            );

    }
}

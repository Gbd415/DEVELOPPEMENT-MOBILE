using System.Globalization;

namespace EduCam.Converters;

public class BoolToFinishTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLast && isLast)
            return "Voir le résultat";
        return "Question suivante";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

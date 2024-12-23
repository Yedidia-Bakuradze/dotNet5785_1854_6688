using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL;

/// <summary>
/// Converts Int value into a boolean true value to disable the editing on this field
/// </summary>
class ConvertIntToReadOnlyTrue : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => true;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

}

/// <summary>
/// Converts CallStatus value into a boolean true value to disable the editing on this field
/// </summary>
class ConvertStatusToReadOnlyTrue : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => true;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}


class ConvertBooleanToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((bool)value)
        {
            case true:
                return Visibility.Visible;
            case false:
                return Visibility.Hidden;
        }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((Visibility)value) == Visibility.Visible;
}
class ConvertVolunteerEnditingModeToReadOnlyValue: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value == "Add Volunteer";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return "Add Volunteer";
        return "Update Volunteer";
    }
}

class ConvertCallInProgressToVisibility : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return Visibility.Hidden;
        return Visibility.Visible;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
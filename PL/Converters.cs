using System.Buffers;
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

#region Admin Window's Converters
#region Sub Screen's Visibility
/// <summary>
/// This convertor converts between an enum value which is made for managing the sub-screen in the Admin's widnow to a Visibitiy value
/// Specificlly for the Clock Manager Sub-Screen view
/// </summary>
class ConvertForClockManagerOperationSubScreenModeToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((OperationSubScreenMode)value == OperationSubScreenMode.ClockManager)
            return Visibility.Visible;
        return Visibility.Hidden;

    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return OperationSubScreenMode.ClockManager;
        return OperationSubScreenMode.Closed;
    }
}

/// <summary>
/// This convertor converts between an enum value which is made for managing the sub-screen in the Admin's widnow to a Visibitiy value
/// Specificlly for the RiskRange Manager Sub-Screen view
/// </summary>
class ConvertForRiskRangeManagerOperationSubScreenModeToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((OperationSubScreenMode)value == OperationSubScreenMode.RiskRangeManager)
            return Visibility.Visible;
        return Visibility.Hidden;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return OperationSubScreenMode.RiskRangeManager;
        return OperationSubScreenMode.Closed;
    }
}

/// <summary>
/// This convertor converts between an enum value which is made for managing the sub-screen in the Admin's widnow to a Visibitiy value
/// Specificlly for the Action Manager Sub-Screen view
/// </summary>
class ConvertForActionManagerOperationSubScreenModeToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((OperationSubScreenMode)value == OperationSubScreenMode.ActionManger)
            return Visibility.Visible;
        return Visibility.Hidden;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return OperationSubScreenMode.ActionManger;
        return OperationSubScreenMode.Closed;
    }
}
#endregion

//TODO: Consider using it for the buttons to disappear when selected
#region Button's Visibility
/// <summary>
/// This convertor converts between an enum value which is made for managing hiding or showing the operation button in the Admin's widnow to a Visibitiy value
/// Specificlly for the Clock Manager Sub-Screen button
/// </summary>
class ConvertForClockManagerOperationSubScreenModeToBtnVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((OperationSubScreenMode)value != OperationSubScreenMode.ClockManager)
            return Visibility.Visible;
        return Visibility.Hidden;

    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return OperationSubScreenMode.ClockManager;
        return OperationSubScreenMode.Closed;
    }
}

/// <summary>
/// This convertor converts between an enum value which is made for managing hiding or showing the operation button in the Admin's widnow to a Visibitiy value
/// Specificlly for the Risk Range Manager Sub-Screen button
/// </summary>
class ConvertForRiskRangeManagerOperationSubScreenModeToBtnVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((OperationSubScreenMode)value != OperationSubScreenMode.RiskRangeManager)
            return Visibility.Visible;
        return Visibility.Hidden;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return OperationSubScreenMode.RiskRangeManager;
        return OperationSubScreenMode.Closed;
    }
}

/// <summary>
/// This convertor converts between an enum value which is made for managing hiding or showing the operation button in the Admin's widnow to a Visibitiy value
/// Specificlly for the Action Manager Sub-Screen button
/// </summary>
class ConvertForActionManagerOperationSubScreenModeToBtnVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((OperationSubScreenMode)value != OperationSubScreenMode.ActionManger)
            return Visibility.Visible;
        return Visibility.Hidden;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return OperationSubScreenMode.ActionManger;
        return OperationSubScreenMode.Closed;
    }
}
#endregion
#endregion

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
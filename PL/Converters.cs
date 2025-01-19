using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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

#region Volunteer List's Convertors

/// <summary>
/// This converter converts the IsActive and CallId values of the VolunteerInList entity to a nice background color
/// </summary>
class ConvertIsActiveAndCallIdToBackgroundColor : IMultiValueConverter
{
    /// <summary>
    /// This convert returns the background color for each user based on its activies in the firm
    /// </summary>
    /// <param name="values">1st value: IsActive, 2nd value: CallId</param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isActive = (bool)values[0];
        int? callId =(int?)values[1];
        if (isActive)
        {
            //Active and doesn't have a call
            if(callId is null)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ED280"));
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6CBDDB"));

            //Active and has a call
        }
        //Not active and no call
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5454"));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [false, 0];
}

#endregion

#region Call List Window Convertors
class ConvertStatusToBackgroundColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((BO.CallStatus)value)
        {
            case BO.CallStatus.Open:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1AE751"));
            case BO.CallStatus.InProgress:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9500"));
            case BO.CallStatus.Closed:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FC4C4E"));

            case BO.CallStatus.Expiered:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#828282"));

            case BO.CallStatus.OpenAndRisky:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8E71A"));

            case BO.CallStatus.InProgressAndRisky:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5900"));
            default:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5900"));

        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.CallStatus.Open;
}

class ConvertStatusToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((BO.CallStatus)value == BO.CallStatus.InProgress || (BO.CallStatus)value == BO.CallStatus.InProgressAndRisky)
            return Visibility.Visible;
        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.CallStatus.Open;
}
#endregion


internal class ConvertActiveStatusToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? Visibility.Visible : Visibility.Hidden;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
}
internal class ConvertActiveStatusToVisibilityReveresed : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value == false ? Visibility.Visible : Visibility.Hidden;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
}
internal class ConvertStatusToVisibilityLevelOne : IMultiValueConverter
{
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        if (((BO.CallStatus)value[0] == BO.CallStatus.Open || (BO.CallStatus)value[0] == BO.CallStatus.OpenAndRisky))
            return false;

        return (string)value[1] != "Add Call";
    }
    public object []ConvertBack(object value, Type []targetType, object parameter, CultureInfo culture) => [];
}
internal class ConvertStatusToVisibilityLevelTwo : IMultiValueConverter
{
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        if (((BO.CallStatus)(value[0]) != BO.CallStatus.Expiered || (BO.CallStatus)(value[0]) == BO.CallStatus.Closed))
            return true;
        return (string)value[1] == "Add Call";
    }
    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) => [];
}
internal class ConvertStatusToVisibilityLevelThree : IMultiValueConverter
{
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)(new ConvertStatusToVisibilityLevelTwo().Convert(value, targetType, parameter, culture));
    }
    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) => [];
}
internal class ConvertBooleanToVisibility : IValueConverter
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
internal class ConvertVolunteerEnditingModeToReadOnlyValue: IValueConverter
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
internal class ConvertCallInProgressToVisibility : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return Visibility.Hidden;
        return Visibility.Visible;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
internal class ConvertRoleToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (BO.UserRole)value == BO.UserRole.Volunteer ? Visibility.Hidden : Visibility.Visible;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.UserRole.Volunteer;
}
internal class ConvertRoleToVisibilityReveresed : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (BO.UserRole)value == BO.UserRole.Admin ? Visibility.Hidden : Visibility.Visible;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.UserRole.Volunteer;
}
internal class ConvertTypeOfEndingToBackgroundColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        switch ((BO.TypeOfEndingCall?)value)
        {
            case BO.TypeOfEndingCall.Treated:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1AE751"));
            case BO.TypeOfEndingCall.SelfCanceled:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FC4C4E"));
            case BO.TypeOfEndingCall.AdminCanceled:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5900"));
            case BO.TypeOfEndingCall.CancellationExpired:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8E71A"));
            default:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#828282"));
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
internal class ConvertCallInProgressToVisibilityReverese: IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return Visibility.Visible;
        return Visibility.Hidden;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
internal class ConvertRoleToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (BO.UserRole)value == BO.UserRole.Admin
        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00EE"))
        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1FBCFF"));

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.UserRole.Volunteer;
}
internal class ConvertRoleToReadOnly : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (BO.UserRole)value == BO.UserRole.Admin;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.UserRole.Volunteer;
}
internal class ConvertTypeOfCallToBackgroundColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((BO.CallType)value)
        {
            case BO.CallType.Undefined:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B0BEC5")); // Light Blue Gray
            case BO.CallType.FlatTire:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8A80")); // Soft Red
            case BO.CallType.BatteryJumpStart:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD180")); // Light Orange
            case BO.CallType.LockedOut:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF59D")); // Pale Yellow
            case BO.CallType.OutOfFuel:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C5E1A5")); // Soft Green
            case BO.CallType.CarStuck:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#81D4FA")); // Light Blue
            case BO.CallType.ChildLockedInCar:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B39DDB")); // Soft Purple
            case BO.CallType.TrafficDirection:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80CBC4")); // Aqua
            case BO.CallType.TowingAssistance:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAB91")); // Coral
            case BO.CallType.MinorRoadRepair:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6EE9C")); // Lemon Yellow
            case BO.CallType.SpecialAssistance:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CE93D8")); // Lavender
            default:
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CFD8DC")); // Neutral Gray
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BO.CallType.OutOfFuel;
}
using System.Collections;
namespace Helpers;

internal static class Tools
{
    /// <summary>
    /// Given the generic value T, this method will generate a stirng containing every field and its value for the given "val" instance
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    /// <param name="val">The instance which is needed to be converted to a string, containing its values</param>
    /// <returns>a string containig all the fields of the T class and the values of the val instance</returns>
    /// <exception cref="Exception">If the instance is null the program will throw an error</exception>
    public static string ToStringProperty<T>(this T val)
    {
        string msg = "";
        if(val == null)
        {
            throw new Exception("Internal Error: Cann't represent string for null values");
        }
        else
        {
            Type currentVarType = val.GetType();
            msg += $"Entity Type: {currentVarType.Name}\n";
            foreach (System.Reflection.PropertyInfo? field in currentVarType.GetProperties())
            {
                var fieldValue = field.GetValue(val);
                msg += $"{field.Name}:";

                //Type of field (IEnumerable / Simple field)
                if(fieldValue is IEnumerable && fieldValue is not string)
                {
                    msg += " [";
                    foreach(var element in (IEnumerable)fieldValue)
                    {
                        msg += $" {element}";
                    }
                    msg += "]\n";
                }
                else
                {
                    msg += $" {fieldValue}\n";
                }
            }
        }
        return msg;
    }
}

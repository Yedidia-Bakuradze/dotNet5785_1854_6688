namespace BO;

#region Dal-BL Exceptions
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? msg) : base(msg) { }
    public BlDoesNotExistException(string msg, DO.DalDoesNotExistException ex) : base(msg, ex) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? msg) : base(msg) { }
    public BlAlreadyExistsException(string msg, DO.DalAlreadyExistsException ex) : base(msg, ex) { }
}

#endregion DalBLException


#region BL Specific Exceptions
/// <summary>
/// This exception is used as an indicator if one or more values of a entity (Prop?) is not valid due to foramtting
/// </summary>
[Serializable]
public class BlInvalidEntityDetails : Exception
{
    public BlInvalidEntityDetails(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is used as an indicator if one or more values of a entity (Prop?) is not valid due to logics
/// </summary>
[Serializable]
public class BlInvalidEntityFieldFormatting : Exception
{
    public BlInvalidEntityFieldFormatting(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is used as an indicator if there are some unimplemented methods
/// </summary>
[Serializable]
public class BlUnimplementedMethodOrFunction : Exception
{
    public BlUnimplementedMethodOrFunction(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown where the system attempts to delete an entity record which is related to other records (Like a volunteer which has an assignment)
/// </summary>
[Serializable]
public class BlEntityRecordIsNotEmpty : Exception
{
    public BlEntityRecordIsNotEmpty(string? msg) : base(msg) { }
}

/// <summary>
/// This exception handles cases where the past enum value is not valid for the operation that the switch depends on
/// </summary>
[Serializable]
public class BlInvalidDistanceCalculationException : Exception
{
    public BlInvalidDistanceCalculationException(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is used when there is a failur in Http requests
/// </summary>
[Serializable]
public class BlHttpGetException : Exception
{
    public BlHttpGetException(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown when the reqiered XElement tag is not found in the given XML tree
/// </summary>
[Serializable]
public class BlXmlElementDoesntExsist : Exception
{
    public BlXmlElementDoesntExsist(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown when the system tries to make a forbidden action such as chaneging user's role mode without access
/// </summary>
[Serializable]
public class BlForbidenSystemActionExeption : Exception
{
    public BlForbidenSystemActionExeption(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown when the system tries to make un registered operation
/// might occure in switch statments when the value is not a valid enum value for that operation
/// </summary>
[Serializable]
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string?msg):base(msg){}
}

/// <summary>
/// This exception is thrown when the user's tries to enter a value that cann't be converted to its real value
/// </summary>
[Serializable]
public class BlInputValueUnConvertableException : Exception
{
    public BlInputValueUnConvertableException(string? msg) : base(msg) {}
}

/// <summary>
/// This exception is thrown when a null value is inserted into a method when he is not welcomeed
/// </summary>
[Serializable]
public class BlUnWantedNullValueException:Exception
{
    public BlUnWantedNullValueException(string? msg) : base(msg) { }
}

[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException() :base("BL Says: The system is running the simulator, any manual modifications are not allowed") { }
    public BLTemporaryNotAvailableException(string msg) : base(msg)
    {
        
    }
}

[Serializable]
public class BlInvalidCordinatesConversionException : Exception
{
    public BlInvalidCordinatesConversionException(string? street) : base($"Bl Says: Street address {street} does not exists"){}
}
#endregion BL

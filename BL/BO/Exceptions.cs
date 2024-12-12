namespace BO;

#region DalBLException
[Serializable]
public class BlDoesNotExistException : Exception
{
    DO.DalDoesNotExistException? oldEx;
    public BlDoesNotExistException(string? msg, DO.DalDoesNotExistException? ex=null) : base(msg) => oldEx = ex;
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    DO.DalAlreadyExistsException? oldEx;
    public BlAlreadyExistsException(string? msg, DO.DalAlreadyExistsException? ex=null) : base(msg) => oldEx = ex;
}

#endregion DalBLException


#region BL
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
public class BlInvalidEnumValueOperationException : Exception
{
    public BlInvalidEnumValueOperationException(string?msg):base(msg)
    {
        
    }
}

#endregion BL

namespace BO;

#region DalBLException
[Serializable]
public class BoDoesNotExistException : Exception
{
    DO.DalDoesNotExistException? oldEx;
    public BoDoesNotExistException(string? msg, DO.DalDoesNotExistException? ex=null) : base(msg) => oldEx = ex;
}

[Serializable]
public class BoAlreadyExistsException : Exception
{
    DO.DalAlreadyExistsException? oldEx;
    public BoAlreadyExistsException(string? msg, DO.DalAlreadyExistsException? ex=null) : base(msg) => oldEx = ex;
}

#endregion DalBLException


#region BL
/// <summary>
/// This exception is used as an indicator if one or more values of a entity (Prop?) is not valid due to foramtting
/// </summary>
[Serializable]
public class BoInvalidEntityDetails : Exception
{
    public BoInvalidEntityDetails(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is used as an indicator if one or more values of a entity (Prop?) is not valid due to logics
/// </summary>
[Serializable]
public class BoInvalidEntityFieldFormatting : Exception
{
    public BoInvalidEntityFieldFormatting(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is used as an indicator if there are some unimplemented methods
/// </summary>
[Serializable]
public class BoUnimplementedMethodOrFunction : Exception
{
    public BoUnimplementedMethodOrFunction(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown where the system attempts to delete an entity record which is related to other records (Like a volunteer which has an assignment)
/// </summary>
[Serializable]
public class BoEntityRecordIsNotEmpty : Exception
{
    public BoEntityRecordIsNotEmpty(string? msg) : base(msg) { }
}

/// <summary>
/// This exception handles cases where the past enum value is not valid for the operation that the switch depends on
/// </summary>
[Serializable]
public class BoInvalidDistanceCalculationException : Exception
{
    public BoInvalidDistanceCalculationException(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is used when there is a failur in Http requests
/// </summary>
[Serializable]
public class BoHttpGetException : Exception
{
    public BoHttpGetException(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown when the reqiered XElement tag is not found in the given XML tree
/// </summary>
[Serializable]
public class BoXmlElementDoesntExsist : Exception
{
    public BoXmlElementDoesntExsist(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown when the system tries to make a forbidden action such as chaneging user's role mode without access
/// </summary>
[Serializable]
public class BoForbidenSystemActionExeption : Exception
{
    public BoForbidenSystemActionExeption(string? msg) : base(msg) { }
}

/// <summary>
/// This exception is thrown when the system tries to make un registered operation
/// might occure in switch statments when the value is not a valid enum value for that operation
/// </summary>
public class BoInvalidEnumValueOperationException : Exception
{
    public BoInvalidEnumValueOperationException(string?msg):base(msg)
    {
        
    }
}

#endregion BL

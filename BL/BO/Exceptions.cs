namespace BO;

#region DalBLException
[Serializable]
public class BoDoesNotExistException : Exception
{
    public BoDoesNotExistException(string? msg,DO.DalDoesNotExistException ex) : base(msg) { }
}

[Serializable]
public class BoAlreadyExistsException : Exception
{
    public BoAlreadyExistsException(string? msg,DO.DalAlreadyExistsException ex) : base(msg) { }
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
/// This exception is used when there is a failur in Http requests
/// </summary>
[Serializable]
public class BoHttpGetException : Exception
{
    public BoHttpGetException(string? msg) : base(msg) { }
}

[Serializable]
public class BoForbidenActionExeption : Exception
{
    public BoForbidenActionExeption(string? msg) : base(msg) { }
}

#endregion BL

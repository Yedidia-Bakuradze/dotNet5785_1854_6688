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
[Serializable]
public class BoInvalidEntityDetails : Exception
{
    public BoInvalidEntityDetails(string? msg) : base(msg) { }
}

[Serializable]
public class BoInvalidEntityFieldFormatting : Exception
{
    public BoInvalidEntityFieldFormatting(string? msg) : base(msg) { }
}



#endregion BL

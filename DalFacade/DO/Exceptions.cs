
namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? msg) : base(msg) { }
}

[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? msg) : base(msg) { }
}

/// <summary>
/// Such Exception is used in the Initialization process when the program tries to create the volunteer entities before the call and assignment entities
/// which shall be forbidden since the volunteer is depended on them
/// </summary>
[Serializable]
public class DalUnGeneratedDependedList: Exception
{
    public DalUnGeneratedDependedList(string? msg) : base(msg) { }
}

[Serializable]
public class DalInValidConfigVariable: Exception
{
    public DalInValidConfigVariable(string? msg) : base(msg) { }
}

[Serializable]
public class DalForbiddenOperation : Exception
{
    public DalForbiddenOperation(string? msg) : base(msg) { }
}







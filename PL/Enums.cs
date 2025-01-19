using System.Collections;

namespace PL;

internal class CallTypesCollection: IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
        (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class UserRoleCollection : IEnumerable
{
    static readonly IEnumerable<BO.UserRole> s_enums =
        (Enum.GetValues(typeof(BO.UserRole)) as IEnumerable<BO.UserRole>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallStatus> s_enums =
        (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class VolunteerInListField : IEnumerable
{
    static readonly IEnumerable<BO.VolunteerInListField> s_enums =
    (Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class CallInListField : IEnumerable
{
    static readonly IEnumerable<BO.CallInListFields> s_enums =
    (Enum.GetValues(typeof(BO.CallInListFields)) as IEnumerable<BO.CallInListFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class OpenCallInListField : IEnumerable
{
    static readonly IEnumerable<BO.OpenCallFields> s_enums =
    (Enum.GetValues(typeof(BO.OpenCallFields)) as IEnumerable<BO.OpenCallFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class ClosedCallInListField : IEnumerable
{
    static readonly IEnumerable<BO.ClosedCallInListFields> s_enums =
    (Enum.GetValues(typeof(BO.ClosedCallInListFields)) as IEnumerable<BO.ClosedCallInListFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class TypeOfRangeCollection : IEnumerable
{
    static readonly IEnumerable<BO.TypeOfRange> s_enums =
        (Enum.GetValues(typeof(BO.TypeOfRange)) as IEnumerable<BO.TypeOfRange>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This enum is made for the admin's sub-screen to show the desired operations screeen
/// </summary>
public enum OperationSubScreenMode { Closed,ClockManager,RiskRangeManager,ActionManger}
public enum TypeOfMap { Pin, Route,MultipleTypeOfRoutes }

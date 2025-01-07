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
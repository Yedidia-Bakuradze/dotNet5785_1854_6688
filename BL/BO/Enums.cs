using Helpers;

namespace BO;
public enum UserRole { Admin, Volunteer, Undefined };
public enum TypeOfRange { AirDistance, WalkingDistance, DrivingDistance };
public enum TypeOfEndingCall { Treated, SelfCanceled, AdminCanceled, CancellationExpired, Undefined };
public enum CallType { Undefined, FoodPreparation, FoodDelivery }
public enum CallInProgressStatus { Taken, TakenAndInRisk }
public enum CallInListFields
{
    Id,
    CallId,
    TypeOfCall,
    OpenningTime,
    TimeToEnd,
    LastVolunteerName,
    TimeElapsed,
    Status,
    TotalAlocations
}

public enum OpenCallFields {CallId, TypeOfCall, Description, CallFullAddress, OpenningTime, LastTimeForClosingTheCall, DistanceFromVolunteer }
public enum ClosedCallInListFields { Id, TypeOfCall, CallAddress, CallStartTime, EnteryTime, ClosingTime, TypeOfClosedCall }

/// <summary>
/// The Call Status
/// </summary>
public enum CallStatus
{

    /// <summary>
    /// Free to be taken
    /// </summary>
    Open,

    /// <summary>
    /// Currently handled by another volunteer
    /// </summary>
    InProgress,

    /// <summary>
    /// Closed by a volunteer
    /// </summary>
    Closed,

    /// <summary>
    /// Hasn't been taken care of, or hasn't been done in time
    /// </summary>
    Expiered,

    /// <summary>
    /// Free to be taken but its close to its deadline
    /// </summary>
    OpenAndRisky,

    /// <summary>
    /// Currently handled by another volunteer but the call's deadline it close (Within the RiskRange range)
    /// </summary>
    InProgressAndRisky,
    /// <summary>
    /// For edge scenarios
    /// </summary>
    Undefined
}

public enum VolunteerInListField
{
    Id,
    FullName,
    IsActive,
    TotalCallsDoneByVolunteer,
    TotalCallsCancelByVolunteer,
    TotalCallsExpiredByVolunteer,
    CallId,
    TypeOfCall
}

public enum TimeUnit
{
    Seconds,
    Minutes,
    Hours,
    Days,
    Weeks,
    Months,
    Years
}

public enum DistanceType { driving, walking, air }
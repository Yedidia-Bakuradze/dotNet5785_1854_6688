using Helpers;

namespace BO;
public enum UserRole { Admin, Volunteer, Undefined };
public enum TypeOfRange { AirDistance, DrivingDistance , WalkingDistance };
public enum TypeOfEndingCall { Treated, SelfCanceled, AdminCanceled, CancellationExpired, Undefined };
//Issue #20: Not enough CallTypes 
public enum CallType
{
    Undefined,              // Default value for undefined calls
    FlatTire,               // Assistance with a flat tire
    BatteryJumpStart,       // Jump-starting a vehicle with cables
    LockedOut,              // Unlocking a locked vehicle
    OutOfFuel,              // Delivering fuel to a stranded vehicle
    CarStuck,               // Rescuing a stuck vehicle (mud, snow, etc.)
    ChildLockedInCar,       // Rescuing a child locked in a vehicle
    TrafficDirection,       // Helping with traffic management
    TowingAssistance,       // Providing towing assistance
    MinorRoadRepair,        // Minor vehicle repairs (e.g., replacing a spare tire)
    SpecialAssistance       // Special help (e.g., assisting an elderly or disabled person)
}
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
    InProgressAndRisky
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
    Second = 1,
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Year
}

public enum DistanceType { driving, walking, air }
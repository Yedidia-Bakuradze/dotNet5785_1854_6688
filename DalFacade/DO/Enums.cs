namespace DO;

public enum UserRole  { Admin,Volunteer,Undefined };
public enum TypeOfRange { AirDistance, WalkingDistance, DrivingDistance};
public enum TypeOfEnding { Treated, SelfCanceled, AdminCanceled, CancellationExpired, Undefined };

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

public enum FileFormat { json,xml}
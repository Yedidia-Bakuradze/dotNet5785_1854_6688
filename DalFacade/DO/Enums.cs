namespace DO;

public enum UserRole  { Admin,Volunteer,Undefined };
public enum TypeOfRange { AirDistance, WalkingDistance, DrivingDistance};
public enum TypeOfEnding { Treated, SelfCanceled, AdminCanceled, CancellationExpired, Undefined };

//Issue #20: Not enough CallTypes 
public enum CallType {Undefined,FoodPreparation,FoodDelivery}

public enum FileFormat { json,xml}
namespace BO;
public enum VolunteerType { Admin, Volunteer, Undefined };
public enum TypeOfRange { AirDistance, WalkingDistance, DrivingDistance };
public enum TypeOfClosedCall { Treated, SelfCanceled, AdminCanceled, CancellationExpired, Undefined };
public enum CallType { None,Undefined, FoodPreparation, FoodDelivery }
public enum CallStatus { Taken, TakenAndInRisk }
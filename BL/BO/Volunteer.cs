﻿using Helpers;

namespace BO;

public class Volunteer
{
    //Get: From the DO object
    //init: Check and then add, after the set it wont be possible to change 
    public int Id { get; init; }

    //Get: From the DO object
    public string FullName { get; set; }

    //Get: From the DO object
    //Set: Check and then set
    public string PhoneNumber { get; set; }

    //Get: From the DO object
    //Set: Check and then set
    public string Email { get; set; }

    //Manager initializes with first pass
    //[Bounus]: Check if the pass is strong
    //[Bounus]: Eyncrpt the pass
    //Get: From the DO object
    //Set: Check logics
    public string? Password { get; set; }
    
    //Get: From the DO object
    //Set: Check if address exists and update the Latitude and Longitude as well (Phase 7: Make it async)
    public string? FullCurrentAddress { get; set; }

    //Get: From the DO object
    //Set: Would be updated and set according to the FullCurrentAddress field
    //Wont be shown, for calculating purposes only
    public double? Latitude { get; set; }

    //Get: From the DO object
    //Set: Would be updated and set according to the FullCurrentAddress field
    //Wont be shown, for calculating purposes only
    public double? Longitude { get; set; }

    //Get: From the DO object
    //Set: Check if the user is a manager
    public UserRole Role { get; set; }

    //A Guesss: 
    //Get: From the DO object
    //Set: Check if the modifier is a manager / the current user
    public bool IsActive { get; set; }

    //Get: From the DO object
    //Set: Check logics (Val > 0)
    public double? MaxDistanceToCall { get; set; }

    //For display only
    //[Bounus]: Calculation of the distance would be done according to this field value
    public TypeOfRange RangeType { get; set; }

    //Get: Find all the 'Treated' calls and sum them up
    //Af far as we understand the assignment - the call count calculatations are made in the runtime and now saved as a field
    public int NumOfHandledCalls {
        get => Helpers.AssignmentManager.GetNumOfHandledCallsByVolunteerId(this.Id, BO.TypeOfEndingCall.Treated);
    }

    //Get: Find all the 'SelfCanceled' calls and sum them up
    public int NumOfCanceledCalls {
        get => Helpers.AssignmentManager.GetNumOfHandledCallsByVolunteerId(this.Id,BO.TypeOfEndingCall.SelfCanceled);
    }

    //Get: Find all the 'CancellationExpired' calls and sum them up
    public int NumOfExpiredCalls{ 
        get=>Helpers.AssignmentManager.GetNumOfHandledCallsByVolunteerId(this.Id, BO.TypeOfEndingCall.CancellationExpired);
    }

    //Holds the call which is handled otherwise: null

    public BO.CallInProgress? CurrentCall { get; set; }
    public override string ToString() => this.ToStringProperty();


}

namespace BlImplementation;
using BlApi;
using BO;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    #region Observers Update Section
    /// <summary>
    /// Adds a new observer to the clock
    /// </summary>
    /// <param name="clockObserver">The action method which needs to be added</param>
    public void AddClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers += clockObserver;

    /// <summary>
    /// Adds a new observer to the config variables
    /// </summary>
    /// <param name="configObserver">The action method which needs to be added</param>
    public void AddConfigObserver(Action configObserver) => AdminManager.ClockUpdatedObservers += configObserver;

    /// <summary>
    /// Removes an observer from the clock
    /// </summary>
    /// <param name="clockObserver">The action method which needs to be removed</param>
    public void RemoveClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers -= clockObserver;

    /// <summary>
    /// Removes an observer from the config variables
    /// </summary>
    /// <param name="configObserver">The action method which needs to be removed</param>
    public void RemoveConfigObserver(Action configObserver) => AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion 


    /// <summary>
    /// This method initializes the system and the database from the BL to the Dal layer
    /// </summary>
    public void DbInit()
    {
        DalTest.Initialization.Do();
        VolunteerManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyListUpdated();
        //TODO: Should we? AssignmentManager.Observers.NotifyListUpdated();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange = AdminManager.RiskRange;
    }

    /// <summary>
    /// This method resets the system and the database from the BL to the Dal layer
    /// </summary>
    public void DbReset()//Stage 4
    {
        VolunteerManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyListUpdated();
        //TODO: Should we? AssignmentManager.Observers.NotifyListUpdated();
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    /// <summary>
    /// This method returns the current system clock of the BL layer
    /// </summary>
    /// <returns>System clock from BL</returns>
    public DateTime GetClock() => AdminManager.Now;

    /// <summary>
    /// This method returns the current system risk range value of the BL layer
    /// </summary>
    /// <returns>System risk range value from BL</returns>
    public TimeSpan GetRiskRange()
    {
        return Helpers.AdminManager.RiskRange;
    }
    /// <summary>
    /// This method resets the risk range value in the BL layer
    /// </summary>
    public void ResetRiskRange()
    {
        AdminManager.RiskRange = TimeSpan.Zero;
    }
    /// <summary>
    /// This method accepts a new risk range value and updates the current system risk range value in the BL layer with it
    /// </summary>
    /// <param name="range">The new risk range value</param>
    public void SetRiskRange(TimeSpan range)
    {
        AdminManager.RiskRange = range;
    }
    
    /// <summary>
    /// This method accepts a TimeUnit and moves forrward the time by one unit of the specified TimeUnit
    /// </summary>
    /// <param name="timeUnit">The TimeUnit which is needed to be add on the current clock by one unit</param>
    /// <exception cref="BO.BlInvalidOperationException">An excption that indicates that there is a forbidden operation</exception>
    public void UpdateClock(TimeUnit timeUnit)
    {
        switch (timeUnit)
        {
            case TimeUnit.Second:
                {
                    AdminManager.UpdateClock(AdminManager.Now.AddSeconds(1));
                    break;
                }
            case TimeUnit.Minute:
                {
                    AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));

                    break;
                }
            case TimeUnit.Hour:
                {
                    AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                    break;
                }
            case TimeUnit.Day:
                {
                    AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                    break;
                }
            case TimeUnit.Month:
                {
                    AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                    break;
                }
            case TimeUnit.Year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
            default:
                {
                    throw new BO.BlInvalidOperationException($"BL: System tries to update the BL clock with unknown type of TimeUnit");
                }

        }
    }

}

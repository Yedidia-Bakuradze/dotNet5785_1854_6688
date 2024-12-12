namespace BlImplementation;
using BlApi;
using BO;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// This method initializes the system and the database from the BL to the Dal layer
    /// </summary>
    public void DbInit()
    {
        //TODO: Find the place which we need to added the function call
        ClockManager.ClockUpdatedObservers += CallManager.UpdateAllOpenAndExpierdCalls;

        DalTest.Initialization.Do();
        ClockManager.UpdateRiskRange(ClockManager.RiskRange);
        ClockManager.UpdateClock(ClockManager.Now);
    }

    /// <summary>
    /// This method resets the system and the database from the BL to the Dal layer
    /// </summary>
    public void DbReset()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
        ClockManager.UpdateRiskRange(ClockManager.RiskRange);
    }

    /// <summary>
    /// This method returns the current system clock of the BL layer
    /// </summary>
    /// <returns>System clock from BL</returns>
    public DateTime GetClock()
    {
        return Helpers.ClockManager.Now;
    }

    /// <summary>
    /// This method returns the current system risk range value of the BL layer
    /// </summary>
    /// <returns>System risk range value from BL</returns>
    public TimeSpan GetRiskRange()
    {
        return Helpers.ClockManager.RiskRange;
    }

    /// <summary>
    /// This method accepts a new risk range value and updates the current system risk range value in the BL layer with it
    /// </summary>
    /// <param name="range">The new risk range value</param>
    public void SetRiskRange(TimeSpan range)
    {
        ClockManager.UpdateRiskRange(range);
    }
    
    /// <summary>
    /// This method accepts a TimeUnit and moves forrward the time by one unit of the specified TimeUnit
    /// </summary>
    /// <param name="timeUnit">The TimeUnit which is needed to be add on the current clock by one unit</param>
    /// <exception cref="BO.BlInvalidEnumValueOperationException">An excption that indicates that there is a forbidden operation</exception>
    public void UpdateClock(TimeUnit timeUnit)
    {
        switch (timeUnit)
        {
            case TimeUnit.Seconds:
                {
                    ClockManager.UpdateClock(ClockManager.Now.AddSeconds(1));
                    break;
                }
            case TimeUnit.Minutes:
                {
                    ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));

                    break;
                }
            case TimeUnit.Hours:
                {
                    ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                    break;
                }
            case TimeUnit.Days:
                {
                    ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                    break;
                }
            case TimeUnit.Months:
                {
                    ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
                    break;
                }
            default:
                {
                    throw new BO.BlInvalidEnumValueOperationException($"BL: System tries to update the BL clock with unknown type of TimeUnit");
                }

        }
    }

}

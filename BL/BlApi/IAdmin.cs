
namespace BlApi;

public interface IAdmin
{

    /// <summary>
    /// Returns the current system clock
    /// </summary>
    /// <returns>System Clock of DateTime</returns>
    DateTime GetClock();

    /// <summary>
    /// The methods adds to the current clock a single value of the past type of time unit
    /// The method will create a new instance of DateTime with the updated valuea and would call the ClockManager's UpdateClock method
    /// </summary>
    /// <param name="timeUnit"></param>
    void UpdateClock(BO.TimeUnit timeUnit);

    /// <summary>
    /// This method returns the risk range value which has been set in our system
    /// </summary>
    /// <returns>The risk range</returns>
    TimeSpan GetRiskRange();

    /// <summary>
    /// Updates the current RiskRange value with the new past range value
    /// </summary>
    /// <xparam name="range">The new range value</param>
    void SetRiskRange(TimeSpan range);

    /// <summary>
    /// This method resets the entier system, including deleteing all the value from the database
    /// and resets the config properties such as clock and riskrange to its default values
    /// </summary>
    void DbReset();

    /// <summary>
    /// This method initializes the databse with pre-set entities
    /// </summary>
    void DbInit();

}

using BO;
using DalApi;

namespace Helpers;

//Remember: All the method shall be as internal static
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// This method returns the number of handled calls for the given volunteer by his Id and the given parameter of type of ending
    /// </summary>
    /// <param name="id">The volunteer's id</param>
    /// <param name="typeOfEndingCall">The type of ending the call</param>
    /// <returns>an integer representing the count of calls that are satisfing the request</returns>
    internal static int GetNumOfHandledCallsByVolunteerId(int id, BO.TypeOfEndingCall typeOfEndingCall)
        => s_dal.Assignment
            .ReadAll((DO.Assignment assignment) => assignment.VolunteerId == id && assignment.TypeOfEnding == (DO.TypeOfEnding)typeOfEndingCall)
            .Count();
        
}

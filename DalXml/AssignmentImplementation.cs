using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
    }

    public void Delete(int id)
    {
    }

    public void DeleteAll()
    {
    }

    public Assignment? Read(int id)
    {
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
    }

    public void Update(Assignment item)
    {
    }
    
    /// <summary>
    /// Translates all the XML values into a single Assignment variable
    /// </summary>
    /// <param name="el">The XElement tree which contains the values for this entity</param>
    /// <returns>The entity contaning all the values from the given tree</returns>
    /// <exception cref="Exception">If the important fields are missing / cann't be converted, the system will throw an exception</exception>
    static Assignment getAssignment(XElement el)
    {
        return new Assignment
        {
            //TODO: Chaneg the exceptions to a dal version
            Id = el.ToIntNullable("Id") ?? throw new Exception("Id cann't be converted"),
            Called = el.ToIntNullable("Called") ?? throw new Exception("CallId cann't be converted"),
            VolunteerId = el.ToIntNullable("VolunteerId") ?? throw new Exception("CallId cann't be converted"),
            TimeOfStarting = el.ToDateTimeNullable("TimeOfStarting") ?? throw new Exception("TimOfStarting cann't be converted"),
            TimeOfEnding = el.ToDateTimeNullable("TimeOfEnding") ?? null,
            TypeOfEnding = el.ToEnumNullable<TypeOfEnding>("TypeOfEnding") ?? null,
        };
    }
}

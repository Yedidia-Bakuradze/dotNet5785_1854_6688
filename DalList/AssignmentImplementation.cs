
namespace Dal;
using DalApi;
using DO;

public class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Adds a new object to the database (The list version)
    /// </summary>
    /// <param name="item">The requested object to add</param>
    /// <exception cref="Exception">Throws an exception if the object already exists in the database</exception>
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment newItem = new Assignment() {
            Id = id,
            Called = item.Called,
            VolunteerId = item.VolunteerId,
            TimeOfStarting = item.TimeOfStarting,
            TimeOfEnding = item.TimeOfEnding,
            TypeOfEnding  = item.TypeOfEnding,
        };

        if (DalList.Assignments.Any((a) => a.Id == id))
        {
            throw new Exception("Object already exists");
        }
        else {
            DalList.Assignments.Add(newItem);
        }
        
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Assignment? Read(int id)
    {
        throw new NotImplementedException();
    }

    public List<Assignment> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Assignment item)
    {
        throw new NotImplementedException();
    }
}


namespace Dal;
using DalApi;
using DO;
using System.Collections;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Adds a new object to the database (The list version)
    /// </summary>
    /// <param name="item">The requested object to add</param>
    /// <exception cref="Exception">Throws an exception if the object already exists in the database</exception>
    public void Create(Assignment item)
    {
        //Since the id is automatically generated, there is not need for checking whether assignment with such id value exists
        int id = Config.NextAssignmentId;
        DataSource.Assignments.Add(item with { Id = id});
    }

    /// <summary>
    /// Accepts an id value and deletes the related assignment with the same id value
    /// If such an assignment with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="id">The requested assignment's id value</param>
    public void Delete(int id)
    {
        Assignment result = DataSource.Assignments.Find((assignment) => assignment.Id == id)
            ?? throw new Exception($"Object of type Assignment with id of {id} hasn't been found");
        DataSource.Assignments.Remove(result);
    }

    /// <summary>
    /// Deletes all the assignment in the database
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Returns a reference to an individual assignment object which contains the requested id value
    /// If such an assignment with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="id">Requested id value for the assignment</param>
    /// <returns></returns>
    public Assignment? Read(int id)
    {
        try
        {
            Assignment res = DataSource.Assignments.FirstOrDefault((assignment) => assignment.Id == id)
                ?? throw new Exception($"Object of type Assignment with id of {id} hasn't been found");
            return res;
        }
        catch(Exception error)
        {
            Console.WriteLine(error.Message);
            return null;
        }
    }

    /// <summary>
    /// Accepts an optional filter and returns a new enumerable stack of Assignment entities which satisfy the filter's condition
    /// If filter hasn't been past, the method will return an enumerable of all the assignments
    /// </summary>
    /// <param name="filter">A filter which returns a boolean value whether the past Assignment value satisfies the logical condition</param>
    /// <returns></returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter != null
            ? from assignment in DataSource.Assignments
              where filter(assignment)
              select assignment
             : from assignment in DataSource.Assignments
               select assignment;
    }
    /// <summary>
    /// Accepts a new assignment which would replace the old assignment with the same id value
    /// If such an assignment with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="item">The new assignment record</param>
    public void Update(Assignment item)
    {
        Assignment? res = Read(item.Id) ??
            throw new Exception($"Object of type Assignment with id of {item.Id} hasn't been found");
        Delete(res.Id);
        DataSource.Assignments.Add(item);
    }
}

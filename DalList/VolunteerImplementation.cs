
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new object to the database (The list version)
    /// </summary>
    /// <param name="newVolunteer">The requested object to add</param>
    /// <exception cref="Exception">Throws an exception if the object already exists in the database</exception>
    public void Create(Volunteer newVolunteer)
    {
        //Since the id is automatically generated, there is not need for checking whether volunteer with such id value exists
        if(DataSource.Volunteers.Any((volunteer) => volunteer.Id == newVolunteer.Id))
        {
            throw new Exception($"Volunteer object with id {newVolunteer.Id} already exists");
        }
        else
        {
            DataSource.Volunteers.Add(newVolunteer);
            //TODO: return item.Id; In the documentation it been written to return the new value of the id
        }
    }

    /// <summary>
    /// Accepts an id value and deletes the related volunteer with the same id value
    /// If such an volunteer with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="id">The requested volunteer's id value</param>
    public void Delete(int id)
    {
        Volunteer result = DataSource.Volunteers.Find((volunteer) => volunteer.Id == id)
            ?? throw new Exception($"Object of type Volunteer with id of {id} hasn't been found");
        DataSource.Volunteers.Remove(result);
    }

    /// <summary>
    /// Deletes all the volunteer in the database
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Returns a reference to an individual volunteer object which contains the requested id value
    /// If such an volunteer with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="id">Requested id value for the volunteer</param>
    /// <returns></returns>
    public Volunteer? Read(int id)
    {
        try
        {
            Volunteer res = DataSource.Volunteers.Find((volunteer) => volunteer.Id == id)
                ?? throw new Exception($"Object of type Volunteer with id of {id} hasn't been found");
            return res;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return null;
        }
    }

    /// <summary>
    /// Returns a copy of all the volunteers in the database
    /// </summary>
    /// <returns>A copied list of all the volunteers in the database</returns>
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    /// <summary>
    /// Accepts a new volunteer which would replace the old volunteer with the same id value
    /// If such an volunteer with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="newVolunteer">The new volunteer record</param>
    public void Update(Volunteer newVolunteer)
    {
        Volunteer? res = Read(newVolunteer.Id) ??
            throw new Exception($"Object of type Volunteer with id of {newVolunteer.Id} hasn't been found");
        Delete(res.Id);
        DataSource.Volunteers.Add(newVolunteer);
    }
}


namespace Dal;
using DalApi;
using DO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new object to the database (The list version)
    /// </summary>
    /// <param name="newVolunteer">The requested object to add</param>
    /// <exception cref="Exception">Throws an exception if the object already exists in the database</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer newVolunteer)
    {
        //Since the id is automatically generated, there is not need for checking whether volunteer with such id value exists
        if(DataSource.Volunteers.Any((volunteer) => volunteer.Id == newVolunteer.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer entity with Id of {newVolunteer.Id} already exists");
        }
        else
        {
            DataSource.Volunteers.Add(newVolunteer);
        }
    }

    /// <summary>
    /// Accepts an id value and deletes the related volunteer with the same id value
    /// If such an volunteer with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="id">The requested volunteer's id value</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer result = DataSource.Volunteers.Find((volunteer) => volunteer.Id == id)
            ?? throw new DalDoesNotExistException($"Volunteer entity with Id of {id} hasn't been found");
        DataSource.Volunteers.Remove(result);
    }

    /// <summary>
    /// Deletes all the volunteer in the database
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
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
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        try
        {
            Volunteer res = DataSource.Volunteers.FirstOrDefault((volunteer) => volunteer.Id == id)
            ?? throw new DalDoesNotExistException($"Volunteer entity with Id of {id} hasn't been found");

            return res;
        }
        catch (DalDoesNotExistException error)
        {
            Console.WriteLine(error.Message);
            return null;
        }
    }

    /// <summary>
    /// Returns a reference to an individual volunteer object which satisfies the past filter function
    /// If such an volunteer which satisfies the filter function hasn't been found - the method will throw and exception and would print out the proper message and return a null value
    /// </summary>
    /// <param name="filter">A condition function which the entity shall satisfy its condition</param>
    /// <returns>Entity which satisfies the filter function or a null value if no such entity has been found</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        try
        {
            Volunteer res = DataSource.Volunteers.FirstOrDefault((volunteer) => filter(volunteer))
                ?? throw new DalDoesNotExistException("No Volunteer entity which satisfies the given condition has been found");
            return res;
        }
        catch (DalDoesNotExistException error)
        {
            return null;
        }
    }

    /// <summary>
    /// Accepts an optional filter and returns a new enumerable stack of Volunteer entities which satisfy the filter's condition
    /// If filter hasn't been past, the method will return an enumerable of all the volunteers
    /// </summary>
    /// <param name="filter">A filter which returns a boolean value whether the past Volunteer value satisfies the logical condition</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null
            ? from volunteer in DataSource.Volunteers
              select volunteer
              : from volunteer in DataSource.Volunteers
                where filter(volunteer)
                select volunteer;
    }

    /// <summary>
    /// Accepts a new volunteer which would replace the old volunteer with the same id value
    /// If such an volunteer with the proper id hasn't been found - the method will throw and exception and would print out the proper message
    /// </summary>
    /// <param name="newVolunteer">The new volunteer record</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer newVolunteer)
    {
        Volunteer? res = Read(newVolunteer.Id) 
            ?? throw new DalDoesNotExistException($"Volunteer entity with Id of {newVolunteer.Id} hasn't been found");

        Delete(res.Id);
        DataSource.Volunteers.Add(newVolunteer);
    }
}

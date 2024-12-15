
using DalApi;
using DO;
namespace Dal;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds the new volunteer to the XML file using XMLSerializer
    /// </summary>
    /// <param name="newVolunteer">The new volunteer to be added</param>
    /// <exception cref="DalAlreadyExistsException">an exception will be thrown if the volunteer already exists in the database</exception>
    public void Create(Volunteer newVolunteer)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.volunteerFileName);
        if(volunteers.Any(v => v.Id == newVolunteer.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer with ID of: {newVolunteer.Id} already exists.");
        }
        volunteers.Add(newVolunteer);

        XMLTools.SaveListToXMLSerializer(volunteers, Config.volunteerFileName);
    }

    /// <summary>
    /// Accepts an id of a volunteer and removes it from the XML database file
    /// if the volunteer doesn't exist the method will throw an exception with a proper message.
    /// </summary>
    /// <param name="id">The id of the volunteer to be removed</param>
    /// <exception cref="DalDoesNotExistException">an Exception will be thrown if the volunteer doesn't exists</exception>
    public void Delete(int id)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.volunteerFileName);
        Volunteer res = volunteers.Find(v => v.Id == id)
            ?? throw new DalDoesNotExistException($"Volunteer with ID of: {id} doesn't exists");
        volunteers.Remove(res);

        XMLTools.SaveListToXMLSerializer<Volunteer>(volunteers, Config.volunteerFileName);
    }

    /// <summary>
    /// This method will remove all the Volunteer entities from the XML file database 
    /// </summary>
    public void DeleteAll()
    {
        List<Volunteer> volunteers = new ();
        XMLTools.SaveListToXMLSerializer<Volunteer>(volunteers,Config.volunteerFileName);
    }


    /// <summary>
    /// Accepts an id value of the volunteer and returns it from the XML file database
    /// if no entity has the past id value, the method will display a message and return a null
    /// </summary>
    /// <param name="id">The requsted id value</param>
    /// <returns>an entity with the value of the past id / if no entity has been found the method will return a null</returns>
    public Volunteer? Read(int id)
    {
        try
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.volunteerFileName);
            Volunteer res = volunteers.Find(v => v.Id == id)
                ?? throw new DalDoesNotExistException($"Volunteer with ID of: {id} doesn't exists");
            return res;
        }catch(Exception error)
        {
            Console.WriteLine(error.Message);
        }
        return null;
    }
    
    /// <summary>
    /// Accepts a filter and returns the first entity from the XML file database which satisfies the past condition
    /// </summary>
    /// <param name="filter">The condition for the returned entity</param>
    /// <returns>an entity which satisfies the past condition</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        try
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.volunteerFileName);

            Volunteer res = volunteers.FirstOrDefault((volunteer) => filter(volunteer))
                ?? throw new DalDoesNotExistException("No Volunteer entity which satisfies the given condition has been found");
            return res;
        }
        catch (DalDoesNotExistException error)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns an enumarable of all the Volunteer entities which the XML file databse holds
    /// additionaly, if a filter function is past the method will return an enumable which contains all the entities which are satisfing the filter condition.
    /// </summary>
    /// <param name="filter">[Optional] a function which selectes the returned entities</param>
    /// <returns>an enumerable of all the entities in the XML file databse / which are satisfing the past filter function</returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.volunteerFileName);
        return volunteers;
    }

    /// <summary>
    /// Accepts a new Volunteer entitiy and removes from the XML file databsae
    /// the old one who has the same key (Id) and adds the new entity to the end of the XML file
    /// </summary>
    /// <param name="newVolunteer">The new Volunteer entity</param>
    public void Update(Volunteer newVolunteer)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.volunteerFileName);
        Volunteer res = Read(newVolunteer.Id)
            ?? throw new DalDoesNotExistException($"Volunteer entity with Id of {newVolunteer.Id} hasn't been found");

        volunteers.Remove(res);
        volunteers.Add(newVolunteer);

        XMLTools.SaveListToXMLSerializer<Volunteer>(volunteers, Config.volunteerFileName);
    }
}

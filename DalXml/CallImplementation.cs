
using DalApi;
using DO;
namespace Dal;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Adds the new Call to the XML file using XMLSerializer
    /// </summary>
    /// <param name="newCall">The new Call to be added</param>
    /// <exception cref="DalAlreadyExistsException">an exception will be thrown if the Call already exists in the database</exception>
    public void Create(Call newCall)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.callFileName);
        calls.Add(newCall with { Id = Config.NextCallId});

        XMLTools.SaveListToXMLSerializer<Call>(calls, Config.callFileName);
    }

    /// <summary>
    /// Accepts an id of a call and removes it from the XML database file
    /// if the Call doesn't exist the method will throw an exception with a proper message.
    /// </summary>
    /// <param name="id">The id of the Call to be removed</param>
    /// <exception cref="DalDoesNotExistException">an Exception will be thrown if the Call doesn't exists</exception>
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.callFileName);
        Call res = Calls.Find(v => v.Id == id)
            ?? throw new DalDoesNotExistException($"Call with ID of: {id} doesn't exists");
        Calls.Remove(res);
        XMLTools.SaveListToXMLSerializer<Call>(Calls, Config.callFileName);
    }

    /// <summary>
    /// This method will remove all the Call entities from the XML file database 
    /// </summary>
    public void DeleteAll()
    {
        List<Call> Calls = new();
        XMLTools.SaveListToXMLSerializer<Call>(Calls, Config.callFileName);
    }

    /// <summary>
    /// Accepts an id value of the Call and returns it from the XML file database
    /// if no entity has the past id value, the method will display a message and return a null
    /// </summary>
    /// <param name="id">The requsted id value</param>
    /// <returns>an entity with the value of the past id / if no entity has been found the method will return a null</returns>
    public Call? Read(int id)
    {
        try
        {
            List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.callFileName);
            Call res = Calls.Find(call => call.Id == id)
                ?? throw new DalDoesNotExistException($"Call with ID of: {id} doesn't exists");
            return res;
        }
        catch (Exception error)
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
    public Call? Read(Func<Call, bool> filter)
    {
        try
        {
            List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.callFileName);

            Call res = calls.FirstOrDefault((Call) => filter(Call))
                ?? throw new DalDoesNotExistException("No Call entity which satisfies the given condition has been found");
            return res;
        }
        catch (DalDoesNotExistException error)
        {
            Console.WriteLine(error.Message);
            return null;
        }
    }

    /// <summary>
    /// Returns an enumarable of all the Call entities which the XML file databse holds
    /// additionaly, if a filter function is past the method will return an enumable which contains all the entities which are satisfing the filter condition.
    /// </summary>
    /// <param name="filter">[Optional] a function which selectes the returned entities</param>
    /// <returns>an enumerable of all the entities in the XML file databse / which are satisfing the past filter function</returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.callFileName);
        return Calls;
    }

    /// <summary>
    /// Accepts a new Call entitiy and removes from the XML file databsae
    /// the old one who has the same key (Id) and adds the new entity to the end of the XML file
    /// </summary>
    /// <param name="newCall">The new Call entity</param>
    public void Update(Call newCall)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.callFileName);
        Call? res = Read(newCall.Id)
            ?? throw new DalDoesNotExistException($"Call entity with Id of {newCall.Id} hasn't been found");
        Delete(res.Id);
        Calls.Add(newCall);

        XMLTools.SaveListToXMLSerializer<Call>(Calls, Config.callFileName);
    }
}

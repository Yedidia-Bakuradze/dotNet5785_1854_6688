namespace Dal;
using DalApi;
using DO;
using System.Collections;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new Call item.
    /// </summary>
    /// <param name="item">The Call item to create.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        //Since the id is automatically generated, there is not need for checking whether assignment with such id value exists
        int id = Config.NextCallId;
        DataSource.Calls.Add(item with { Id = id });
        //TODO: return id; In the documentation it been written to return the new value of the id

    }

    /// <summary>
    /// Deletes a Call item by its id.
    /// </summary>
    /// <param name="id">The id of the Call item to delete.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        //If the item does not exist, an exception will be thrown
        Call result = DataSource.Calls.Find((call) => call.Id == id)
        ?? throw new DalDoesNotExistException($"Call entity with Id of {id} hasn't been found");
            DataSource.Calls.Remove(result);//If the item exists, it will be removed
    }

    /// <summary>
    /// Deletes all Call items.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Reads a Call item by its id.
    /// </summary>
    /// <param name="id">The id of the Call item to read.</param>
    /// <returns>The Call item with the specified id, or null if it does not exist.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        try
        {
            // If the item does not exist, an exception will be thrown
            Call res = DataSource.Calls.FirstOrDefault((call) => call.Id == id)
                ?? throw new DalDoesNotExistException($"Call entity with Id of {id} hasn't been found");
            return res;
        }
        catch (DalDoesNotExistException errorMsg)
        {
            Console.WriteLine(errorMsg.Message);
            return null;
        }
    }


    /// <summary>
    /// Returns a reference to an individual call  object which satisfies the past filter function
    /// If such an call which satisfies the filter function hasn't been found - the method will throw and exception and would print out the proper message and return a null value
    /// </summary>
    /// <param name="filter">A condition function which the entity shall satisfy its condition</param>
    /// <returns>Entity which satisfies the filter function or a null value if no such entity has been found</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        try
        {
            Call res = DataSource.Calls.FirstOrDefault((call) => filter(call))
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
    /// Accepts an optional filter and returns a new enumerable stack of Call entities which satisfy the filter's condition
    /// If filter hasn't been past, the method will return an enumerable of all the calls
    /// </summary>
    /// <param name="filter">A filter which returns a boolean value whether the past Call value satisfies the logical condition</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter == null
            ? from call in DataSource.Calls
              select call
              : from call in DataSource.Calls
                where filter(call)
                select call;
    }


    /// <summary>
    /// Updates a Call item.
    /// </summary>
    /// <param name="item">The Call item to update.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
            //  If the item does not exist, an exception will be thrown
            Call result = DataSource.Calls.Find((call) => call.Id == item.Id)
            ?? throw new DalDoesNotExistException($"Call entity with Id of {item.Id} hasn't been found");
            Delete(result.Id);//If the item exists, it will be removed
            DataSource.Calls.Add(item);//The updated item will be added
    }
}




namespace Dal;
using DalApi;
using DO;
public class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new Call item.
    /// </summary>
    /// <param name="item">The Call item to create.</param>
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
    public void Delete(int id)
    {
            //If the item does not exist, an exception will be thrown
            Call result = DataSource.Calls.Find((call) => call.Id == id) ?? throw new Exception($"no such Call with id:{id}");
            DataSource.Calls.Remove(result);//If the item exists, it will be removed
    }

    /// <summary>
    /// Deletes all Call items.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Reads a Call item by its id.
    /// </summary>
    /// <param name="id">The id of the Call item to read.</param>
    /// <returns>The Call item with the specified id, or null if it does not exist.</returns>
    public Call? Read(int id)
    {
        try
        {
            // If the item does not exist, an exception will be thrown
            Call res = DataSource.Calls.Find((call) => call.Id == id) ?? throw new Exception($"no such Call with id:{id}");
            return res;
        }
        catch (Exception errorMsg)
        {
            Console.WriteLine(errorMsg.Message);
            return null;
        }
    }

    /// <summary>
    /// Reads all Call items.
    /// </summary>
    /// <returns>A list of all Call items.</returns>
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }


    /// <summary>
    /// Updates a Call item.
    /// </summary>
    /// <param name="item">The Call item to update.</param>
    public void Update(Call item)
    {
            //  If the item does not exist, an exception will be thrown
            Call result = DataSource.Calls.Find((call) => call.Id == item.Id) ?? throw new Exception($"no such Call with id:{item.Id}");
            Delete(result.Id);//If the item exists, it will be removed
            DataSource.Calls.Add(item);//The updated item will be added
    }
}



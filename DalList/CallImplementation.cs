
namespace Dal;
using DalApi;
using DO;
public class CallImplementation : ICall
{
    // Creates a new Call item
    public void Create(Call item)
    {
        //Since the id is automatically generated, there is not need for checking whether assignment with such id value exists
        int id = Config.NextCallId;
        DalList.Calls.Add(item with { Id = id });
        //TODO: return id; In the documentation it been written to return the new value of the id
    
    }

    // Deletes a Call item by its id
    public void Delete(int id)
    {
        try
        {
            //If the item does not exist, an exception will be thrown
            Call result = DalList.Calls.Find((call) => call.Id == id) ?? throw new Exception($"no such Call with id:{id}");
            DalList.Calls.Remove(result);//If the item exists, it will be removed
        }
        catch (Exception errorMsg)
        {
            Console.WriteLine(errorMsg.Message);
        }
    }

    // Deletes all Call items
    public void DeleteAll()
    {
        DalList.Calls.Clear();
    }

    // Reads a Call item by its id
    public Call? Read(int id)
    {
        try
        {
            // If the item does not exist, an exception will be thrown
            Call res = DalList.Calls.Find((call) => call.Id == id) ?? throw new Exception($"no such Call with id:{id}");
            return res;
        }
        catch (Exception errorMsg)
        {
            Console.WriteLine(errorMsg.Message);
            return null;
        }
    }

    // Reads all Call items
    public List<Call> ReadAll()
    {
        return new List<Call>(DalList.Calls);
    }

    // Updates a Call item
    public void Update(Call item)
    {
        try
        {
            //  If the item does not exist, an exception will be thrown
            Call result = DalList.Calls.Find((call) => call.Id == item.Id) ?? throw new Exception($"no such Call with id:{item.Id}");
            Delete(result.Id);//If the item exists, it will be removed
            DalList.Calls.Add(item);//The updated item will be added
        }
        catch (Exception errorMsg)
        {
            Console.WriteLine(errorMsg.Message);
        }
    }
}



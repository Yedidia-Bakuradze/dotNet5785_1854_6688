
namespace Dal;
using DalApi;
using DO;
public class CallImplementation : ICall
{
    // Creates a new Call item
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        DalList.Calls.Add(item with { Id = id });
    }

    // Deletes a Call item by its id
    public void Delete(int id)
    {
        try
        {
            Call result = DalList.Calls.Find((call) => call.Id == id) ?? throw new Exception($"no such Call with id:{id}");
            DalList.Calls.Remove(result);
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
            Call result = DalList.Calls.Find((call) => call.Id == item.Id) ?? throw new Exception($"no such Call with id:{item.Id}");
            Delete(result.Id);
            DalList.Calls.Add(item);
        }
        catch (Exception errorMsg)
        {
            Console.WriteLine(errorMsg.Message);
        }
    }
}



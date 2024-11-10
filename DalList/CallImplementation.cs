
namespace Dal;
using DalApi;
using DO;
public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        DalList.Calls.Add(item with { Id = id });
    }

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

    public void DeleteAll()
    {
        DalList.Calls.Clear();
    }

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

    public List<Call> ReadAll()
    {
        return new List<Call>(DalList.Calls);
    }

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



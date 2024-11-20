namespace DalApi;

public interface ICrud<T> where T : class
{
    public void Create(T item); //Creates new entity object in DAL
    public T? Read(int id); //Reads entity object by its ID 
    public List<T> ReadAll(); //stage 1 only, Reads all entity objects
    public void Update(T item); //Updates entity object
    public void Delete(int id); //Deletes an object by its Id
    public void DeleteAll(); //Delete all entity objects
}

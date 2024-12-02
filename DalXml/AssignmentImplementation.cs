using DalApi;
using DO;
using System.Xml.Linq;
namespace Dal;

public class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Adds a new Assignment entity in the XML database file
    /// </summary>
    /// <param name="newAssignment">The new Assignment instance</param>
    public void Create(Assignment newAssignment)
    {
        XElement data = XMLTools.LoadListFromXMLElement(Config.assignmentFileName);
        XElement newAssigment = getXMLAssignmentValue(newAssignment with { Id = Config.NextAssignmentId});
        data.Add(newAssigment);
        XMLTools.SaveListToXMLElement(data, Config.assignmentFileName);
    }

    /// <summary>
    /// Accepts the id of the required assignment to be removed and the method removes it from the XML tree
    /// If such entity hasn't been found, the method will throw an exception.
    /// </summary>
    /// <param name="id">The Assignment entity id value</param>
    /// <exception cref="DalDoesNotExistException">If entity hasn't been found the method will throw an exception</exception>
    public void Delete(int id)
    {
        XElement data = XMLTools.LoadListFromXMLElement(Config.assignmentFileName);
        XElement removeTag = data.Elements().FirstOrDefault(assignment => (int?)assignment.Element("Id") == id)
            ?? throw new DalDoesNotExistException($"Assingment with ID of: {id} doesn't exist");

        removeTag.Remove();
        XMLTools.SaveListToXMLElement(data, Config.assignmentFileName);
    }

    /// <summary>
    /// Removes all the elements from the XML tree
    /// </summary>
    public void DeleteAll()
    {
        XElement data = XMLTools.LoadListFromXMLElement(Config.assignmentFileName);
        data.RemoveAll();
        XMLTools.SaveListToXMLElement(data, Config.assignmentFileName);
    }

    /// <summary>
    /// Accepts an Assignment id and returns an Assingment instance containing the values in the assosiated XML tag
    /// If such instance hasn't been found, the method will throw an exception
    /// </summary>
    /// <param name="id">Assignment id</param>
    /// <returns>Assignment instance with the proper values</returns>
    /// <exception cref="DalDoesNotExistException">An exception will be thrown if such entity hasn't been found</exception>
    public Assignment? Read(int id)
    {
        XElement data = XMLTools.LoadListFromXMLElement(Config.assignmentFileName);
        XElement res = data.Elements().FirstOrDefault(tag => (int?)tag.Element("Id") == id)
            ?? throw new DalDoesNotExistException($"Assignment with ID of: {id} doesn't exist");

        return getAssignment(res);
    }

    /// <summary>
    /// Accepts a filter (a boolean function) and returns the first Assingment instance from the XML file which meets the condition of the filter
    /// If such instance hasn't been found, the method will throw an exception
    /// </summary>
    /// <param name="filter">a boolean funciton to validate the return Assignment instance</param>
    /// <returns>Assignment instance which meets the conditions of the filter</returns>
    /// <exception cref="DalDoesNotExistException">An exception will be thrown if such entity hasn't been found</exception>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        XElement data = XMLTools.LoadListFromXMLElement(Config.assignmentFileName);
        XElement res = data.Elements().FirstOrDefault(a => filter(getAssignment(a)))
            ?? throw new DalDoesNotExistException("No assignment which satisfies the given filter has been found");
        return getAssignment(res);
    }

    /// <summary>
    /// Returns an enumarable of all the Assignment entities which are satisfing the filter function condition
    /// Alternatively, if the filter function hasn't been past, the method will return a enumerable set of all Assignment entities
    /// </summary>
    /// <param name="filter">[Optional] if past, a boolean function requires all the entities to meet its condition in order to return back from the method</param>
    /// <returns>a set of all entities / a set of entities which satisfy the given filter function</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        IEnumerable<XElement> assignments = XMLTools.LoadListFromXMLElement(Config.assignmentFileName).Elements();
        IEnumerable<Assignment> res = filter == null
                                      ? from assignmet in assignments
                                        select getAssignment(assignmet)
                                      : from a in assignments
                                        where filter(getAssignment(a))
                                        select getAssignment(a);
        return res;
    }

    /// <summary>
    /// Accepts a new Assignment instance and replaces the old with the new instance in the XML file
    /// If such entity doesn't exist in the XML file, the method will throw an exception
    /// </summary>
    /// <param name="newAssignment">a new Assignmet instance</param>
    /// <exception cref="DalDoesNotExistException">an exception thrown when the entity hasn't been found</exception>
    public void Update(Assignment newAssignment)
    {
        XElement data = XMLTools.LoadListFromXMLElement(Config.assignmentFileName);
        XElement res = data.Elements().FirstOrDefault(a => (int?)a.Element("Id") == newAssignment.Id)
            ?? throw new DalDoesNotExistException($"Assignment with ID of: {newAssignment.Id} doesn't exists");
        res.Remove();
        data.Add(getXMLAssignmentValue(newAssignment));
        XMLTools.SaveListToXMLElement(data, Config.assignmentFileName);
    }

    /// <summary>
    /// Translates all the XML values into a single Assignment variable
    /// </summary>
    /// <param name="el">The XElement tree which contains the values for this entity</param>
    /// <returns>The entity contaning all the values from the given tree</returns>
    /// <exception cref="Exception">If the important fields are missing / cann't be converted, the system will throw an exception</exception>
    static Assignment getAssignment(XElement el)
    {
        return new Assignment
        {
            //TODO: Chaneg the exceptions to a dal version
            Id = el.ToIntNullable("Id") ?? throw new Exception("Id cann't be converted"),
            Called = el.ToIntNullable("Called") ?? throw new Exception("CallId cann't be converted"),
            VolunteerId = el.ToIntNullable("VolunteerId") ?? throw new Exception("CallId cann't be converted"),
            TimeOfStarting = el.ToDateTimeNullable("TimeOfStarting") ?? throw new Exception("TimOfStarting cann't be converted"),
            TimeOfEnding = el.ToDateTimeNullable("TimeOfEnding") ?? null,
            TypeOfEnding = el.ToEnumNullable<TypeOfEnding>("TypeOfEnding") ?? null,
        };
    }

    /// <summary>
    /// Returns an XML tag which contains all the fields of Assignment entity
    /// </summary>
    /// <param name="assignment">The source assignment</param>
    /// <returns>XML Tag which contains all the entity's values</returns>
    static XElement getXMLAssignmentValue(Assignment assignment)
    {
        List<XElement> subTags = new List<XElement>() {
                new XElement("Id", assignment.Id),
                new XElement("Called", assignment.Called),
                new XElement("VolunteerId", assignment.VolunteerId),
                new XElement("TimeOfStarting", assignment.TimeOfStarting)
               };

        if(assignment.TimeOfEnding != null)
        {
            subTags.Add(new XElement("TimeOfEnding", assignment.TimeOfEnding.ToString()));
        }
        if(assignment.TypeOfEnding != null)
        {
            subTags.Add(new XElement("TypeOfEnding", assignment.TypeOfEnding.ToString()));
        }

        return new ("Assignment", subTags.ToArray());
    }
}

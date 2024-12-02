namespace Dal;

using DO;
using System.Xml.Linq;
using System.Xml.Serialization;

static class XMLTools
{
    const string s_xmlDir = @"..\xml\";
    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }

    #region SaveLoadWithXMLSerializer
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            new XmlSerializer(typeof(List<T>)).Serialize(file, list);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (!File.Exists(xmlFilePath)) return new();
            using FileStream file = new(xmlFilePath, FileMode.Open);
            XmlSerializer x = new(typeof(List<T>));
            return x.Deserialize(file) as List<T> ?? new();
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    #region SaveLoadWithXElement
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    #region XmlConfig
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        root.Element(elemName)?.SetValue((nextId + 1).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
        return nextId;
    }
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return num;
    }
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }
    
    /// <summary>
    /// A getter to get the Risk Range value from the XML file where its located
    /// </summary>
    /// <param name="xmlFileName">The XML file name where its located</param>
    /// <param name="elementName">The XML tag name where the value is located</param>
    /// <returns>The risk range value which is set to be in the project</returns>
    /// <exception cref="FormatException"></exception>
    public static TimeSpan GetConfigTimeSpan(string xmlFileName, string elementName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        TimeSpan res = root.ToTimeSpanNullable(elementName)
            ?? throw new FormatException($"can't convert:  {xmlFileName}, {elementName}");
        return res;
    }
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    /// <summary>
    /// A setter for the Max Range property which takes the TimeSpan value and sets it in the related XML tag in the tree
    /// </summary>
    /// <param name="xmlFileName">XML file name</param>
    /// <param name="elementName">Target XML element's name which would be modified</param>
    /// <param name="newValue">The replacement value</param>
    public static void SetConfigTimeSpan(string xmlFileName, string elementName, TimeSpan newValue)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elementName)?.SetValue((newValue).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    #endregion


    #region ExtensionFuctions
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;
    
    /// <summary>
    /// A method to convert the XML file's value to a TimeSpan type of value
    /// </summary>
    /// <param name="element">The element which the value is located</param>
    /// <param name="name">The name of the tag which holds the value</param>
    /// <returns></returns>
    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name)
        => TimeSpan.TryParse((string?)element.Element(name),out var result)
        ? (TimeSpan?) result
        : null;

    #endregion

}
using DalApi;
namespace Dal;
/// <summary>
/// This class manages the id's and other system configuration settings via the XML file data-config.xml
/// </summary>
internal class ConfigImplementation : IConfig
{
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public void Reset()
    {
        Config.Reset();
    }
}

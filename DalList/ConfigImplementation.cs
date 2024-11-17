using DalApi;

namespace Dal;

public class ConfigImplementation : IConfig
{
    public TimeSpan RiskRange // Inherits from IConfig
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public DateTime Clock  // Inherits from IConfig
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public void Reset() // Inherits from IConfig
    {
        Config.Reset();
    }
}

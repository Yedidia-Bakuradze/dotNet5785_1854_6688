using DalApi;
using System.Runtime.CompilerServices;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public TimeSpan RiskRange // Inherits from IConfig
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.RiskRange = value;
    }
    public DateTime Clock  // Inherits from IConfig
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset() // Inherits from IConfig
    {
        Config.Reset();
    }
}

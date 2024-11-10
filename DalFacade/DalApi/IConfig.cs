
namespace DalApi;

public interface IConfig
{
    TimeSpan RiskRange { get; set; }
    DateTime Clock { get; set; }

    void Reset();
}

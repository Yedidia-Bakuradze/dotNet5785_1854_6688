namespace BlApi;

// BL managment instance Factory
public static class Factory
{
    /// <summary>
    /// This GET returns an managments instance of the BL layer for the PL layer
    /// </summary>
    /// <returns></returns>
    public static IBl Get() => new BlImplementation.Bl();
}

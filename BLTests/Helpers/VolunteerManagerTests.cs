
//TODO: Resolve the protected layer issue
//namespace Helpers.Tests;
//[TestClass()]
//public class VolunteerManagerTests
//{
//    /// <summary>
//    /// This tests checks that the geocoding is correct by comparing a known geocoding value
//    /// </summary>
//    [TestMethod()]
//    public void GetGeoCordinatesTest()
//    {
//        try
//        {
//            (double? latitude, double? longitude) = VolunteerManager.GetGeoCordinates("HaTothan 3 Jerusalem");
//            if (latitude == null || longitude == null)
//            {
//                Assert.Fail("Address hasn't been found");
//            }
//            Assert.AreEqual(31.8323803, (double)latitude, 0.0000001, "Latitude does not match the expected value.");
//            Assert.AreEqual(35.2411291, (double)longitude, 0.0000001, "Longitude does not match the expected value.");
//        }
//        catch (Exception ex)
//        {
//            Assert.Fail(ex.Message);
//        }
//    }
//}

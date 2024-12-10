

using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static bool IsCallIdValid(int id)
    {
        // Convert the ID to a string to validate length
        string idStr = id.ToString();

        // Check if the ID has exactly 10 digits
        if (idStr.Length != 10)
            return false;

        // Calculate the checksum
        int sum = 0;
        foreach (char c in idStr)
        {
            sum += c - '0'; // Convert each character to its numeric value
        }

        // Check if the sum is divisible by 7 (arbitrary checksum rule)
        return sum % 7 == 0;
    }
}

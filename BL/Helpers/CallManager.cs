

using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static bool IsCallIdValid(string id)
    {
        // Check if the ID is null or empty
        if (string.IsNullOrEmpty(id))
            return false;

        // Check if the ID has exactly 10 characters
        if (id.Length != 10)
            return false;

        // Check the format: AA000000AA
        if (!id.Take(2).All(char.IsUpper) || !id.Skip(2).Take(6).All(char.IsDigit) || !id.Skip(8).All(char.IsUpper))
            return false;

        // Calculate the checksum
        int sum = 0;
        foreach (char c in id)
        {
            if (char.IsUpper(c))
                sum += c - 'A' + 1; // 'A' -> 1, 'B' -> 2, ..., 'Z' -> 26
            else if (char.IsDigit(c))
                sum += c - '0'; // '0' -> 0, '1' -> 1, ..., '9' -> 9
        }

        // Check if the sum is divisible by 7
        return sum % 7 == 0;
    }
}

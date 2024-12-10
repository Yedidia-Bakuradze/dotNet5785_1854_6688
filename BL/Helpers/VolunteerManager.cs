using DalApi;

namespace Helpers;

//Remember: All the method shall be as internal static
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static bool IsVolunteerIdValid(int id)
    {
        // Convert the integer ID to a string for easier manipulation
        string idStr = id.ToString();

        // Check if the ID has exactly 9 digits
        if (idStr.Length != 9)
            return false;

        // Convert the ID to an integer array
        int[] digits = idStr.Select(c => c - '0').ToArray();

        // Calculate the sum according to the algorithm
        int sum = 0;
        for (int i = 0; i < 8; i++) // Only process the first 8 digits
        {
            int value = digits[i] * (i % 2 == 0 ? 1 : 2);
            sum += value > 9 ? value - 9 : value;
        }

        // Calculate the control digit
        int controlDigit = (10 - (sum % 10)) % 10;

        // Check if the control digit matches the last digit
        return controlDigit == digits[8];
    }

}

using Bogus.DataSets;

namespace InvenireServer.Tests.Integration.Extensions;

public static class FakerTestExtensions
{

    /// <summary>
    /// Generates a randomized, secure-looking password that includes at least one uppercase letter,
    /// one digit, and a mix of lowercase letters.
    /// </summary>
    /// <param name="length">The desired length of the password. Must be at least 3.</param>
    /// <returns>A randomized password string satisfying the constraints.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified length is less than 3.</exception>
    public static string SecurePassword(this Internet _, int length = 8)
    {
        var faker = new Faker();

        if (length < 3)
        {
            throw new ArgumentException("Password length must be at least 3 to satisfy all constraints.");
        }

        var upper = faker.Random.Char('A', 'Z').ToString();
        var digit = faker.Random.Number(0, 9).ToString();
        var rest = faker.Random.String2(length - 2, "abcdefghijklmnopqrstuvwxyz");

        var combined = upper + digit + rest;

        return new string([.. faker.Random.Shuffle(combined.ToCharArray())]);
    }
}
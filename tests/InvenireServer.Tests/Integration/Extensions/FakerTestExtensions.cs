using Bogus.DataSets;

namespace InvenireServer.Tests.Integration.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Internet"/> faker to generate secure test data.
/// </summary>
public static class FakerTestExtensions
{
    /// <summary>
    /// Generates a secure random password containing at least one uppercase letter, one digit, and the remaining characters as lowercase letters.
    /// </summary>
    /// <param name="_">The <see cref="Internet"/> faker instance (ignored).</param>
    /// <param name="length">The desired length of the generated password. Must be at least 3.</param>
    /// <returns>A randomly generated secure password string.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="length"/> is less than 3.</exception>
    public static string SecurePassword(this Internet _, int length = 8)
    {
        var faker = new Faker();

        if (length < 3)
        {
            throw new ArgumentException("Password length must be at least 3 to satisfy all constraints.");
        }

        var upper = faker.Random.Char('A', 'Z').ToString();
        var digit = faker.Random.Number(0, 9).ToString();
        var rest = faker.Random.String2(length - 2);

        var combined = upper + digit + rest;

        return new string([.. faker.Random.Shuffle(combined.ToCharArray())]);
    }
}
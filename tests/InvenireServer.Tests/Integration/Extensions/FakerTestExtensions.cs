using Bogus.DataSets;

namespace InvenireServer.Tests.Integration.Extensions;

public static class FakerTestExtensions
{
    public static string SecurePassword(this Internet _, int length = 8)
    {
        var faker = new Faker();

        if (length < 3) throw new ArgumentException("Password length must be at least 3 to satisfy all constraints.");

        var upper = faker.Random.Char('A', 'Z').ToString();
        var digit = faker.Random.Number(0, 9).ToString();
        var rest = faker.Random.String2(length - 2);

        var combined = upper + digit + rest;

        return new string([.. faker.Random.Shuffle(combined.ToCharArray())]);
    }
}
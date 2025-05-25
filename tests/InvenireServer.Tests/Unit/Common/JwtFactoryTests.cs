using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using InvenireServer.Application.Core.Factories;

namespace InvenireServer.Tests.Unit.Common;

public class JwtFactoryTests
{
    [Fact]
    public void Constructor_ReturnsCorrectInstanceWithConfiguration()
    {
        // Prepare.
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Key", "test_key" },
                { "Jwt:ExpirationTime", "45" }
            }).Build();

        // Act & Assert.
        var factory = new JwtFactory(config);

        factory.Issuer.Should().Be("test_issuer");
        factory.SigningKey.Should().Be("test_key");
        factory.ExpirationTime.Should().Be(TimeSpan.FromMinutes(45));
    }

    [Fact]
    public void Generate_ReturnsCorrectInstance()
    {
        // Prepare.
        var payload = new List<Claim>
        {
            new("name", "test_name"),
            new("admin", "true")
        };
        const string key = "test_key";
        const string issuer = "test_issuer";
        var factory = new JwtFactory(key, issuer);

        // Act & Assert.
        var jwt = factory.Create(payload);

        jwt.Header.Should().ContainSingle(c => c.Type == "typ" && c.Value == "JWT");
        jwt.Header.Should().ContainSingle(c => c.Type == "alg" && c.Value == "HS256");
        jwt.Payload.Should().ContainSingle(c => c.Type == "exp");
        jwt.Payload.Should().ContainSingle(c => c.Type == "iss" && c.Value == issuer);
        jwt.Payload.Should().ContainSingle(c => c.Type == "aud" && c.Value == issuer);
        jwt.Signature.Should().NotBeNullOrEmpty();
    }
}
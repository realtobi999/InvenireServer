using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using InvenireServer.Domain.Core.Entities.Common;

namespace InvenireServer.Tests.Unit.Common;

public class JwtTests
{
    [Fact]
    public void Write_ReturnsCorrectString()
    {
        // Prepare.
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };
        var payload = new List<Claim>
        {
            new("sub", "1234567890"),
            new("name", "test_name"),
            new("admin", "true")
        };
        const string signature = "test_signature";
        var jwt = new Jwt(header, payload, signature);

        // Act & Assert.
        var result = jwt.Write();

        var expectedHeaderJson = JsonSerializer.Serialize(header.ToDictionary(c => c.Type, object (c) => c.Value));
        var expectedPayloadJson = JsonSerializer.Serialize(payload.ToDictionary(c => c.Type, object (c) => c.Value));
        var expected = $"{EncodeBase64Url(Encoding.UTF8.GetBytes(expectedHeaderJson))}.{EncodeBase64Url(Encoding.UTF8.GetBytes(expectedPayloadJson))}.{signature}";

        expected.Should().Be(result);
    }

    [Fact]
    public void Parse_ReturnsCorrectInstance()
    {
        // Prepare.
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };
        var payload = new List<Claim>
        {
            new("sub", "1234567890"),
            new("name", "test_name"),
            new("admin", "true")
        };
        const string signature = "test_signature";
        var headerJson = JsonSerializer.Serialize(header.ToDictionary(c => c.Type, object (c) => c.Value));
        var payloadJson = JsonSerializer.Serialize(payload.ToDictionary(c => c.Type, object (c) => c.Value));

        // Act & Assert.
        var jwt = Jwt.Parse($"{EncodeBase64Url(Encoding.UTF8.GetBytes(headerJson))}.{EncodeBase64Url(Encoding.UTF8.GetBytes(payloadJson))}.{signature}");

        jwt.Header.Should().BeEquivalentTo(header);
        jwt.Payload.Should().BeEquivalentTo(payload);
        jwt.Signature.Should().Be(signature);
    }

    [Fact]
    public void Parse_ThrowsExceptionWhenIncorrectJwtFormat()
    {
        // Prepare.
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };
        var payload = new List<Claim>
        {
            new("sub", "1234567890"),
            new("name", "test_name"),
            new("admin", "true")
        };
        const string signature = "test_signature";
        var payloadJson = JsonSerializer.Serialize(payload.ToDictionary(c => c.Type, object (c) => c.Value));

        // Act & Assert.
        var act = () => Jwt.Parse($"{EncodeBase64Url(Encoding.UTF8.GetBytes(payloadJson))}.{signature}"); // We omit the signature part.

        act.Should().Throw<ArgumentException>().WithMessage("Invalid JWT token format.");
    }

    private static string EncodeBase64Url(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
namespace InvenireServer.Application.Attributes;

/// <summary>
/// Marks a class as a JSON-based request received from a client.
/// </summary>
/// <remarks>
/// Each field or property in the annotated class must be  explicitly  decorated
/// with <see cref="JsonPropertyName"/> if it is part of the  request  body,  or
/// <see cref="JsonIgnoreAttribute"/> if it is not.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class JsonRequestAttribute : Attribute;
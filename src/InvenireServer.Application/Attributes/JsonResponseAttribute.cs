namespace InvenireServer.Application.Attributes;

/// <summary>
/// Marks a class as a JSON-based response that will be send from the server.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class JsonResponseAttribute : Attribute;

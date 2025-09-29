namespace InvenireServer.Application.Interfaces.Common;

public interface IQuickResponseCodeGenerator
{
    byte[] GenerateCode(string content, int width = 150, int height = 150);
    byte[] GenerateCodeWithLabel(string content, string? label, int width = 150, int height = 150);
}

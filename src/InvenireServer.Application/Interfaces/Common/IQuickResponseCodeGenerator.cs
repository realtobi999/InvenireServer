namespace InvenireServer.Application.Interfaces.Common;

public interface IQuickResponseCodeGenerator
{
    byte[] GenerateCode(string content, int width = 150, int height = 150);
    byte[] GenerateCodeWithLabels(string content, IEnumerable<string> labels, int width = 150, int height = 150);
}

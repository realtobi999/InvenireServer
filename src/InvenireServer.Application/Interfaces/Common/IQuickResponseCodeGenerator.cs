namespace InvenireServer.Application.Interfaces.Common;

public interface IQuickResponseCodeGenerator
{
    public const int MinimumSize = 150;
    public const int MaximumSize = 550;

    byte[] GenerateCode(string content, int size = MinimumSize);
    byte[] GenerateCodeWithLabels(string content, IReadOnlyList<string> labels, int size = MinimumSize);
}

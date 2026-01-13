namespace InvenireServer.Application.Interfaces.Common;

/// <summary>
/// Defines a generator for quick response (QR) code images.
/// </summary>
public interface IQuickResponseCodeGenerator
{
    /// <summary>
    /// Minimum allowed size in pixels for generated QR code images.
    /// </summary>
    public const int MinimumSize = 150;

    /// <summary>
    /// Maximum allowed size in pixels for generated QR code images.
    /// </summary>
    public const int MaximumSize = 550;

    /// <summary>
    /// Generates a PNG-encoded QR code image.
    /// </summary>
    /// <param name="content">Content encoded in the QR code.</param>
    /// <param name="size">Image size in pixels.</param>
    /// <returns>PNG-encoded QR code image.</returns>
    byte[] GenerateCode(string content, int size = MinimumSize);

    /// <summary>
    /// Generates a PNG-encoded QR code image with labels.
    /// </summary>
    /// <param name="content">Content encoded in the QR code.</param>
    /// <param name="labels">Labels rendered beneath the code.</param>
    /// <param name="size">Image size in pixels.</param>
    /// <returns>PNG-encoded QR code image.</returns>
    byte[] GenerateCodeWithLabels(string content, IReadOnlyList<string> labels, int size = MinimumSize);
}

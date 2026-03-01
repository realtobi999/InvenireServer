using System.Globalization;
using System.Text;
using CsvHelper;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromCsv;

/// <summary>
/// Parses CSV files into organization invitation commands.
/// </summary>
public class ImportFromCsvOrganizationInvitationParser
{
    /// <summary>
    /// Parses organization invitation commands from a CSV stream.
    /// </summary>
    /// <param name="stream">CSV input stream.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning parsed invitation commands.</returns>
    public static async Task<List<CreateOrganizationInvitationCommand>> Parse(Stream stream, CancellationToken ct)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, true, -1, true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var commands = new List<CreateOrganizationInvitationCommand>();

        await foreach (var row in csv.GetRecordsAsync<CreateOrganizationInvitationCommandCsvRow>(ct))
        {
            commands.Add(new CreateOrganizationInvitationCommand
            {
                Id = row.Id,
                EmployeeId = row.EmployeeId,
                Description = row.Description,
                EmployeeEmailAddress = row.EmployeeEmailAddress
            });
        }

        return commands;
    }
}

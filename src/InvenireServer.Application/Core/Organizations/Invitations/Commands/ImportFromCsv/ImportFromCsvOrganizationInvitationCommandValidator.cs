using System.Text;
using FluentValidation;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromCsv;

/// <summary>
/// Defines validation rules for importing organization invitations from CSV.
/// </summary>
public class ImportFromCsvOrganizationInvitationCommandValidator : AbstractValidator<ImportFromCsvOrganizationInvitationCommand>
{
    public ImportFromCsvOrganizationInvitationCommandValidator()
    {
        RuleFor(c => c.Stream)
            .NotNull()
            .Must(stream => stream.CanRead)
                .WithMessage("'stream' must be readable.")
            .Must(stream => !stream.CanSeek || stream.Length > 0)
                .WithMessage("'stream' must not be empty.")
            .WithName("stream")
            .DependentRules(() => // perform validation of all the invitations only if all the rules above passed.
                RuleFor(c => c)
                    .CustomAsync(async (c, context, ct) =>
                    {
                        // ensure the stream is at position 0 before reading.
                        if (c.Stream.CanSeek)
                            c.Stream.Position = 0;

                        var invitations = (List<CreateOrganizationInvitationCommand>?)null;
                        try
                        {
                            invitations = await ImportFromCsvOrganizationInvitationParser.Parse(c.Stream, ct);
                        }
                        catch
                        {
                            throw new BadRequest400Exception("The file is corrupted or in a bad format.");
                        }

                        for (int i = 0; i < invitations.Count; i++)
                        {
                            var invitation = invitations[i];
                            var result = new CreateOrganizationInvitationCommandValidator().Validate(invitation);

                            if (!result.IsValid)
                                foreach (var error in result.Errors)
                                {
                                    var identifier = invitation.EmployeeEmailAddress;
                                    context.AddFailure($"Invitation {(string.IsNullOrEmpty(identifier) ? "MISSING_IDENTIFIER" : identifier)}: {error.ErrorMessage}");
                                }
                        }
                    })
            );
    }
}

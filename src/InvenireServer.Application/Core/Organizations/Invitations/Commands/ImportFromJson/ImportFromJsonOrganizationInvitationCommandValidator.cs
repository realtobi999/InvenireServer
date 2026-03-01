using System.Text.Json;
using FluentValidation;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromJson;

/// <summary>
/// Defines validation rules for importing organization invitations from JSON.
/// </summary>
public class ImportFromJsonOrganizationInvitationCommandValidator : AbstractValidator<ImportFromJsonOrganizationInvitationCommand>
{
    public ImportFromJsonOrganizationInvitationCommandValidator()
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
                            invitations = JsonSerializer.Deserialize<List<CreateOrganizationInvitationCommand>>(await new StreamReader(c.Stream).ReadToEndAsync(ct))!;
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

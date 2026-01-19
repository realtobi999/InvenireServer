using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Properties.Commands.Create;
using InvenireServer.Application.Core.Properties.Commands.Delete;
using InvenireServer.Application.Core.Properties.Commands.Update;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.DeleteAll;
using InvenireServer.Application.Core.Properties.Items.Commands.GenerateCodes;
using InvenireServer.Application.Core.Properties.Items.Commands.ImportFromExcel;
using InvenireServer.Application.Core.Properties.Items.Commands.ImportFromJson;
using InvenireServer.Application.Core.Properties.Items.Commands.Scan;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Core.Properties.Scans.Commands.Complete;
using InvenireServer.Application.Core.Properties.Scans.Commands.Create;
using InvenireServer.Application.Core.Properties.Scans.Commands.Update;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers.Commands;

/// <summary>
/// Controller for property commands.
/// </summary>
[ApiController]
public class PropertyCommandController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyCommandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the request to create a property.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        };
        var result = await _mediator.Send(command);

        return Created($"/api/properties/{result.Property.Id}", null);
    }

    /// <summary>
    /// Handles the request to update a property.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties")]
    public async Task<IActionResult> Update([FromBody] UpdatePropertyCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        };
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to delete a property.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/properties")]
    public async Task<IActionResult> Delete()
    {
        await _mediator.Send(new DeletePropertyCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        });

        return NoContent();
    }

    /// <summary>
    /// Handles the request to create property items.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items")]
    public async Task<IActionResult> CreateItems([FromBody] CreatePropertyItemsCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        };

        await _mediator.Send(command);
        return Created();
    }

    /// <summary>
    /// Handles the request to import property items from a JSON file.
    /// </summary>
    /// <param name="file">Uploaded file.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items/json-file")]
    public async Task<IActionResult> ImportItemsFromJson(IFormFile file)
    {
        await _mediator.Send(new ImportFromJsonPropertyItemsCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Stream = file.OpenReadStream()
        });

        return Created();
    }

    /// <summary>
    /// Handles the request to import property items from an Excel file.
    /// </summary>
    /// <param name="file">Uploaded file.</param>
    /// <param name="columns">Column mapping string.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items/excel-file")]
    public async Task<IActionResult> ImportItemsFromExcel(IFormFile file, [FromQuery] string columns)
    {
        if (columns is null)
            throw new BadRequest400Exception($"The '{nameof(columns)}' query parameter is missing or invalid.");

        await _mediator.Send(new ImportFromExcelPropertyItemsCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Stream = file.OpenReadStream(),
            ColumnString = columns,
        });

        return Created();
    }

    /// <summary>
    /// Handles the request to generate property item codes.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <param name="size">Code size in pixels.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items/generate-codes")]
    public async Task<IActionResult> GenerateCodes([FromBody] GenerateCodesPropertyItemsCommand command, int size = 150)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Size = size
        };

        var zip = await _mediator.Send(command);

        return File(zip, "application/zip", "images.zip");
    }

    /// <summary>
    /// Handles the request to update property items.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties/items")]
    public async Task<IActionResult> UpdateItems([FromBody] UpdatePropertyItemsCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        };
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to delete property items.
    /// </summary>
    /// <param name="command">Request to handle, if provided.</param>
    /// <param name="wipe">Whether to delete all items when no request body is provided.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/properties/items")]
    public async Task<IActionResult> DeleteItems([FromBody] DeletePropertyItemsCommand? command, [FromQuery] bool wipe)
    {
        if (command is null)
        {
            if (!wipe) throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

            await _mediator.Send(new DeleteAllPropertyItemsCommand
            {
                Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
            });

            return NoContent();
        }

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        };

        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to scan a property item.
    /// </summary>
    /// <param name="itemId">Item identifier.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize()]
    [HttpPut("/api/properties/items/{itemId:guid}/scan")]
    public async Task<IActionResult> ScanItem(Guid itemId)
    {
        await _mediator.Send(new ScanPropertyItemCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            ItemId = itemId,
        });

        return NoContent();
    }

    /// <summary>
    /// Handles the request to create a property suggestion.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPost("/api/properties/suggestions")]
    public async Task<IActionResult> CreateSuggestion([FromBody] CreatePropertySuggestionCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        };
        var result = await _mediator.Send(command);

        return Created($"/api/properties/suggestions/{result.Suggestion.Id}", null);
    }

    /// <summary>
    /// Handles the request to update a property suggestion.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <param name="suggestionId">Suggestion identifier.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPut("/api/properties/suggestions/{suggestionId:guid}")]
    public async Task<IActionResult> UpdateSuggestion([FromBody] UpdatePropertySuggestionCommand command, Guid suggestionId)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            SuggestionId = suggestionId
        };
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to accept a property suggestion.
    /// </summary>
    /// <param name="suggestionId">Suggestion identifier.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties/suggestions/{suggestionId:guid}/accept")]
    public async Task<IActionResult> AcceptSuggestion(Guid suggestionId)
    {
        await _mediator.Send(new AcceptPropertySuggestionCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            SuggestionId = suggestionId
        });

        return NoContent();
    }

    /// <summary>
    /// Handles the request to decline a property suggestion.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <param name="suggestionId">Suggestion identifier.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties/suggestions/{suggestionId:guid}/decline")]
    public async Task<IActionResult> DeclineSuggestion([FromBody] DeclinePropertySuggestionCommand command, Guid suggestionId)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            SuggestionId = suggestionId
        };
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to delete a property suggestion.
    /// </summary>
    /// <param name="suggestionId">Suggestion identifier.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize()]
    [HttpDelete("/api/properties/suggestions/{suggestionId:guid}")]
    public async Task<IActionResult> DeleteSuggestion(Guid suggestionId)
    {
        await _mediator.Send(new DeletePropertySuggestionCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            SuggestionId = suggestionId,
        });

        return NoContent();
    }

    /// <summary>
    /// Handles the request to create a property scan.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/scans")]
    public async Task<IActionResult> CreateScan([FromBody] CreatePropertyScanCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        };
        var result = await _mediator.Send(command);

        return Created($"/api/properties/scans/{result.Scan.Id}", null);
    }

    /// <summary>
    /// Handles the request to update a property scan.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties/scans")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdatePropertyScanCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        };
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to complete a property scan.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties/scans/complete")]
    public async Task<IActionResult> CompleteScan()
    {
        await _mediator.Send(new CompletePropertyScanCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        });

        return NoContent();
    }
}

using MediatR;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Properties.Commands.Update;
using InvenireServer.Application.Core.Properties.Commands.Delete;
using InvenireServer.Application.Core.Properties.Scans.Commands.Update;
using InvenireServer.Application.Core.Properties.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.Scan;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Core.Properties.Scans.Commands.Complete;
using InvenireServer.Application.Core.Properties.Scans.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;
using InvenireServer.Application.Core.Properties.Items.Commands.DeleteAll;
using InvenireServer.Application.Core.Properties.Items.Commands.CreateFromJsonFile;
using InvenireServer.Application.Core.Properties.Items.Commands.CreateFromExcelFile;
using InvenireServer.Domain.Exceptions.Http;
using DocumentFormat.OpenXml.Wordprocessing;

namespace InvenireServer.Presentation.Controllers.Commands;

[ApiController]
public class PropertyCommandController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyCommandController(IMediator mediator)
    {
        _mediator = mediator;
    }

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

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items/json-file")]
    public async Task<IActionResult> CreateItemsFromJsonFile(IFormFile file)
    {
        await _mediator.Send(new CreatePropertyItemsFromJsonFileCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Stream = file.OpenReadStream()
        });

        return Created();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items/excel-file")]
    public async Task<IActionResult> CreateItemsFromExcelFile(IFormFile file, [FromQuery] string columns)
    {
        if (columns is null)
            throw new BadRequest400Exception($"The '{nameof(columns)}' query parameter is missing or invalid.");

        await _mediator.Send(new CreatePropertyItemsFromExcelFileCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Stream = file.OpenReadStream(),
            ColumnString = columns,
        });

        return Created();
    }

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
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Properties.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.Scan;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Core.Properties.Scans.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyController(IMediator mediator)
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
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        };
        var result = await _mediator.Send(command);

        return Created($"/api/properties/{result.Property.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items")]
    public async Task<IActionResult> CreateItems([FromBody] CreatePropertyItemsCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        };
        await _mediator.Send(command);

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
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        };
        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/properties/items")]
    public async Task<IActionResult> DeleteItems([FromQuery] List<Guid> ids)
    {
        if (ids.Count == 0)
            throw new ValidationException([new ValidationFailure("ids", "At least one ID must be provided.")]);

        await _mediator.Send(new DeletePropertyItemsCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            Ids = ids
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPost("/api/properties/suggestions/items")]
    public async Task<IActionResult> CreateItemsSuggestion([FromBody] PostPropertySuggestionRequest request)
    {
        if (request is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        var validator = new CreatePropertyItemCommandValidator();
        foreach (var command in request.RequestBody)
        {
            var validation = validator.Validate(command);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);
        }

        var result = await _mediator.Send(new CreatePropertySuggestionCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            RequestBody = JsonSerializer.Serialize(request.RequestBody),
            RequestType = PropertySuggestionRequestType.CREATE,
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Created($"/api/properties/suggestions/{result.Suggestion.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPut("/api/properties/suggestions/items")]
    public async Task<IActionResult> UpdateItemsSuggestion([FromBody] PutPropertySuggestionRequest request)
    {
        if (request is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        var validator = new UpdatePropertyItemCommandValidator();
        foreach (var command in request.RequestBody)
        {
            var validation = validator.Validate(command);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);
        }

        var result = await _mediator.Send(new CreatePropertySuggestionCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            RequestBody = JsonSerializer.Serialize(request.RequestBody),
            RequestType = PropertySuggestionRequestType.UPDATE,
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Created($"/api/properties/suggestions/{result.Suggestion.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpDelete("/api/properties/suggestions/items")]
    public async Task<IActionResult> DeleteItemsSuggestion([FromBody] DeletePropertySuggestionRequest request)
    {
        if (request is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        var result = await _mediator.Send(new CreatePropertySuggestionCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            RequestBody = JsonSerializer.Serialize(request.RequestBody),
            RequestType = PropertySuggestionRequestType.DELETE,
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Created($"/api/properties/suggestions/{result.Suggestion.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/suggestions/{suggestionId:guid}/accept")]
    public async Task<IActionResult> AcceptSuggestion(Guid suggestionId)
    {
        await _mediator.Send(new AcceptPropertySuggestionCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            SuggestionId = suggestionId
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/suggestions/{suggestionId:guid}/decline")]
    public async Task<IActionResult> DeclineSuggestion([FromBody] DeclinePropertySuggestionCommand command, Guid suggestionId)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            SuggestionId = suggestionId
        };
        await _mediator.Send(command);

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
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        };
        var result = await _mediator.Send(command);

        return Created($"/api/properties/scans/{result.Scan.Id}", null);
    }

    [Authorize]
    [HttpPost("/api/properties/items/{itemId:guid}/scan")]
    public async Task<IActionResult> ScanItem(Guid itemId)
    {
        await _mediator.Send(new ScanPropertyItemCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            ItemId = itemId,
        });

        return NoContent();
    }
}
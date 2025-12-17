using FluentValidation;

namespace InvenireServer.Application.Behaviors;

/// <summary>
/// Represents a pipeline behavior that executes before a command is  passed  to
/// its handler. This behavior  is  responsible  for  executing  all  validators
/// associated with the request and throwing a <see cref="ValidationException"/>
/// if any validation errors occur.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the request by  executing  all  registered  validators.  If  any
    /// validation errors are  found,  a  <see  cref="ValidationException"/>  is
    /// thrown. Otherwise, the next delegate in the pipeline is invoked.
    /// </summary>
    /// <param name="request">The incoming request to validate.</param>
    /// <param name="next">The delegate representing the next action in the pipeline, typically the request handler.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>The response from the next handler in the pipeline.</returns>
    /// <exception cref="ValidationException">Thrown when one or more validation errors occur.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token)
    {
        if (!_validators.Any())
            return await next(token);

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, token)));
        var errors = results
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (errors.Count > 0)
            throw new ValidationException("One or more request validation errors occurred", errors);

        return await next(token);
    }
}

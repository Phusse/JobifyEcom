using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Enums;
using Jobify.Ecom.Application.Exceptions;
using Jobify.Ecom.Application.Models;
using FluentValidation;
using FluentValidation.Results;

namespace Jobify.Ecom.Infrastructure.CQRS.Decorators;

internal class ValidationDecorator<TRequest, TResponse>(IHandler<TRequest, TResponse> next, IEnumerable<IValidator<TRequest>> validators)
    : IHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IHandler<TRequest, TResponse> _next = next;
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, CancellationToken ct)
    {
        if (_validators.Any())
        {
            IEnumerable<Task<ValidationResult>> validationTasks = _validators.Select(v => v.ValidateAsync(request, ct));
            ValidationResult[] validationResults = await Task.WhenAll(validationTasks);

            List<ResponseDetail> failures = [.. validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .Select(f => new ResponseDetail($"{f.PropertyName}: {f.ErrorMessage}", ResponseSeverity.Error))
            ];

            if (failures.Count > 0)
            {
                throw new AppException(
                    id: "SYSTEM_VALIDATION_FAILED",
                    statusCode: 400,
                    title: "Validation failed.",
                    details: [.. failures]
                );
            }
        }

        return await _next.Handle(request, ct);
    }
}

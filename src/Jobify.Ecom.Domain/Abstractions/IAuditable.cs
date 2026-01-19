namespace Jobify.Ecom.Domain.Abstractions;

internal interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}

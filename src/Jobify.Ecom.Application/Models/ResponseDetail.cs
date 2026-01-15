using Jobify.Ecom.Application.Enums;

namespace Jobify.Ecom.Application.Models;

public record ResponseDetail(
    string Message,
    ResponseSeverity Severity
);

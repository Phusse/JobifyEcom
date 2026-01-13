using Jobify.Application.Enums;

namespace Jobify.Application.Models;

public record ResponseDetail(
    string Message,
    ResponseSeverity Severity
);

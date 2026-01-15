namespace Jobify.Ecom.Application.Configurations.Security;

public record SessionManagementOptions
{
    public int StandardSessionDurationHours { get; init; }
    public int ExtendedSessionDurationDays { get; init; }
    public int AbsoluteSessionLimitDays { get; init; }
    public int StandardExpiryExtensionHours { get; init; }
    public int ExtendedExpiryExtensionHours { get; init; }
    public int ExpiryExtensionTriggerPercent { get; init; }
}

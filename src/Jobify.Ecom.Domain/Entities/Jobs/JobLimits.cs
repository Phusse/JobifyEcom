namespace Jobify.Ecom.Domain.Entities.Jobs;

public static class JobLimits
{
    public const int TitleMinLength = 1;
    public const int TitleMaxLength = 150;

    public const int DescriptionMinLength = 1;
    public const int DescriptionMaxLength = 3000;

    public const int MinAllowedMoney = 0;
    public const int MaxAllowedMoney = 1_000_000;
}

namespace Jobify.Ecom.Domain.Extensions.Validations;

internal static class StringExtensions
{
    extension(string value)
    {
        public string EnsureNotEmptyAndTrim()
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("value cannot be empty.");

            return value.Trim();
        }
    }
}

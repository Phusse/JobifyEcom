namespace JobifyEcom.Contracts.Results;

/// <summary>
/// Central registry of all standardized application results (non-error outcomes).
/// Each entry provides a unique code, a user-facing title, and optional details.
/// Organized into <c>partial</c> files by domain for easier maintenance,
/// and can be used to represent successful or informational outcomes in <c>ServiceResult</c>.
/// </summary>
internal static partial class ResultCatalog { }

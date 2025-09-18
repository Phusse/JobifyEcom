namespace JobifyEcom.Contracts.Routes;

/// <summary>
/// Central source of all API route paths, grouped by domain (Auth, User, Worker, Job, Metadata)
/// and HTTP verb (GET, POST, PATCH, DELETE).
/// Implemented as a partial class for maintainability.
/// </summary>
internal static partial class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
}

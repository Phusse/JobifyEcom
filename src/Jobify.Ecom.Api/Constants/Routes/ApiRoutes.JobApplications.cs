namespace Jobify.Ecom.Api.Constants.Routes;

internal static partial class ApiRoutes
{
    public static class JobApplications
    {
        public const string Base = $"{ApiBasePath}/applications";
        public const string GetById = "/{applicationId:guid}";
        public const string ForJob = $"{Jobs.Base}/{{jobId:guid}}/applications";
        public const string MyApplications = "/me";
    }
}

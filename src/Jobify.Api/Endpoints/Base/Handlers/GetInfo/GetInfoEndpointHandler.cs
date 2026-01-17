using Jobify.Api.Models;

namespace Jobify.Api.Endpoints.Base.Handlers.GetInfo;

internal static class GetInfoEndpointHandler
{
    private static readonly string[] Features = [
        "User Authentication & Management",
        "Job Posting & Candidate Sourcing",
        "Job Matching & Recommendations",
        "Roadmap & Career Planning",
        "AI-Powered Insights",
    ];

    public static IResult Handle()
    {
        ApiResponse<object> response = new(
            Success: true,
            MessageId: "SYSTEM_API_INFO",
            Message: "Jobify API Gateway Information",
            Details: null,
            Data: new
            {
                Name = "Jobify API Gateway",
                Version = "1.0.0",
                Description = "A scalable and secure API platform powering job matching, recruitment, and career management.",
                Features,
                Documentation = "/scalar (development only endpoint)",
            }
        );

        return Results.Ok(response);
    }
}

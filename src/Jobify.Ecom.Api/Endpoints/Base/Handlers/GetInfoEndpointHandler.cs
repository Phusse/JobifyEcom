using Jobify.Ecom.Api.Models;

namespace Jobify.Ecom.Api.Endpoints.Base.Handlers;

public static class GetInfoEndpointHandler
{
    private static readonly string[] Features = [
        "Job Posting & Candidate Sourcing",
        "User Scouting & Engagement",
    ];

    public static IResult Handle()
    {
        ApiResponse<object> response = new(
            Success: true,
            MessageId: "SYSTEM_API_INFO",
            Message: "Jobify Ecom API Information",
            Details: null,
            Data: new
            {
                Name = "Jobify Ecom API",
                Version = "1.0.0",
                Description = "A robust e-commerce API for job postings, candidate sourcing, scouting and engagement.",
                Features,
                Documentation = "/docs (development only endpoint)",
            }
        );

        return Results.Ok(response);
    }
}

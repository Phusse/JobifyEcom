using Jobify.Ecom.Application.Constants.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Extensions.Responses;
using Jobify.Ecom.Application.Features.JobApplications.Extensions;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.JobApplications;
using Jobify.Ecom.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Application.Features.JobApplications.GetApplicationById;

public class GetApplicationByIdQueryHandler(AppDbContext context) : IHandler<GetApplicationByIdQuery, OperationResult<JobApplicationResponse>>
{
    public async Task<OperationResult<JobApplicationResponse>> Handle(GetApplicationByIdQuery message, CancellationToken cancellationToken = default)
    {
        if (message.RequestingUserId is not Guid requestingUserId)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        JobApplication application = await context.JobApplications
            .AsNoTracking()
            .Include(ja => ja.Job)
            .FirstOrDefaultAsync(ja => ja.Id == message.ApplicationId, cancellationToken)
            ?? throw ResponseCatalog.JobApplication.NotFound.ToException();

        bool isApplicant = application.ApplicantUserId == requestingUserId;
        bool isJobOwner = application.Job.PostedByUserId == requestingUserId;

        if (!isApplicant && !isJobOwner)
            throw ResponseCatalog.JobApplication.ModificationForbidden.ToException();

        return ResponseCatalog.JobApplication.Found
            .As<JobApplicationResponse>()
            .WithData(application.ToResponse())
            .ToOperationResult();
    }
}

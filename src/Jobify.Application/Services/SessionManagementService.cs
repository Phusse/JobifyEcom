using Jobify.Application.Configurations.Security;
using Jobify.Application.Constants.Cache;
using Jobify.Application.Extensions.Entities;
using Jobify.Application.Models;
using Jobify.Domain.Entities.UserSessions;
using Jobify.Domain.Enums;
using Jobify.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Jobify.Application.Services;

public class SessionManagementService
{
    private readonly TimeSpan _standardSessionDuration;
    private readonly TimeSpan _extendedSessionDuration;
    private readonly TimeSpan _absoluteSessionLimit;

    private readonly TimeSpan _standardExpiryExtension;
    private readonly TimeSpan _extendedExpiryExtension;

    private readonly double _expiryExtensionTriggerPercent;

    private readonly ICacheService _cacheService;
    private readonly AppDbContext _db;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

    public SessionManagementService(IOptions<SessionManagementOptions> options, ICacheService cacheService, AppDbContext db)
    {
        _cacheService = cacheService;
        _db = db;
        SessionManagementOptions opts = options.Value;

        if (opts.StandardSessionDurationHours <= 0)
            throw new InvalidOperationException("SessionManagement:StandardSessionDurationHours must be greater than 0.");

        if (opts.ExtendedSessionDurationDays <= 0)
            throw new InvalidOperationException("SessionManagement:ExtendedSessionDurationDays must be greater than 0.");

        if (opts.AbsoluteSessionLimitDays < opts.ExtendedSessionDurationDays)
            throw new InvalidOperationException("SessionManagement:AbsoluteSessionLimitDays must be greater than or equal to ExtendedSessionDurationDays.");

        if (opts.ExpiryExtensionTriggerPercent < 1 || opts.ExpiryExtensionTriggerPercent > 100)
            throw new InvalidOperationException("SessionManagement:ExpiryExtensionTriggerPercent must be between 1 and 100.");

        _standardSessionDuration = TimeSpan.FromHours(opts.StandardSessionDurationHours);
        _extendedSessionDuration = TimeSpan.FromDays(opts.ExtendedSessionDurationDays);
        _absoluteSessionLimit = TimeSpan.FromDays(opts.AbsoluteSessionLimitDays);

        _standardExpiryExtension = TimeSpan.FromHours(opts.StandardExpiryExtensionHours);
        _extendedExpiryExtension = TimeSpan.FromHours(opts.ExtendedExpiryExtensionHours);

        _expiryExtensionTriggerPercent = opts.ExpiryExtensionTriggerPercent;
    }

    public async Task<UserSession> CreateSessionAsync(Guid userId, SystemRole systemRole, bool rememberMe = false, CancellationToken cancellationToken = default)
    {
        TimeSpan initialLifetime = rememberMe ? _extendedSessionDuration : _standardSessionDuration;
        UserSession session = new(userId, rememberMe, initialLifetime, _absoluteSessionLimit);

        _db.UserSessions.Add(session);
        await _db.SaveChangesAsync(cancellationToken);

        SessionData sessionData = session.ToSessionData(systemRole, isLocked: true);
        await _cacheService.SetAsync(CacheKey(session.Id), sessionData, CacheTtl);

        return session;
    }

    public async Task<SessionData?> GetSessionDataAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        SessionData? cachedSession = await _cacheService.GetAsync<SessionData>(CacheKey(sessionId));

        if (cachedSession is not null) return cachedSession;

        SessionData? sessionDataDto = await _db.UserSessions
            .AsNoTracking()
            .Where(s => s.Id == sessionId)
            .Select(s => new SessionData
            (
                SessionId: s.Id,
                UserId: s.UserId,
                ExpiresAt: s.ExpiresAt,
                AbsoluteExpiresAt: s.AbsoluteExpiresAt,
                IsRevoked: s.IsRevoked,
                RememberMe: s.RememberMe,
                IsLocked: s.User.IsLocked,
                Role: s.User.Role
            ))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (sessionDataDto is null) return null;

        TimeSpan cacheDuration = CalculateCacheTimeToLive(sessionDataDto.ExpiresAt, CacheTtl);

        if (cacheDuration > TimeSpan.Zero)
            await _cacheService.SetAsync(CacheKey(sessionId), sessionDataDto, cacheDuration);

        return sessionDataDto;
    }

    public async Task<SessionData?> ExtendSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var sessionDataDto = await _db.UserSessions
            .Where(s => s.Id == sessionId)
            .Select(s => new
            {
                Session = s,
                s.User.IsLocked,
                s.User.Role,
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (sessionDataDto is null || sessionDataDto.Session is null || sessionDataDto.Session.IsExpired()) return null;

        if (sessionDataDto.Session.ShouldRefresh(_expiryExtensionTriggerPercent))
        {
            TimeSpan extension = sessionDataDto.Session.RememberMe
                ? _extendedExpiryExtension
                : _standardExpiryExtension;

            sessionDataDto.Session.ExtendSession(extension);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKey(sessionDataDto.Session.Id));

        return sessionDataDto.Session.ToSessionData(sessionDataDto.Role, sessionDataDto.IsLocked);
    }

    public async Task<bool> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        UserSession? session = await _db.UserSessions.FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken: cancellationToken);

        if (session is null || session.IsRevoked) return false;

        session.Revoke();
        await _db.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKey(sessionId));

        return true;
    }

    public async Task<bool> DeleteSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        UserSession? session = await _db.UserSessions.FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session is null) return false;

        _db.UserSessions.Remove(session);
        await _db.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKey(sessionId));

        return true;
    }

    public async Task<bool> DeleteAllUserSessionsExceptAsync(Guid userId, Guid? currentSessionId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<UserSession> query = _db.UserSessions.Where(s => s.UserId == userId);

        if (currentSessionId.HasValue)
            query = query.Where(s => s.Id != currentSessionId.Value);

        List<Guid> sessionIds = await query
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        if (sessionIds.Count is 0) return false;

        await query.ExecuteDeleteAsync(cancellationToken);

        foreach (Guid sessionId in sessionIds)
            await _cacheService.RemoveAsync(CacheKey(sessionId));

        return true;
    }

    private static string CacheKey(Guid sessionId) => $"{CacheKeys.Session}{sessionId:N}";

    private static TimeSpan CalculateCacheTimeToLive(DateTime expiresAtUtc, TimeSpan maxTimeToLive)
    {
        TimeSpan remaining = expiresAtUtc - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero) return TimeSpan.Zero;

        return remaining < maxTimeToLive ? remaining : maxTimeToLive;
    }
}

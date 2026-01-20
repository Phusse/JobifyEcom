SELECT *
FROM UserSessions
WHERE
    IsRevoked = 1
	OR ExpiresAt <= SYSUTCDATETIME()
	OR AbsoluteExpiresAt <= SYSUTCDATETIME();

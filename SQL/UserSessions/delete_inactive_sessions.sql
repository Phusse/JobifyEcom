BEGIN TRANSACTION;

DELETE FROM UserSessions
WHERE
    IsRevoked = 1
	OR ExpiresAt <= SYSUTCDATETIME()
	OR AbsoluteExpiresAt <= SYSUTCDATETIME();

-- ROLLBACK; -- use this while verifying
-- COMMIT;   -- uncomment only when you're confident

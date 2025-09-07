namespace JobifyEcom.DTOs.Worker;

/// <summary>
/// Represents the response for creating a worker profile.
/// </summary>
public class WorkerProfileResponse
{
	/// <summary>
	/// Gets or sets the unique identifier for the worker.
	/// </summary>
	public required Guid WorkerId { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the user.
	/// </summary>
	public required Guid UserId { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the worker profile was created.
	/// </summary>
	public required DateTime CreatedAt { get; set; }
}

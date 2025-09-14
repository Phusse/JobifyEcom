using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides a configured <see cref="JsonSerializerOptions"/> instance
/// for consistent JSON serialization and deserialization throughout the application.
/// </summary>
internal static class JsonOptionsFactory
{
	/// <summary>
	/// Creates and returns a preconfigured <see cref="JsonSerializerOptions"/> instance.
	/// </summary>
	/// <remarks>
	/// The returned options include:
	/// <list type="bullet">
	///   <item><description>CamelCase property naming policy.</description></item>
	///   <item><description>Ignores <c>null</c> values during serialization.</description></item>
	///   <item><description>Case-insensitive property name matching during deserialization.</description></item>
	///   <item><description>Ignores reference cycles when serializing objects.</description></item>
	///   <item><description>Serializes enums as camelCase strings (no integer values allowed).</description></item>
	/// </list>
	/// </remarks>
	/// <returns>
	/// A configured instance of <see cref="JsonSerializerOptions"/>
	/// that can be reused across serialization and deserialization operations.
	/// </returns>
	internal static JsonSerializerOptions Create()
	{
		JsonSerializerOptions options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNameCaseInsensitive = true,
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
		};

		options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
		return options;
	}
}

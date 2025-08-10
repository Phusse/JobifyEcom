using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobifyEcom.Helpers
{
	public static class JsonOptionsFactory
	{
		public static JsonSerializerOptions Create()
		{
			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNameCaseInsensitive = true,
				ReferenceHandler = ReferenceHandler.IgnoreCycles
			};

			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));

			return options;
		}
	}
}

using System.Text.Json.Serialization;

namespace DeviceManager.Core.Services.DeviceTokenService
{
    public record TokenContent
    {
        [JsonPropertyName("id")] public long DeviceId { get; init; }
        [JsonPropertyName("s")] public string SecurityStamp { get; init; }
    }
}
using System.Text.Json.Serialization;

namespace DynDNS.Models.Dns;

public record DnsRecord
{
    [JsonPropertyName("id")] public uint? Id { get; init; }

    [JsonPropertyName("hostname")] public required string HostName { get; init; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RecordType Type { get; init; }

    [JsonPropertyName("priority")] public string? Priority { get; init; }

    [JsonPropertyName("destination")] public required string Destination { get; init; }

    [JsonPropertyName("deleterecord")] public bool? DeleteRecord { get; init; }

    [JsonPropertyName("state")] public string? State { get; init; }
}
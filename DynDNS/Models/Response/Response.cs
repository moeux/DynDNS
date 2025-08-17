using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynDNS.Models.Response;

public class Response
{
    [JsonPropertyName("serverrequestid")] public required string ServerRequestId { get; init; }

    [JsonPropertyName("clientrequestid")] public string? ClientRequestId { get; init; }

    [JsonPropertyName("action")] public required string Action { get; init; }

    [JsonPropertyName("status")] public required string Status { get; init; }

    [JsonPropertyName("statuscode")] public required uint StatusCode { get; init; }

    [JsonPropertyName("shortmessage")] public required string ShortMessage { get; init; }

    [JsonPropertyName("longmessage")] public string? LongMessage { get; init; }

    [JsonInclude]
    [JsonPropertyName("responsedata")]
    private JsonElement ResponseData { get; init; }

    [JsonIgnore]
    public IImmutableDictionary<string, string> Data
    {
        get
        {
            if (ResponseData.ValueKind == JsonValueKind.Object)
                return JsonSerializer.Deserialize<IImmutableDictionary<string, string>>(ResponseData.GetRawText()) ??
                       ImmutableDictionary<string, string>.Empty;

            return ImmutableDictionary<string, string>.Empty;
        }
    }
}
using System.Text.Json.Serialization;

namespace DynDNS.Models.Request;

public abstract class RequestBase
{
    protected RequestBase(string action, string clientRequestId)
    {
        Action = action;
        Parameters["clientrequestid"] = clientRequestId;
    }

    [JsonInclude]
    [JsonPropertyName("action")]
    private string Action { get; }

    [JsonInclude]
    [JsonPropertyName("param")]
    internal IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
}
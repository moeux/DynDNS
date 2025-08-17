using System.Net.Http.Json;
using System.Text.Json;
using DynDNS.Models.Dns;
using DynDNS.Models.Request;
using DynDNS.Models.Response;

namespace DynDNS.Client;

public static class NetcupClient
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(GetEnvironmentVariable("NETCUP_BASE_ADDRESS"))
    };

    private static readonly Dictionary<string, string> AuthDictionary = new()
    {
        ["customernumber"] = GetEnvironmentVariable("NETCUP_CUSTOMER_NUMBER"),
        ["apikey"] = GetEnvironmentVariable("NETCUP_API_KEY"),
        ["apipassword"] = GetEnvironmentVariable("NETCUP_API_PASSWORD")
    };

    private static string GetEnvironmentVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? throw new ArgumentNullException(key);
    }

    public static async Task LoginAsync()
    {
        var loginRequest = new LoginRequest
        {
            Parameters =
            {
                ["customernumber"] = AuthDictionary["customernumber"],
                ["apikey"] = AuthDictionary["apikey"],
                ["apipassword"] = AuthDictionary["apipassword"]
            }
        };

        using var response = await Client.PostAsJsonAsync(string.Empty, loginRequest, JsonSerializerOptions.Web);

        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<Response>() ??
                            throw new HttpRequestException("Failed to deserialize login response");

        if (loginResponse.Status != Status.Success) throw new HttpRequestException("Login unsuccessful");

        AuthDictionary["apisessionid"] = loginResponse.Data["apisessionid"];

        Console.WriteLine(loginResponse.LongMessage ?? loginResponse.ShortMessage);
    }

    public static async Task LogoutAsync()
    {
        var logoutRequest = new LogoutRequest
        {
            Parameters =
            {
                ["customernumber"] = AuthDictionary["customernumber"],
                ["apikey"] = AuthDictionary["apikey"],
                ["apisessionid"] = AuthDictionary["apisessionid"]
            }
        };

        using var response = await Client.PostAsJsonAsync(string.Empty, logoutRequest, JsonSerializerOptions.Web) ??
                             throw new HttpRequestException("Failed to deserialize logout response");

        response.EnsureSuccessStatusCode();

        var logoutResponse = await response.Content.ReadFromJsonAsync<Response>();

        if (logoutResponse?.Status != Status.Success) throw new HttpRequestException("Logout unsuccessful");

        Console.WriteLine(logoutResponse.LongMessage ?? logoutResponse.ShortMessage);
    }

    public static async Task<DnsRecord[]> UpdateDnsRecordsAsync(string domainName, params DnsRecord[] dnsRecords)
    {
        if (dnsRecords.Any(dnsRecord =>
                string.IsNullOrWhiteSpace(dnsRecord.Priority) && dnsRecord.Type == RecordType.MX))
            throw new ArgumentException("MX DNS records must have priority set");

        var updateDnsRecordsRequest = new UpdateDnsRecordsRequest
        {
            Parameters =
            {
                ["domainname"] = domainName,
                ["dnsrecordset"] = new Dictionary<string, object>
                {
                    ["dnsrecords"] = dnsRecords
                },
                ["customernumber"] = AuthDictionary["customernumber"],
                ["apikey"] = AuthDictionary["apikey"],
                ["apisessionid"] = AuthDictionary["apisessionid"]
            }
        };

        using var response =
            await Client.PostAsJsonAsync(string.Empty, updateDnsRecordsRequest, JsonSerializerOptions.Web);

        response.EnsureSuccessStatusCode();

        var updateDnsRecordsResponse = await response.Content.ReadFromJsonAsync<Response>();

        if (updateDnsRecordsResponse?.Status != Status.Success)
            throw new HttpRequestException("UpdateDnsRecords unsuccessful");

        Console.WriteLine(updateDnsRecordsResponse.LongMessage ?? updateDnsRecordsResponse.ShortMessage);

        return updateDnsRecordsResponse.DnsRecordsData["dnsrecords"];
    }

    public static async Task<string> GetIpAddress()
    {
        return await Client.GetStringAsync("https://api.ipify.org");
    }
}
using DynDNS.Client;
using DynDNS.Models.Dns;

namespace DynDNS;

internal static class Program
{
    private static DnsRecord? _previousDnsRecord;

    private static readonly string DomainName =
        Environment.GetEnvironmentVariable("NETCUP_DOMAINNAME") ?? throw new ArgumentNullException();

    private static readonly string HostName =
        Environment.GetEnvironmentVariable("NETCUP_HOSTNAME") ?? throw new ArgumentNullException();

    private static async Task Main()
    {
        while (true)
        {
            var currentIp = await NetcupClient.GetIpAddress();

            if (currentIp != _previousDnsRecord?.Destination)
            {
                Console.WriteLine($"Updating DNS - {DateTime.Now}");
                await UpdateDnsRecord(currentIp);
            }

            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }

    private static async Task UpdateDnsRecord(string currentIp)
    {
        await NetcupClient.LoginAsync();

        var newDnsRecord = new DnsRecord
        {
            HostName = HostName,
            Type = RecordType.A,
            Destination = currentIp
        };
        var updatedDnsRecords = await NetcupClient.UpdateDnsRecordsAsync(
            DomainName,
            _previousDnsRecord is null
                ? [newDnsRecord]
                : [newDnsRecord, _previousDnsRecord with { DeleteRecord = true }]);
        await NetcupClient.LogoutAsync();

        _previousDnsRecord = updatedDnsRecords.First(dnsRecord =>
            dnsRecord.HostName == newDnsRecord.HostName &&
            dnsRecord.Type == newDnsRecord.Type &&
            dnsRecord.Destination == newDnsRecord.Destination);
    }
}
using DynDNS.Client;
using DynDNS.Models.Dns;

namespace DynDNS;

internal static class Program
{
    private static string _ip = string.Empty;

    private static readonly string DomainName =
        Environment.GetEnvironmentVariable("NETCUP_DOMAINNAME") ?? throw new ArgumentNullException();

    private static readonly string HostName =
        Environment.GetEnvironmentVariable("NETCUP_HOSTNAME") ?? throw new ArgumentNullException();

    private static async Task Main()
    {
        while (true)
        {
            var currentIp = await NetcupClient.GetIpAddress();

            if (currentIp != _ip)
            {
                Console.WriteLine($"Updating DNS - {DateTime.Now}");
                _ip = currentIp;
                await UpdateDnsRecord();
            }

            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }

    private static async Task UpdateDnsRecord()
    {
        await NetcupClient.LoginAsync();
        await NetcupClient.UpdateDnsRecordsAsync(
            DomainName,
            new DnsRecord
            {
                HostName = HostName,
                Type = RecordType.A,
                Destination = _ip
            });
        await NetcupClient.LogoutAsync();
    }
}
using DynDNS.Client;
using DynDNS.Models.Dns;

namespace DynDNS;

internal static class Program
{
    private static string _ip = string.Empty;

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
            Environment.GetEnvironmentVariable("NETCUP_DOMAINNAME") ?? throw new ArgumentNullException(),
            new DnsRecord
            {
                HostName = Environment.GetEnvironmentVariable("NETCUP_HOSTNAME") ??
                           throw new ArgumentNullException(),
                Type = RecordType.A,
                Destination = _ip
            });
        await NetcupClient.LogoutAsync();
    }
}
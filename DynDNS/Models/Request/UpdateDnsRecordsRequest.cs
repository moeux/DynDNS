namespace DynDNS.Models.Request;

public class UpdateDnsRecordsRequest()
    : RequestBase(Action.UpdateDnsRecords, $"{nameof(UpdateDnsRecordsRequest)}{Guid.NewGuid():N}");
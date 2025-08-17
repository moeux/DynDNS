namespace DynDNS.Models.Request;

public class LogoutRequest() : RequestBase(Action.Logout, $"{nameof(LogoutRequest)}{Guid.NewGuid():N}");
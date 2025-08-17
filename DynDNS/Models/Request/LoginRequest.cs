namespace DynDNS.Models.Request;

public class LoginRequest() : RequestBase(Action.Login, $"{nameof(LoginRequest)}{Guid.NewGuid():N}");
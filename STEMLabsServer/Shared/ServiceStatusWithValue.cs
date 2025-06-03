namespace STEMLabsServer.Shared;

// Derives from ServiceStatus to additionally hold a value of type T
public class ServiceStatusWithValue<T>(bool success, string failureReason = "", T? value = default)
    : ServiceStatus(success, failureReason)
{
    public T? Value { get; set; } = value;
}
namespace STEMLabsServer.Shared;

public class ServiceStatusWithValue<T>(bool success, string failureReason = "", T? value = default)
    : ServiceStatus(success, failureReason)
{
    public T? Value { get; set; } = value;
}
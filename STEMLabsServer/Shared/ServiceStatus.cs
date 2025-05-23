namespace STEMLabsServer.Shared;

public class ServiceStatus(bool success, string failureReason = "")
{
    public bool Success { get; set; } = success;
    public string FailureReason { get; set; } = failureReason;
}
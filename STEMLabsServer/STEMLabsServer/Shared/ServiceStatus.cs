namespace STEMLabsServer.Shared;

// Class used to indicate the status of a service operation and optionally provide a failure reason.
public class ServiceStatus(bool success, string failureReason = "")
{
    public bool Success { get; set; } = success;
    public string FailureReason { get; set; } = failureReason;
}
namespace IKDFrontEnd.Interfaces
{
	public interface IErrorLogService
	{
		Task LogErrorAsync(string errorUrl, string? referrerUrl, string? ipAddress);
	}
}

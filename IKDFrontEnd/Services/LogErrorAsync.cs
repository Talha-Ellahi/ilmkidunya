using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Services
{
	public class ErrorLogService : IErrorLogService
	{
		private readonly DbCollegeContext _contextCollege;

		public ErrorLogService(DbCollegeContext contextCollege)
		{
			_contextCollege = contextCollege;
		}
		public async Task LogErrorAsync(string errorUrl, string? referrerUrl, string? ipAddress)
		{
			if (string.IsNullOrWhiteSpace(errorUrl))
				return;

			var error = await _contextCollege.ErrorLists
				.FirstOrDefaultAsync(x => x.ErrorUrl == errorUrl);

			if (error != null)
			{
				error.ErrorCount++;
				error.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
				// Optionally update referrer if you want the latest one
				if (!string.IsNullOrWhiteSpace(referrerUrl))
					error.ReferrerUrl = referrerUrl;
			}
			else
			{
				int nextId = (await _contextCollege.ErrorLists
					.MaxAsync(x => (int?)x.ErrorId) ?? 0) + 1;

				_contextCollege.ErrorLists.Add(new ErrorList
				{
					ErrorId = nextId,
					ErrorUrl = errorUrl,
					ReferrerUrl = referrerUrl,
					Ip = ipAddress,
					CreatedDate = DateOnly.FromDateTime(DateTime.Now),
					UpdatedDate = null,
					ErrorCount = 1
				});
			}

			await _contextCollege.SaveChangesAsync();
		}
	}
}

using IKDFrontEnd.BackupModel1;
using IKDFrontEnd.BackupModel2;
using IKDFrontEnd.BackupModel3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IKDFrontEnd.Services
{
    public class RandomCmsService
    {
        private readonly List<DbContext> _contexts;
        private readonly Random _random;
        private readonly ILogger<RandomCmsService> _logger;

        
        private static readonly Dictionary<string, DateTime> _failedDbs = new();

        public RandomCmsService(Dbikd1Context db1, Dbikd2Context db2, /*Dbikd4Context db4,Dbikd3Context db3,*/ ILogger<RandomCmsService> logger)
        {
            _logger = logger;
            _contexts = new List<DbContext> { db1, db2 };
            _random = new Random();
        }

        public DbContext? GetRandomContext()
        {
            // Filter out DBs that recently failed (retry after 2 minutes)
            var available = _contexts.Where(c =>
            {
                var name = c.GetType().Name;
                return !_failedDbs.ContainsKey(name) ||
                       (DateTime.Now - _failedDbs[name]).TotalMinutes > 2;
            }).ToList();

            if (!available.Any())
            {
                _logger.LogError("No available database connections. All servers failed recently.");
                return null;
            }

            int index = _random.Next(available.Count);
            return available[index];
        }

        public void MarkAsFailed(DbContext context, Exception ex)
        {
            string name = context.GetType().Name;
            _failedDbs[name] = DateTime.Now;
            _logger.LogError(ex, $"Database {name} marked as failed at {DateTime.Now}");
        }
    }
}

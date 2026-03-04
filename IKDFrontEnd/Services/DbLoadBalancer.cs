//using IKDFrontEnd.BackupModels;
//using IKDFrontEnd.Models;
//using Microsoft.EntityFrameworkCore;

//namespace IKDFrontEnd.Services
//{
//    public class DbLoadBalancer
//    {
//        private readonly DbikdContext _mainDb;
//        private readonly BackupDbContext _backupDb;
//        private static readonly Random _random = new Random();

//        public DbLoadBalancer(DbikdContext mainDb, BackupDbContext backupDb)
//        {
//            _mainDb = mainDb;
//            _backupDb = backupDb;
//        }

//        public DbContext GetDbContext()
//        {
//            // 50/50 random split
//            return _random.Next(2) == 0 ? _mainDb : _backupDb;
//        }
//    }

//}

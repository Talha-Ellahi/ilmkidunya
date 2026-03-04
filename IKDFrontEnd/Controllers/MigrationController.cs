//using IKDFrontEnd.BackupModel1;
//using IKDFrontEnd.BackupModel2;
//using IKDFrontEnd.BackupModel3;
//using IKDFrontEnd.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;


//namespace IKDApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MigrationController : ControllerBase
//    {
//        private readonly DbikdContext _context;

//        private readonly Dbikd2Context _backupContext1; // backup DB1
//        private readonly Dbikd3Context _backupContext2; // backup DB2
//        private readonly Dbikd4Context _backupContext3; // backup DB3
//        public MigrationController(DbikdContext context, Dbikd2Context backupContext1, Dbikd3Context backupContext2, Dbikd4Context backupContext3)
//        {
//            _context = context;
           
//            _backupContext3 = backupContext3;
//            _backupContext1 = backupContext1;
//            _backupContext2 = backupContext2;
//        }

//        [HttpPost("MigrateTblCm")]
//        public async Task<IActionResult> MigrateTblCm()
//        {
//            try
//            {
//                // Fetch source data from main DB
//                var sourceData = await _context.TblCms
//                    .AsNoTracking()

//                    .ToListAsync();

//                if (sourceData == null || !sourceData.Any())
//                    return BadRequest("No data found in source table.");

//                // Map to BackupModel1
//                //var backupData1 = sourceData.Select(x => new BackupModel1.TblCm
//                //{
//                //    Id = x.Id,
//                //    SectionId = x.SectionId,
//                //    PageName = x.PageName,
//                //    Url = x.Url,
//                //    Heading = x.Heading,
//                //    Desc1 = x.Desc1,
//                //    Desc2 = x.Desc2,
//                //    Desc3 = x.Desc3,
//                //    MetaTitle = x.MetaTitle,
//                //    MetaDesc = x.MetaDesc,
//                //    MetaKeys = x.MetaKeys,
//                //    Image = x.Image,
//                //    UserId = x.UserId,
//                //    Date = x.Date
//                //}).ToList();

//                //// Map to BackupModel2
//                //var backupData2 = sourceData.Select(x => new BackupModel2.TblCm
//                //{
//                //    Id = x.Id,
//                //    SectionId = x.SectionId,
//                //    PageName = x.PageName,
//                //    Url = x.Url,
//                //    Heading = x.Heading,
//                //    Desc1 = x.Desc1,
//                //    Desc2 = x.Desc2,
//                //    Desc3 = x.Desc3,
//                //    MetaTitle = x.MetaTitle,
//                //    MetaDesc = x.MetaDesc,
//                //    MetaKeys = x.MetaKeys,
//                //    Image = x.Image,
//                //    UserId = x.UserId,
//                //    Date = x.Date
//                //}).ToList();

//                // Map to BackupModel3
//                var backupData3 = sourceData.Select(x => new IKDFrontEnd.BackupModel3.TblCm
//                {
//                    Id = x.Id,
//                    SectionId = x.SectionId,
//                    PageName = x.PageName,
//                    Url = x.Url,
//                    Heading = x.Heading,
//                    Desc1 = x.Desc1,
//                    Desc2 = x.Desc2,
//                    Desc3 = x.Desc3,
//                    MetaTitle = x.MetaTitle,
//                    MetaDesc = x.MetaDesc,
//                    MetaKeys = x.MetaKeys,
//                    Image = x.Image,
//                    UserId = x.UserId,
//                    Date = x.Date
//                }).ToList();

//                if (/*!backupData1.Any() || !backupData2.Any() ||*/ !backupData3.Any())
//                    return BadRequest("No records to migrate.");

//                // --- Save to Backup DB1 ---
//                //using (var transaction1 = await _backupContext1.Database.BeginTransactionAsync())
//                //{
//                //    try
//                //    {
//                //        await _backupContext1.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT TblCms ON");
//                //        foreach (var batch in backupData1.Chunk(500))
//                //        {
//                //            await _backupContext1.TblCms.AddRangeAsync(batch);
//                //            await _backupContext1.SaveChangesAsync();
//                //        }
//                //        await _backupContext1.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT TblCms OFF");
//                //        await transaction1.CommitAsync();
//                //    }
//                //    catch (Exception ex)
//                //    {
//                //        await transaction1.RollbackAsync();
//                //        return BadRequest($"Migration failed for Backup DB1: {ex.InnerException?.Message ?? ex.Message}");
//                //    }
//                //}

//                // --- Save to Backup DB2 ---
//                //using (var transaction2 = await _backupContext2.Database.BeginTransactionAsync())
//                //{
//                //    try
//                //    {
//                //        await _backupContext2.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT TblCms ON");
//                //        foreach (var batch in backupData2.Chunk(500))
//                //        {
//                //            await _backupContext2.TblCms.AddRangeAsync(batch);
//                //            await _backupContext2.SaveChangesAsync();
//                //        }
//                //        await _backupContext2.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT TblCms OFF");
//                //        await transaction2.CommitAsync();
//                //    }
//                //    catch (Exception ex)
//                //    {
//                //        await transaction2.RollbackAsync();
//                //        return BadRequest($"Migration failed for Backup DB2: {ex.InnerException?.Message ?? ex.Message}");
//                //    }
//                //}

//                // --- Save to Backup DB3 ---
//                using (var transaction3 = await _backupContext3.Database.BeginTransactionAsync())
//                {
//                    try
//                    {
//                        await _backupContext3.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT TblCms ON");
//                        foreach (var batch in backupData3.Chunk(500))
//                        {
//                            await _backupContext3.TblCms.AddRangeAsync(batch);
//                            await _backupContext3.SaveChangesAsync();
//                        }
//                        await _backupContext3.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT TblCms OFF");
//                        await transaction3.CommitAsync();
//                    }
//                    catch (Exception ex)
//                    {
//                        await transaction3.RollbackAsync();
//                        return BadRequest($"Migration failed for Backup DB3: {ex.InnerException?.Message ?? ex.Message}");
//                    }
//                }

//                return Ok($"Migration successful. Total records migrated: {sourceData.Count} into all 3 backup databases.");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"Migration failed: {ex.InnerException?.Message ?? ex.Message}");
//            }
//        }

       


//    }
//}

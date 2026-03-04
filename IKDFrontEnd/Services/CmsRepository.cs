using Microsoft.EntityFrameworkCore;
using IKDFrontEnd.ViewModels.Common;
using IKDFrontEnd.BackupModel1;
using IKDFrontEnd.BackupModel2;
using IKDFrontEnd.BackupModel3;


namespace IKDFrontEnd.Services
{
    public class CmsRepository
    {
        private readonly RandomCmsService _randomCms;

        public CmsRepository(RandomCmsService randomCms)
        {
            _randomCms = randomCms;
        }

        public async Task<TblCmsDto?> GetByUrlAsync(string url)
        {
            if (!url.StartsWith("https://www.ilmkidunya.com", StringComparison.OrdinalIgnoreCase))
            {
                if (!url.StartsWith("/"))
                    url = "/" + url;

                url = "https://www.ilmkidunya.com" + url;
            }

            var triedContexts = new HashSet<string>();

            for (int i = 0; i < 3; i++)
            {
                var context = _randomCms.GetRandomContext();
                if (context == null) break;

                var name = context.GetType().Name;
                if (triedContexts.Contains(name))
                    continue; // already tried this one

                triedContexts.Add(name);

                try
                {
                    dynamic? data = context switch
                    {
                        Dbikd2Context db2 => await db2.TblCms.FirstOrDefaultAsync(x => x.Url.Trim() == url),
                        Dbikd3Context db3 => await db3.TblCms.FirstOrDefaultAsync(x => x.Url.Trim() == url),
                        Dbikd4Context db4 => await db4.TblCms.FirstOrDefaultAsync(x => x.Url.Trim() == url),
                        _ => null
                    };

                    return data == null ? null : MapToDto(data);
                }
                catch (Exception ex)
                {
                    _randomCms.MarkAsFailed(context, ex);
                }
            }

            return null;
        }

        public async Task<List<TblCmsDto>> GetListByUrlAsync(string url)
        {
            if (!url.StartsWith("https://www.ilmkidunya.com", StringComparison.OrdinalIgnoreCase))
                url = "https://www.ilmkidunya.com/" + url.TrimStart('/');

            var resultList = new List<TblCmsDto>();
            var triedContexts = new HashSet<string>();

            for (int i = 0; i < 3; i++)
            {
                var context = _randomCms.GetRandomContext();
                if (context == null) break;

                var name = context.GetType().Name;
                if (triedContexts.Contains(name))
                    continue;

                triedContexts.Add(name);

                try
                {
                    dynamic? dataList = context switch
                    {
                        Dbikd2Context db2 => await db2.TblCms
                            .Where(x => x.Url.Contains(url))
                            .ToListAsync(),
                        Dbikd3Context db3 => await db3.TblCms
                            .Where(x => x.Url.Contains(url))
                            .ToListAsync(),
                        Dbikd4Context db4 => await db4.TblCms
                            .Where(x => x.Url.Contains(url))
                            .ToListAsync(),
                        _ => null
                    };

                    if (dataList != null && dataList.Count > 0)
                    {
                        foreach (var item in (IEnumerable<dynamic>)dataList)
                        {
                            resultList.Add(MapToDto(item));
                        }

                        // ✅ Stop after first database that returns results
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _randomCms.MarkAsFailed(context, ex);
                }
            }

            return resultList;
        }




        public async Task<List<TblCmsDto>> SearchAsync(string keyword)
        {
            var context = _randomCms.GetRandomContext();

            if (context is Dbikd2Context db2)
                return await db2.TblCms
                    .Where(x => x.Heading.Contains(keyword))
                    .Select(x => MapToDto(x))
                    .ToListAsync();

            if (context is Dbikd3Context db3)
                return await db3.TblCms
                    .Where(x => x.Heading.Contains(keyword))
                    .Select(x => MapToDto(x))
                    .ToListAsync();

            if (context is Dbikd4Context db4)
                return await db4.TblCms
                    .Where(x => x.Heading.Contains(keyword))
                    .Select(x => MapToDto(x))
                    .ToListAsync();

            return new List<TblCmsDto>();
        }

        private static TblCmsDto MapToDto(dynamic cms)
        {
            return new TblCmsDto
            {
                Id = cms.Id,
                Heading = cms.Heading,
                MetaTitle = cms.MetaTitle,
                MetaDesc = cms.MetaDesc,
                MetaKeys = cms.MetaKeys,
                Desc1 = cms.Desc1,
                Desc2 = cms.Desc2,
                Desc3 = cms.Desc3,
                Url = cms.Url,
                PageName = cms.PageName,
                Image = cms.Image,
            };
        }
    }
}

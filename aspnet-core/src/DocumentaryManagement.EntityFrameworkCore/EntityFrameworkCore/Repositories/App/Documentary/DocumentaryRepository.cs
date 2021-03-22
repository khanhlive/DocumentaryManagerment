using Abp.EntityFrameworkCore;
using Abp.Runtime.Session;
using Abp.UI;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DocumentaryManagement.Authorization.Users;
using DocumentaryManagement.EntityFrameworkCore.Repositories.App.Documentary.Models;
using DocumentaryManagement.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentaryManagement.EntityFrameworkCore.Repositories.App.Documentary
{
    public class DocumentaryRepository : DocumentaryManagementRepositoryBase<AppDocumentary>, IDocumentaryRepository
    {
        public DocumentaryRepository(IDbContextProvider<DocumentaryManagementDbContext> dbContextProvider, IConfiguration configuration, IAbpSession abpSession)
            : base(dbContextProvider, configuration, abpSession)
        {

        }

        protected override IQueryable<AppDocumentary> SetEntityIncludes(IQueryable<AppDocumentary> entities)
        {
            return entities.Where(p => !p.IsDeleted);//.Include(p => p.DocumentType).Include(p => p.AgencyIssued);
        }

        public override void Before_InsertUpdate(AppDocumentary entity)
        {
            AppDocumentary item = this.FirstOrDefault(p => p.Code == entity.Code && p.Id != entity.Id);
            if (entity.Id == 0)
            {
                entity.CreationDate = DateTime.Now;
                if (AbpSession.UserId != null) entity.CreationId = AbpSession.UserId.Value;
            }
            else
            {
                entity.UpdatedDate = DateTime.Now;
                if (AbpSession.UserId != null) entity.UpdatedId = AbpSession.UserId.Value;
            }
            if (item != null && entity.ReleaseDate.Year == item.ReleaseDate.Year)
            {
                throw new UserFriendlyException($"Ký hiệu văn bản: \"{entity.Code}\" đã tồn tại trong hệ thống");
            }
        }

        public List<AppDocumentary> GetFilterReportData(DocumentFilterOptions documentFilterOptions, Authorization.PermissionType permissionType, long userId)
        {
            DocumentaryManagementDbContext DbContext = this.GetDevContext();
            return this.MapName(DbContext, this.GetQueryFilter(DbContext, documentFilterOptions, permissionType, userId)).ToList();
        }

        private IQueryable<AppDocumentary> GetQueryFilter(DocumentaryManagementDbContext DbContext, DocumentFilterOptions documentFilterOptions, Authorization.PermissionType permissionType, long userId)
        {
            //DocumentaryManagementDbContext DbContext = this.GetDevContext();
            var query = DbContext.Set<AppDocumentary>().Where(p => p.IsDeleted == false);
            User user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            int? departmentId = user == null ? 0 : user.DepartmentId;
            if (permissionType == Authorization.PermissionType.Admin || permissionType == Authorization.PermissionType.DocumentManager)
            {

            }
            else if (permissionType == Authorization.PermissionType.Approved)
            {
                query = (from a in query
                         join b in DbContext.AppRotation.Where(p => p.UserId == null ? p.DepartmentId == departmentId : p.UserId == userId) on a.Id equals b.DocumentId into kq
                         where (a.ApprovedType == 1 ? a.ApprovedUserId == userId : a.ApprovedDepartmentId == departmentId) || (kq.Any())
                         select a
                        );
            }
            else
            {
                query = (from a in query
                         join b in DbContext.AppRotation.Where(p => p.UserId == null ? p.DepartmentId == departmentId : p.UserId == userId) on a.Id equals b.DocumentId into kq
                         where (kq.Any())
                         select a
                        );
            }

            if (documentFilterOptions != null)
            {
                query = query.Where(p => p.Type == documentFilterOptions.Type);
                if (documentFilterOptions.Keyword != null && !documentFilterOptions.Keyword.Trim().Equals(""))
                {
                    if (documentFilterOptions.FilterBy == 1)
                    {
                        query = query.Where(p => documentFilterOptions.Exactly ? p.Code.Equals(documentFilterOptions.Keyword.Trim()) : p.Code.Contains(documentFilterOptions.Keyword));
                    }
                    else
                    {
                        query = query.Where(p => documentFilterOptions.Exactly ? p.SummaryContent.Equals(documentFilterOptions.Keyword.Trim()) : p.SummaryContent.Contains(documentFilterOptions.Keyword));
                    }
                }
                if (documentFilterOptions.Approved != 0)
                {
                    query = query.Where(p => documentFilterOptions.Approved == 1 ? p.IsApproved == true : (p.IsApproved == false || p.IsApproved == null));
                }

                query = from a in query
                    where (documentFilterOptions.Year == null || documentFilterOptions.Year == -1 ||
                           a.ReleaseDate.Year == documentFilterOptions.Year)
                    select a;
            }

            return SetEntityIncludes(query);
        }

        public virtual LoadResult GetDevExtreme(DataSourceLoadOptionsBase loadOptions, DocumentFilterOptions documentFilterOptions, Authorization.PermissionType permissionType)
        {
            DocumentaryManagementDbContext DbContext = this.GetDevContext();
            var query = GetQueryFilter(DbContext, documentFilterOptions, permissionType, AbpSession.UserId ?? 0);
            var query2 = (from a in query
                          join b in DbContext.Users on a.ApprovedUserId equals b.Id into kq
                          from u in kq.DefaultIfEmpty()
                          join c in DbContext.AppDepartment on a.ApprovedDepartmentId equals c.Id into kq1
                          from d in kq1.DefaultIfEmpty()
                          join doc in DbContext.AppDocumentType on a.DocumentTypeId equals doc.Id into kq2
                          from doc1 in kq2.DefaultIfEmpty()
                          join agen in DbContext.AppAgencyIssued on a.AgencyIssuedId equals agen.Id into kq3
                          from agen1 in kq3.DefaultIfEmpty()
                          where (documentFilterOptions.Year == null || documentFilterOptions.Year == -1 || a.ReleaseDate.Year == documentFilterOptions.Year)
                          orderby a.ReleaseDate.Year descending, a.PrefixNumber, a.SuffixNumber
                          select new
                          {
                              a,
                              IsView = (permissionType != Authorization.PermissionType.Admin && permissionType != Authorization.PermissionType.DocumentManager) && DbContext.AppRotation.Any(p => p.DocumentId == a.Id && p.UserId == AbpSession.UserId && p.IsView == true),
                              UserName = u == null ? null : u.FullName2,
                              DepartmentName = d == null ? null : d.Name,
                              DocTypeName = doc1 == null ? null : doc1.Name,
                              AgencyName = agen1 == null ? null : agen1.Name,
                          }
                       ).AsEnumerable().Select(p =>
                       {
                           var item = p.a;
                           item.ApprovedUserId_Name = p.UserName;
                           item.ApprovedDepartmentId_Name = p.DepartmentName;
                           item.IsView = p.IsView;
                           item.DocumentTypeId_Name = p.DocTypeName;
                           item.AgencyIssuedId_Name = p.AgencyName;
                           return item;
                       });
            return DataSourceLoader.Load(query2, loadOptions);
        }

        public List<AppDocumentary> GetBookReportData(DocumentFilterOptions documentFilterOptions)
        {
            DocumentaryManagementDbContext DbContext = this.GetDevContext();
            return MapName(DbContext, GetBookQuery(DbContext, documentFilterOptions)).ToList();
        }

        private IEnumerable<AppDocumentary> MapName(DocumentaryManagementDbContext DbContext, IQueryable<AppDocumentary> documentaries)
        {
            var query2 = (from a in documentaries
                          join doc in DbContext.AppDocumentType on a.DocumentTypeId equals doc.Id into kq2
                          from doc1 in kq2.DefaultIfEmpty()
                          join agen in DbContext.AppAgencyIssued on a.AgencyIssuedId equals agen.Id into kq3
                          from agen1 in kq3.DefaultIfEmpty()
                          select new
                          {
                              a,
                              DocTypeName = doc1 == null ? null : doc1.Name,
                              AgencyName = agen1 == null ? null : agen1.Name,
                          }
                       ).AsEnumerable().Select(p =>
                       {
                           var item = p.a;
                           item.DocumentTypeId_Name = p.DocTypeName;
                           item.AgencyIssuedId_Name = p.AgencyName;
                           return item;
                       });
            return query2;
        }

        private IQueryable<AppDocumentary> GetBookQuery(DocumentaryManagementDbContext DbContext, DocumentFilterOptions documentFilterOptions)
        {
            var query = DbContext.Set<AppDocumentary>().Where(p => p.IsDeleted == false);
            if (documentFilterOptions != null)
            {
                query = (from a in query.Where(p => p.Type == documentFilterOptions.Type)
                         where (documentFilterOptions.LoaiVanBan == null ||
                                a.DocumentTypeId == documentFilterOptions.LoaiVanBan)
                         where (documentFilterOptions.NgayTuDate == null ||
                                a.ReleaseDate >= documentFilterOptions.NgayTuDate)
                         where (documentFilterOptions.NgayDenDate == null ||
                                a.ReleaseDate <= documentFilterOptions.NgayDenDate)
                         where (documentFilterOptions.Year == null || documentFilterOptions.Year == -1 || a.ReleaseDate.Year == documentFilterOptions.Year)
                         orderby a.ReleaseDate.Year descending, a.PrefixNumber, a.SuffixNumber
                         select a);

            }
            return SetEntityIncludes(query);
        }

        public virtual LoadResult GetBookDevExtreme(DataSourceLoadOptionsBase loadOptions, DocumentFilterOptions documentFilterOptions)
        {
            DocumentaryManagementDbContext DbContext = this.GetDevContext();
            return DataSourceLoader.Load(MapName(DbContext, GetBookQuery(DbContext, documentFilterOptions)), loadOptions);
        }

        public List<AppDocumentary> GetSearchReportData(DocumentSearchOptions searchOptions, Authorization.PermissionType permissionType, long userId)
        {
            DocumentaryManagementDbContext DbContext = this.GetDevContext();
            return this.GetQuerySearch(DbContext, searchOptions, permissionType, userId).ToList();
        }

        private IQueryable<AppDocumentary> GetQuerySearch(DocumentaryManagementDbContext DbContext, DocumentSearchOptions searchOptions, Authorization.PermissionType permissionType, long userId)
        {
            var query = DbContext.Set<AppDocumentary>().Where(p => p.IsDeleted == false);
            User user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            int? departmentId = user == null ? 0 : user.DepartmentId;
            if (permissionType == Authorization.PermissionType.Admin || permissionType == Authorization.PermissionType.DocumentManager)
            {

            }
            else if (permissionType == Authorization.PermissionType.Approved)
            {
                query = (from a in query
                         join b in DbContext.AppRotation.Where(p =>
                             p.UserId == null ? p.DepartmentId == departmentId : p.UserId == userId) on a.Id equals b
                             .DocumentId into kq
                         where (a.ApprovedType == 1
                             ? a.ApprovedUserId == userId
                             : a.ApprovedDepartmentId == departmentId) || (kq.Any())

                         orderby a.ReleaseDate.Year descending, a.PrefixNumber, a.SuffixNumber
                         select a
                    );
            }
            else
            {
                query = (from a in query
                         join b in DbContext.AppRotation.Where(p =>
                             p.UserId == null ? p.DepartmentId == departmentId : p.UserId == userId) on a.Id equals b
                             .DocumentId into kq
                         where (kq.Any())
                         orderby a.ReleaseDate.Year descending, a.PrefixNumber, a.SuffixNumber
                         select a
                    );
            }
            if (searchOptions != null)
            {
                query = (from doc in query.Where(p => p.Type == searchOptions.Type)
                         where (searchOptions.Code == null || searchOptions.Code == string.Empty) ||
                               doc.Code.ToLower().Contains(searchOptions.Code.ToLower())
                         where (searchOptions.NoiDungTomTat == null || searchOptions.NoiDungTomTat == string.Empty) ||
                               doc.SummaryContent.ToLower().Contains(searchOptions.NoiDungTomTat.ToLower())
                         where ((!searchOptions.NgayBanHanhTu.HasValue ||
                                 doc.ReleaseDate >= searchOptions.NgayBanHanhTu) &&
                                (!searchOptions.NgayBanHanhDen.HasValue ||
                                 doc.ReleaseDate <= searchOptions.NgayBanHanhDen))
                         where ((!searchOptions.NgayGuiTu.HasValue || doc.ReceivedDate >= searchOptions.NgayGuiTu) &&
                                (!searchOptions.NgayGuiDen.HasValue || doc.ReceivedDate <= searchOptions.NgayGuiDen))
                         where (searchOptions.Year == null || searchOptions.Year == -1 || doc.ReleaseDate.Year == searchOptions.Year)
                         orderby doc.ReleaseDate.Year descending, doc.PrefixNumber, doc.SuffixNumber
                         select doc
                    );
                if (!string.IsNullOrEmpty(searchOptions.NoiBanHanh))
                {
                    query = (from doc in query
                             join pl in DbContext.AppAgencyIssued
                                 .Where(p => p.Name.ToLower().Contains(searchOptions.NoiBanHanh))
                                 .Select(p => p.Id) on doc.AgencyIssuedId equals pl
                             orderby doc.ReleaseDate.Year descending, doc.PrefixNumber, doc.SuffixNumber
                             select doc
                        );
                }
            }
            return SetEntityIncludes(query);
        }

        public LoadResult GetSearchDevExtreme(DataSourceLoadOptionsBase loadOptions, DocumentSearchOptions searchOptions, Authorization.PermissionType permissionType)
        {
            DocumentaryManagementDbContext DbContext = this.GetDevContext();
            return DataSourceLoader.Load(MapName(DbContext, this.GetQuerySearch(DbContext, searchOptions, permissionType, AbpSession.UserId ?? 0)), loadOptions);
        }

        public async Task<List<User>> GetUserApproved()
        {
            return await Context.Users.ToListAsync();
        }

        public override void Delete(AppDocumentary entity)
        {
            if (entity.IsApproved == true)
            {
                throw new UserFriendlyException("Văn bản này đã được duyệt, không được phép xóa");
            }
            base.Delete(entity);
        }

    }
}

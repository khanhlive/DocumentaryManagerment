using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DocumentaryManagement.Model;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DocumentaryManagement.Documentary.Dto
{
    [AutoMapFrom(typeof(AppDocumentary))]
    public class DocumentaryDto : EntityDto
    {
        public DocumentaryDto()
        {
            AppAttachments = new HashSet<AppAttachments>();
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string TextNumber { get; set; }
        [JsonIgnore]
        public int? PrefixNumber { get; set; }
        [JsonIgnore]
        public string SuffixNumber { get; set; }
        public string Signer { get; set; }
        public string ApprovedBy { get; set; }
        public string ReceivedBy { get; set; }
        public int DocumentTypeId { get; set; }
        public int AgencyIssuedId { get; set; }
        public int TotalPage { get; set; }
        public bool IsProcessed { get; set; }
        public string CategoryName { get; set; }
        public string PerformancePerson { get; set; }
        public string Description { get; set; }
        public string SummaryContent { get; set; }
        public string Content { get; set; }
        public int? Type { get; set; }
        public long CreationId { get; set; }
        public DateTime CreationDate { get; set; }
        public long? UpdatedId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsApproved { get; set; }
        public int? ApprovedDepartmentId { get; set; }
        public long? ApprovedUserId { get; set; }
        public int? ApprovedType { get; set; }
        public string ApprovedContent { get; set; }
        public string ApprovedUserId_Name { get; set; }
        public string ApprovedDepartmentId_Name { get; set; }

        public virtual AppAgencyIssued AgencyIssued { get; set; }
        public virtual AppDocumentType DocumentType { get; set; }
        public virtual ICollection<AppAttachments> AppAttachments { get; set; }
    }
}

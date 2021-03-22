using Abp.AutoMapper;
using DocumentaryManagement.Attachment.Dto;
using DocumentaryManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DocumentaryManagement.Documentary.Dto
{
    [AutoMapTo(typeof(AppDocumentary))]
    public class CreateDocumentaryDto
    {
        public CreateDocumentaryDto()
        {
            AppAttachments = new HashSet<UpdateAttachmentDto>();
        }
        public string Code { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public DateTime ReleaseDate { get; set; }
        [JsonIgnore]
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
        [JsonIgnore]
        public long CreationId { get; set; }
        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        public int? ApprovedDepartmentId { get; set; }
        public long? ApprovedUserId { get; set; }
        public int? ApprovedType { get; set; }


        [JsonProperty("releaseDate")]
        public string ReleaseDateSync
        {
            get
            {
                return string.Empty;
            }
            set
            {
                if (DateTime.TryParseExact(value, new string[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _date))
                {
                    this.ReleaseDate = _date;
                }
            }
        }
        [JsonProperty("receivedDate")]
        public string ReceivedDateSync {
            get {
                return string.Empty;
            }
            set
            {
                if (DateTime.TryParseExact(value, new string[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _date))
                {
                    this.ReceivedDate = _date;
                }
            }
        }

        public virtual ICollection<UpdateAttachmentDto> AppAttachments { get; set; }
    }
}

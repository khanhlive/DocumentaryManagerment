using System;
using System.Globalization;

namespace DocumentaryManagement.EntityFrameworkCore.Repositories.App.Documentary.Models
{
    public class DocumentFilterOptions
    {
        public string Keyword { get; set; }
        public int FilterBy { get; set; }
        public bool Exactly { get; set; }
        public int Type { get; set; }
        public int? Year { get; set; }
        public int Approved { get; set; }
        public string NgayTu
        {
            get
            {
                return string.Empty;
            }
            set
            {
                if (DateTime.TryParseExact(value, new string[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _date))
                {
                    this.NgayTuDate = _date;
                }
            }
        }
        public string NgayDen
        {
            get
            {
                return string.Empty;
            }
            set
            {
                if (DateTime.TryParseExact(value, new string[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _date))
                {
                    this.NgayDenDate = _date;
                }
            }
        }
        public DateTime? NgayTuDate { get; set; }
        public DateTime? NgayDenDate { get; set; }
        public int? LoaiVanBan { get; set; }
    }
}

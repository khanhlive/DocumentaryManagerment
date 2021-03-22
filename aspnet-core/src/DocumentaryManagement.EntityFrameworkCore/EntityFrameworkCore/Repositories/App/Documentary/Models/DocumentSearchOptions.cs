using System;

namespace DocumentaryManagement.EntityFrameworkCore.Repositories.App.Documentary.Models
{
    public class DocumentSearchOptions
    {
        public string Code { get; set; }
        public string NoiBanHanh { get; set; }
        public string NoiDungTomTat { get; set; }
        public DateTime? NgayBanHanhTu { get; set; }
        public DateTime? NgayBanHanhDen { get; set; }
        public DateTime? NgayGuiTu { get; set; }
        public DateTime? NgayGuiDen { get; set; }
        public int Type { get; set; }
        public int? Year { get; set; }
    }
}

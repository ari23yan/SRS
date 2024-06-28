using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos
{
    public class DoctorDto
    {
        public int? ST { get; set; }
        public int? Active { get; set; }
        public string? NP { get; set; }
        public string? FP { get; set; }
        public string? NoNezam { get; set; }
        public string? NameP { get; set; }
        public string? Name { get; set; }
        public int? CodeJ { get; set; }
        public string? OutpAdmitPer { get; set; }
        public string? Takhsos { get; set; }
        public string? TakhsosKoli { get; set; }
        public int? CodeFarei { get; set; }
        public string? VisitTar { get; set; }
        public string? AccNezam { get; set; }
        public int? GroupCode { get; set; }
        public string? DoctorNationalCode { get; set; }
        public string? Mobile1 { get; set; }
        public string? Perscode { get; set; }
        public string? NationalTariffCode { get; set; }
    }
}

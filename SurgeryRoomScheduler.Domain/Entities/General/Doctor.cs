using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("Doctors", Schema = "General")]

    public class Doctor
    {
        public long? Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(128)]

        public string? LastName { get; set; }
        [MaxLength(256)]

        public string? FullName { get; set; }
        [MaxLength(30)]

        public string? NoNezam { get; set; }
        [MaxLength(20)]

        public string? NationalCode { get; set; }
        [MaxLength(20)]

        public string? PhoneNumber { get; set; }
        [MaxLength(500)]

        public string? Specialty { get; set; }
        [MaxLength(500)]
        public string? GeneralSpecialty { get; set; }

        public bool? IsActive { get; set; }

        public int? Code { get; set; }

        public int? CodeFarei { get; set; }
        [MaxLength(256)]

        public string? NationalTariffCode { get; set; }

        public int? GroupCode { get; set; }
        public int? PersonnelCode { get; set; }
    }
}

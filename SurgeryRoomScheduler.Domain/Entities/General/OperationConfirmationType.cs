using SurgeryRoomScheduler.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("OperationConfirmationTypes", Schema = "General")]

    public class OperationConfirmationType: BaseEntity
    {
        [MaxLength(100)]

        public string Name { get; set; }
        [MaxLength(200)]

        public string? Description { get; set; }
    }
}

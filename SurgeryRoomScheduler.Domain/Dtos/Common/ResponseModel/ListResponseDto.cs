using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel
{
    public class ListResponseDto<T>
    {
        public IEnumerable<T> List { get; set; }
        public int TotalCount { get; set; }
    }
}

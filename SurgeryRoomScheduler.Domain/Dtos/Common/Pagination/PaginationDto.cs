using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;



namespace SurgeryRoomScheduler.Domain.Dtos.Common.Pagination
{
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Searchkey { get; set; }
        [EnumDataType(typeof(FilterType))]

        public FilterType? FilterType { get; set; }
    }
}

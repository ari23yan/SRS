﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel
{
    public class ResponseDto<T>
    {
        public string? Message { get; set; }
        public bool? IsSuccessFull { get; set; }
        public T? Data { get; set; }
        public string? Status { get; set; }
        public int? TotalCount { get; set; }
    }
}

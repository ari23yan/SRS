﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Jobs
{
    public class JwtTokenHandler
    {
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsTokenValid() => !string.IsNullOrEmpty(Token) && ExpiryTime > DateTime.UtcNow;
    }
}

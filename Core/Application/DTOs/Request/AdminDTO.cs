using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Request {
    public class AdminDTO {
        public string fullname { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}

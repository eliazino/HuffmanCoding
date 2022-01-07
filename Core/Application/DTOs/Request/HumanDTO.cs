using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Request {
    public class HumanDTO {
        public string fullname { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public long creator { get; set; }
        public string height { get; set; }
        public string dob { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Request {
    public class LogsDTO {
        public long id { get; set; }
        public long logClass { get; set; }
        public long humanID { get; set; }
        public string details { get; set; }
        public long logTime { get; set; }
    }
}

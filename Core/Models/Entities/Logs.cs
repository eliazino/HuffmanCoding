using Core.Application.DTOs.Request;
using Core.Exceptions;
using Core.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Entities {
    public class Logs : BaseEntity {
        public long id { get; private set; }
        public long logClass { get; private set; }
        public long humanID { get; private set; }
        public string details { get; private set; }
        public long logTime { get; private set; }
        public Logs() {}
        public Logs(LogsDTO data) {
            if (isNullOrEmpty(data.details))
                throw new InputError("Invalid request. Details is required");
            if(data.logClass == default(long) || data.humanID == default(long))
                throw new InputError("Invalid request. Details is required");
            this.logClass = data.logClass;
            this.humanID = data.humanID;
            this.details = data.details;
            this.logTime = Utilities.getTodayDate().unixTimestamp;
        }
    }
}

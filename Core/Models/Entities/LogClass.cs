using Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Entities {
    public class LogClass : BaseEntity {
        public long id { get; private set; }
        public string name { get; private set; }
        public LogClass() { }
        public LogClass(string name) {
            if (isNullOrEmpty(name))
                throw new InputError("Invalid name of category");
            this.name = name;
        }
    }
}

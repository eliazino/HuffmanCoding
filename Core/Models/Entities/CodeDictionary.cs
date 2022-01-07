using Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Entities {
    public class CodeDictionary : BaseEntity {
        public long id { get; private set; }
        public long humanID { get; private set; }
        public string dictionary { get; private set; }
        public CodeDictionary() { }
        public CodeDictionary(string dictionary, long humanID) {
            if (isNullOrEmpty(dictionary))
                throw new LogicError("Dictionary cannot be empty");
            if (humanID == default(long))
                throw new LogicError("HumanID cannot be empty");
            this.humanID = humanID;
            this.dictionary = dictionary;
        }
    }
}

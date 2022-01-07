using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Filter {
    public class AdminFilter : Filter {
        public string username {
            get {
                string thisName = GetCallerName();
                return (string)getField(thisName);
            }
            set {
                string thisName = GetCallerName();
                if (fieldIsSet(thisName)) {
                    reference[thisName] = value;
                } else {
                    reference.Add(thisName, value);
                }
            }
        }
        public string password {
            get {
                string thisName = GetCallerName();
                return (string)getField(thisName);
            }
            set {
                string thisName = GetCallerName();
                if (fieldIsSet(thisName)) {
                    reference[thisName] = value;
                } else {
                    reference.Add(thisName, value);
                }
            }
        }
        public long id {
            get {
                string thisName = GetCallerName();
                return (long)getField(thisName);
            }
            set {
                string thisName = GetCallerName();
                if (fieldIsSet(thisName)) {
                    reference[thisName] = value;
                } else {
                    reference.Add(thisName, value);
                }
            }
        }
    }
}

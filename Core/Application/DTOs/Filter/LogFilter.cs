using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Filter {
    public class LogFilter : Filter {
        public long humanID {
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
        public long category {
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

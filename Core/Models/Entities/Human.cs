using Core.Application.DTOs.Request;
using Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Entities {
    public class Human : BaseEntity {
        public long id { get; private set; }
        public string fullname { get; private set; }
        public string gender { get; private set; }
        public string email { get; private set; }
        public string phone { get; private set; }
        public long creator { get; private set; }
        public string height { get; private set; }
        public string dob { get; private set; }
        public Human(HumanDTO data) {
            if (isNullOrEmpty(data.fullname, data.gender, data.email, data.phone, data.height, data.dob))
                throw new InputError("All fields are required!");
            this.fullname = data.fullname;
            this.gender = data.gender;
            this.email = data.email;
            this.phone = data.phone;
            this.creator = data.creator;
            this.height = data.height;
            this.dob = data.dob;
        }
        public Human() { }
    }
}

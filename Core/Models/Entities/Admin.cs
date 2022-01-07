using Core.Application.DTOs.Request;
using Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Entities {
    public class Admin : BaseEntity {
        public long id { get; private set; }
        public string fullname { get; private set; }
        public string gender { get; private set; }
        public string email { get; private set; }
        public string phone { get; private set; }
        public string username { get; private set; }
        public string password { get; private set; }
        public Admin(AdminDTO data) {
            if (isNullOrEmpty(data.fullname, data.gender, data.email, data.phone, data.username, data.password))
                throw new InputError("All fields are required!");
            this.fullname = data.fullname;
            this.gender = data.gender;
            this.email = data.email;
            this.phone = data.phone;
            this.username = data.username;
            this.password = data.password;
        }
        public Admin() { }
        public void update() {
            if(id == default(long)){
                throw new LogicError("Invalid Profile!");
            }

        }
    }
}

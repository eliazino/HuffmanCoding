using Core.Application.DTOs.Request;
using Core.Application.Interfaces.UseCases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Services.UseCases {
    public class AdminUseCase : IAdminUseCase {
        public Task<object> createAdmin(AdminDTO data) {
            throw new NotImplementedException();
        }

        public Task<object> createHuman(HumanDTO data) {
            throw new NotImplementedException();
        }

        public Task<object> createLogCategories(string name) {
            throw new NotImplementedException();
        }

        public Task<object> getHuman(long id) {
            throw new NotImplementedException();
        }

        public Task<object> getHumanActivities(long humanID) {
            throw new NotImplementedException();
        }

        public Task<object> getHumanDictionary(long humanID) {
            throw new NotImplementedException();
        }

        public Task<object> getLogCategories() {
            throw new NotImplementedException();
        }

        public Task<object> logHumanActivity(LogsDTO activity) {
            throw new NotImplementedException();
        }

        public Task<object> logHumanDictionary(long HumanID, string dictionary) {
            throw new NotImplementedException();
        }

        public Task<object> login(string username, string password) {
            throw new NotImplementedException();
        }
    }
}

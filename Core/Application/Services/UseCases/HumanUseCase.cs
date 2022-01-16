using Core.Application.DTOs.Request;
using Core.Application.Interfaces.UseCases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Services.UseCases {
    public class HumanUseCase : IHumanUseCase {
        public Task<object> createHuman(HumanDTO data) {
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

        public Task<object> logHumanActivity(LogsDTO activity) {
            throw new NotImplementedException();
        }

        public Task<object> logHumanDictionary(long HumanID, string dictionary) {
            throw new NotImplementedException();
        }
    }
}

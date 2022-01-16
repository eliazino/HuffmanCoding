using Core.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.UseCases {
    public interface IHumanUseCase {
        Task<object> createHuman(HumanDTO data);
        Task<object> getHuman(long id);
        Task<object> logHumanActivity(LogsDTO activity);
        Task<object> getHumanActivities(long humanID);
        Task<object> logHumanDictionary(long HumanID, string dictionary);
        Task<object> getHumanDictionary(long humanID);
    }
}

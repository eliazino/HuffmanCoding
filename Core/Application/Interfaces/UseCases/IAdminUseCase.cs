using Core.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.UseCases {
    public interface IAdminUseCase {
        Task<object> login(string username, string password);
        Task<object> createAdmin(AdminDTO data);
        Task<object> getLogCategories();
        Task<object> createLogCategories(string name);
    }
}

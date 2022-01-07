using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IAdminRepository {
        Task<bool> createAdmin(Admin data);
        Task<bool> updateAdmin(Admin data);
        Task<List<Admin>> getAdmin(AdminFilter data);
    }
}

using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface ILogClassRepository {
        Task<bool> create(LogClass data);
        Task<bool> update(LogClass data);
        Task<List<LogClass>> get(long id);
        Task<List<LogClass>> get();
    }
}

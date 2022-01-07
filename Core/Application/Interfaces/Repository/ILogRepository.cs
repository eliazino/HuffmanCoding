using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface ILogRepository {
        Task<bool> create(Logs dictionary);
        Task<bool> update(Logs data);
        Task<List<Logs>> get(LogFilter filter);
    }
}

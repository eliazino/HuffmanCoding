using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IHumanRepository {
        Task<bool> create(Human dictionary);
        Task<bool> update(Human data);
        Task<List<Human>> get(long id);
        Task<List<Human>> get();
    }
}

using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface ICodeDictionaryRepository {
        Task<bool> create(CodeDictionary dictionary);
        Task<bool> update(CodeDictionary data);
        Task<List<Admin>> get(long id, long humanID);
        Task<List<Admin>> get();
    }
}

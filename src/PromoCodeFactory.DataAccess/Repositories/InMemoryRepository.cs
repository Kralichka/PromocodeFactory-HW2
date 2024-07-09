using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var element = await this.GetByIdAsync(id);

            if (element is null)
                return false;

            var data = Data.ToList();
            data.Remove(element);
            Data = data;

            return true;
        }

        public async Task<T> CreateNewAsync(T entity)
        {
            var data = Data.ToList();
            entity.Id = Guid.NewGuid();
            data.Add(entity);
            Data = data;

            return await Task.FromResult(entity);
        }

        public async Task<T> UpdateEmployeeAsync(T entity)
        {
            var oldEntity = await this.GetByIdAsync(entity.Id);

            if (oldEntity is null)
                return null;

            var data = Data.ToList();
            data.Remove(oldEntity);
            data.Add(entity);
            Data = data;

            return await Task.FromResult(entity);
        }
    }
}
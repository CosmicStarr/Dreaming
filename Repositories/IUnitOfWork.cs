using System;
using System.Threading.Tasks;
using Dreaming.Data;
using Dreaming.Repositories;

namespace DreamMore.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
     
        IApplicationUserRepository _ApplicationUserRepo {get;}
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task<int> Complete();
    }
}
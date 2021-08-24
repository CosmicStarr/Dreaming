using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dreaming.Helper;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.DTOs;

namespace DreamMore.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> Get(int Id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>> filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, 
        string includeproperties = null);
        Task<PagerList<T>> GetAll(Parms Prams = null,Expression<Func<T,bool>> filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, 
        string includeproperties = null);
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
            string includeproperties = null);
        void Add(T entity);
        void Remove(T entity);
        void Remove(string Id);
        void RemoveRange(IEnumerable<T> entity);
    }
}
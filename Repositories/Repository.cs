using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dreaming.Data;
using Dreaming.Helper;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs;
using StackExchange.Redis;

namespace DreamMore.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        //the dbset are the DbSets in the above class. Since this is a generic repository, I have to set<T> them. like giving empty containers labels.
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext Context)
        {
            _context = Context;
            dbSet = Context.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public async Task<T> Get(int Id)
        {
            return await dbSet.FindAsync(Id);
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeproperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeproperties != null)
            {
                foreach (var item in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            if (orderby != null)
            {
                return await orderby(query).ToListAsync();
            }
            return await query.ToListAsync();
        }

        public async Task<PagerList<T>> GetAll(Parms Parms, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeproperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeproperties != null)
            {
                foreach (var item in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            if (orderby != null)
            {
                return await PagerList<T>.CreateAsync(orderby(query), Parms.PageNumber, Parms.PageSize);
            }
            return await PagerList<T>.CreateAsync(query, Parms.PageNumber, Parms.PageSize);

        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeproperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeproperties != null)
            {
                foreach (var item in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Remove(string Id)
        {
            T entityToRemove = dbSet.Find(Id);
            dbSet.Remove(entityToRemove);
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity).State = EntityState.Modified;
        }
    }
}
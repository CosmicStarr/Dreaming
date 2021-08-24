using System;
using System.Collections;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Dreaming.Data;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models;

namespace DreamMore.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration; 


        public UnitOfWork(ApplicationDbContext Context,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration configuration)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = Context;
        }

        public IApplicationUserRepository _ApplicationUserRepo => new ApplicationUserRep(_userManager, _signInManager, _mapper, _configuration);

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repotyp = typeof(Repository<>);
                var repoInstance = Activator.CreateInstance(repotyp.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repoInstance);
            }

            return (IRepository<TEntity>)_repositories[type];
        }
    }
}
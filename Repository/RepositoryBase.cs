using FiskalApp.Contracts;
using FiskalApp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FiskalApp.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected AppDbContext _repositoryContext { get; set; }
        public RepositoryBase(AppDbContext repositoryContext)
        {
            this._repositoryContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {
            return this._repositoryContext.Set<T>().AsNoTracking();
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this._repositoryContext.Set<T>().Where(expression).AsNoTracking();
        }
        public void Create(T entity)
        {
            this._repositoryContext.Set<T>().Add(entity);
        }
        public void Update(T entity)
        {
            this._repositoryContext.Set<T>().Update(entity);
        }
        public void Delete(T entity)
        {
            this._repositoryContext.Set<T>().Remove(entity);
        }
    }
}

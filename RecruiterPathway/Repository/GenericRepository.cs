﻿using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RecruiterPathway.Repository
{
    public class GenericRepository<TModel>: IDisposable where TModel : class
    {
        //protected so subclasses can use
        protected DatabaseContext context;
        protected DbSet<TModel> set;
        public GenericRepository(DatabaseContext context, DbSet<TModel> set)
        {
            this.context = context;
            this.set = set;
        }
        async public Task<IEnumerable<TModel>> Get(Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TModel> query = set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        async public Task<List<TModel>> GetAll()
        {
            Task<List<TModel>> result;
            lock (set)
            {
                result =  set.ToListAsync();
            }
            return await result;
        }
        async public ValueTask<TModel> GetById(object id)
        {
            ValueTask<TModel> result;
            lock (set)
            {
                if (id is Guid guid)
                {
                    result = set.FindAsync(guid);
                }
                else
                {
                    result = set.FindAsync(id);
                }
            }
            return await result;
        }
        async public virtual Task<bool> Insert(TModel obj)
        {
            await set.AddAsync(obj);
            return true;
        }
        public virtual void Delete(TModel obj)
        {
            set.Remove(obj);
        }
        async public virtual void Delete(object id)
        {
            TModel model = await GetById(id);
            lock (set)
            {
                set.Remove(model);
            }
        }
        public void Update(TModel obj)
        {
            context.Entry(obj).State = EntityState.Modified;
        }
        public void Save()
        {
            lock (context)
            {
                context.SaveChanges();
            }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

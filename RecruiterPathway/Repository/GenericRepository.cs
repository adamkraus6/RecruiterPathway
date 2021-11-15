using Microsoft.EntityFrameworkCore;
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
        protected DatabaseContext _context;
        protected DbSet<TModel> _set;
        public GenericRepository(DatabaseContext context, DbSet<TModel> set)
        {
            this._context = context;
            this._set = set;
        }
        async public Task<IEnumerable<TModel>> Get(Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TModel> query;
            lock (_set)
            {
                 query = _set;
            }
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
            lock (_set)
            {
                result =  _set.ToListAsync();
            }
            return await result;
        }
        async virtual public ValueTask<TModel> GetById(object id)
        {
            ValueTask<TModel> result;
            lock (_set)
            {
                if (id is Guid guid)
                {
                    result = _set.FindAsync(guid);
                }
                else
                {
                    result = _set.FindAsync(id);
                }
            }
            return await result;
        }
        async public virtual Task<bool> Insert(TModel obj)
        {
            await _set.AddAsync(obj);
            return true;
        }
        public async virtual Task Delete(TModel obj)
        {
            lock (_context)
            {
                _context.Attach(obj);
                _context.Remove(obj);
            }
            Save();
            Console.WriteLine("Called Delete(obj)");
        }
        async public virtual Task Delete(object id)
        {
            TModel model = await GetById(id);
            if (model == null)
            {
                return;
            }
            lock (_context)
            {
                _context.Attach(model);
                _context.Remove(model);
            }
            Save();
        }
        public async Task Update(TModel obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            Save();
            Console.WriteLine("called update(obj)");
        }
        public virtual void Save()
        {
            var updated = _context.SaveChanges();
            Console.WriteLine("updated " + updated);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            _db.Products.Include(u => u.category).Include(u => u.CategoryId).Include(u=>u.CategoryId);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            query = query.Where(filter);
            return query.FirstOrDefault();
        }
        //Category,CoverType
        public IEnumerable<T> GetAll(string? includeProperties=null)
        {
            IQueryable<T> query = dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties
                    .Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}

using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.impl
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EXE201_Rental_Sport_Field1Context _context;
        public Task AddCategoryAsync(Category Category, int authorId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Category> GetAllCategorysAsync()
        {
           return _context.Categories.AsQueryable();
        }

        public Task<Category> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(Category Category)
        {
            throw new NotImplementedException();
        }
    }
}

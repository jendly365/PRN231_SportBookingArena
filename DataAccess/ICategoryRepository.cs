using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface ICategoryRepository
    {
        Task AddCategoryAsync(Category Category, int authorId);

        // READ
        IQueryable<Category> GetAllCategorysAsync();
        Task<Category> GetCategoryByIdAsync(int id);

        // UPDATE
        Task UpdateCategoryAsync(Category Category);

        // DELETE
        Task DeleteCategoryAsync(int id);
    }
}

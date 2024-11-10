using BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class CategoryDAO
    {
        private Fall24CosmeticsDbContext _context;
        private static CategoryDAO instance;
        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public CategoryDAO()
        {
            _context = new Fall24CosmeticsDbContext();
        }

        public List<CosmeticCategory> GetAll()
        {
            return _context.CosmeticCategories.ToList();
        }

        public CosmeticCategory GetById(string id)
        {
            return _context.CosmeticCategories.SingleOrDefault(x => x.CategoryId.Equals(id));
        }
    }
}

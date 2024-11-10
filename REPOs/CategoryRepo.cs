using BOs;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public class CategoryRepo : ICategoryRepo
    {
        public List<CosmeticCategory> GetCategories() => CategoryDAO.Instance.GetAll();

        public CosmeticCategory GetCategory(string id) => CategoryDAO.Instance.GetById(id);
    }
}

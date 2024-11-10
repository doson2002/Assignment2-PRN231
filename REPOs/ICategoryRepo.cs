using BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public interface ICategoryRepo
    {
        public CosmeticCategory GetCategory(string id);
        public List<CosmeticCategory> GetCategories();
    }
}

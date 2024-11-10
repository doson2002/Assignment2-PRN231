using BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public interface ICosmeticRepo
    {
        public CosmeticInformation GetCosmetic(string id);
        public List<CosmeticInformation> GetCosmetics();
        public bool AddCosmetic(CosmeticInformation cosmetic);
        public bool RemoveCosmetic(string id);
        public bool UpdateCosmetic(CosmeticInformation cosmetic);
    }
}

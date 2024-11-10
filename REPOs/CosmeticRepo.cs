using BOs;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public class CosmeticRepo : ICosmeticRepo
    {
        public bool AddCosmetic(CosmeticInformation cosmetic) => CosmeticDAO.Instance.AddCosmetic(cosmetic);

        public bool RemoveCosmetic(string id) => CosmeticDAO.Instance.RemoveCosmetic(id);

        public bool UpdateCosmetic(CosmeticInformation cosmetic) => CosmeticDAO.Instance.UpdateCosmetic(cosmetic);
        public List<CosmeticInformation> GetCosmetics() => CosmeticDAO.Instance.GetAll();

        public CosmeticInformation GetCosmetic(string id) => CosmeticDAO.Instance.GetById(id);
    }
}

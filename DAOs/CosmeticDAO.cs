using BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class CosmeticDAO
    {
        private Fall24CosmeticsDbContext context;
        private static CosmeticDAO instance;

        public static CosmeticDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CosmeticDAO();
                }
                return instance;
            }
        }

        public CosmeticDAO()
        {
            context = new Fall24CosmeticsDbContext();
        }

        public bool AddCosmetic(CosmeticInformation cosmetic)
        {
            CosmeticCategory existedCategory = this.context.CosmeticCategories
                    .FirstOrDefault(o => o.CategoryId == cosmetic.CategoryId);
            if (existedCategory == null)
            {
                throw new Exception("Category Cosmetic not exist");
            }
            bool result = false;
            CosmeticInformation check = GetById(cosmetic.CosmeticId);
            if (check == null)
            {

                try
                {
                    /* Cosmetic silverJewelry = new Cosmetic()
                     {
                         CosmeticId = cosmetic.CosmeticId,
                         CosmeticName = cosmetic.CosmeticName,
                         MetalWeight = cosmetic.MetalWeight,
                         Price = cosmetic.Price,
                         ProductionYear = cosmetic.ProductionYear,
                         CosmeticDescription = cosmetic.CosmeticDescription,
                         CategoryId = cosmetic.CategoryId,
                         CreatedDate = DateTime.Now

                     };*/
                    context.CosmeticInformations.Add(cosmetic);
                    result = context.SaveChanges() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine(ex.ToString());
                }
            }
            return result;
        }
        public bool RemoveCosmetic(string id)
        {
            bool result = false;
            CosmeticInformation check = this.GetById(id);
            if (check != null)
            {

                try
                {
                    context.CosmeticInformations.Remove(check);
                    result = context.SaveChanges() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }
        public bool UpdateCosmetic(CosmeticInformation cosmetic)
        {
            bool result = false;
            CosmeticInformation check = this.GetById(cosmetic.CosmeticId);
            if (check != null)
            {
                context.Entry(check).State = EntityState.Detached;

                try
                {
                    /*                    context.Entry(check).CurrentValues.SetValues(silverJewelry);*/
                    context.Entry(cosmetic).State = EntityState.Modified;
                    result = context.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine(ex.ToString());
                }
            }
            return result;
        }
        /*        private Cosmetic GetCosmeticById(int id)
                {
                    return context.Cosmetics.SingleOrDefault(x => x.Id.Equals(id));
                }*/
        public List<CosmeticInformation> GetAll()
        {
            return context.CosmeticInformations.Include(x => x.Category).ToList();
        }
        public CosmeticInformation GetById(string id)
        {
            return context.CosmeticInformations.SingleOrDefault(x => x.CosmeticId.Equals(id));
        }
    }
}

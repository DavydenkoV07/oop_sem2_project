using System;
using System.Collections.Generic;
using System.Linq;
using HobbyTracker.Models;

namespace HobbyTracker.Services
{
    public class HobbyManager
    {
        public List<HobbyCategory> Categories { get; private set; }
        private readonly DataStorage _storage;

        public HobbyManager()
        {
            _storage = new DataStorage();
            Categories = _storage.LoadData();
        }

        public void AddCategory(string name)
        {
            Categories.Add(new HobbyCategory { Id = Guid.NewGuid(), Name = name });
            SaveChanges();
        }

        public void AddItemToCategory(Guid categoryId, HobbyItem item)
        {
            var category = Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                item.Id = Guid.NewGuid();
                category.Items.Add(item);
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            _storage.SaveData(Categories);
        }
    }
}
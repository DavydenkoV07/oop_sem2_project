using System;
using System.Collections.Generic;
using System.Linq;
using HobbyTracker.Models;

namespace HobbyTracker.Services
{
    public class HobbyManager
    {
        public List<HobbyCategory> Categories { get; private set; }
        
        // Оголошуємо одразу два сховища
        private readonly MySqlStorage _dbStorage;
        private readonly DataStorage _jsonStorage;

        public HobbyManager()
        {
            _dbStorage = new MySqlStorage();
            _jsonStorage = new DataStorage();

            try
            {
                // Спробуємо завантажити дані з MySQL (основне джерело)
                Categories = _dbStorage.LoadData();
                
                // Якщо база порожня, але в JSON є дані (наприклад, при першому переході на БД)
                if (Categories.Count == 0)
                {
                    Categories = _jsonStorage.LoadData();
                    // Одразу зберігаємо старі дані з JSON у нову базу даних
                    if (Categories.Count > 0)
                    {
                        _dbStorage.SaveData(Categories);
                    }
                }
            }
            catch (Exception)
            {
                // Якщо немає підключення до бази (наприклад, ти забула увімкнути сервер),
                // програма не впаде, а завантажить локальний JSON-бекап!
                Categories = _jsonStorage.LoadData();
            }
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
            // Магія подвійного збереження
            try
            {
                _dbStorage.SaveData(Categories); // Зберігаємо в MySQL
            }
            catch
            {
                // Якщо БД відпала в процесі роботи, ігноруємо помилку, бо є бекап
            }

            _jsonStorage.SaveData(Categories); // Завжди зберігаємо локально у JSON
        }
    }
}
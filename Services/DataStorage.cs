using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HobbyTracker.Models;

namespace HobbyTracker.Services
{
    public class DataStorage
    {
        private readonly string _filePath = "hobby_data.json";

        public void SaveData(List<HobbyCategory> categories)
        {
            string json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public List<HobbyCategory> LoadData()
        {
            if (!File.Exists(_filePath))
            {
                return new List<HobbyCategory>();
            }

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<HobbyCategory>>(json) ?? new List<HobbyCategory>();
        }
    }
}
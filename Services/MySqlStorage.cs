using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using HobbyTracker.Models;

namespace HobbyTracker.Services
{
    public class MySqlStorage
    {
        
        private readonly string _connectionString = "Server=localhost;User ID=root;Password=dav0719;Database=hobby_tracker_db;";

        public List<HobbyCategory> LoadData()
        {
            var categories = new List<HobbyCategory>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            // 1. Завантажуємо всі категорії
            using var catCmd = new MySqlCommand("SELECT * FROM Categories", connection);
            using var catReader = catCmd.ExecuteReader();
            while (catReader.Read())
            {
                categories.Add(new HobbyCategory
                {
                    Id = Guid.Parse(catReader.GetString("Id")),
                    Name = catReader.GetString("Name")
                });
            }
            catReader.Close();

            // 2. Завантажуємо всі записи і розподіляємо їх по категоріях
            using var itemCmd = new MySqlCommand("SELECT * FROM HobbyItems", connection);
            using var itemReader = itemCmd.ExecuteReader();
            while (itemReader.Read())
            {
                var item = new HobbyItem
                {
                    Id = Guid.Parse(itemReader.GetString("Id")),
                    Title = itemReader.GetString("Title"),
                    Description = itemReader.IsDBNull(itemReader.GetOrdinal("Description")) ? null : itemReader.GetString("Description"),
                    ReleaseYear = itemReader.IsDBNull(itemReader.GetOrdinal("ReleaseYear")) ? null : itemReader.GetInt32("ReleaseYear"),
                    PersonalComment = itemReader.IsDBNull(itemReader.GetOrdinal("PersonalComment")) ? null : itemReader.GetString("PersonalComment"),
                    Link = itemReader.IsDBNull(itemReader.GetOrdinal("Link")) ? null : itemReader.GetString("Link"),
                    Status = (ItemStatus)itemReader.GetInt32("Status"),
                    DateAdded = itemReader.GetDateTime("DateAdded")
                };

                var categoryId = Guid.Parse(itemReader.GetString("CategoryId"));
                var category = categories.FirstOrDefault(c => c.Id == categoryId);
                
                if (category != null)
                {
                    category.Items.Add(item);
                }
            }

            return categories;
        }

        public void SaveData(List<HobbyCategory> categories)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Для простоти та імітації збереження файлу: очищаємо старі дані (каскадне видалення прибере і елементи)
                using var clearCmd = new MySqlCommand("DELETE FROM Categories", connection, transaction);
                clearCmd.ExecuteNonQuery();

                // Записуємо нові дані
                foreach (var category in categories)
                {
                    using var catCmd = new MySqlCommand("INSERT INTO Categories (Id, Name) VALUES (@id, @name)", connection, transaction);
                    catCmd.Parameters.AddWithValue("@id", category.Id.ToString());
                    catCmd.Parameters.AddWithValue("@name", category.Name);
                    catCmd.ExecuteNonQuery();

                    foreach (var item in category.Items)
                    {
                        using var itemCmd = new MySqlCommand(@"INSERT INTO HobbyItems 
                            (Id, CategoryId, Title, Description, ReleaseYear, PersonalComment, Link, Status, DateAdded) 
                            VALUES (@id, @catId, @title, @desc, @year, @comment, @link, @status, @date)", connection, transaction);
                        
                        itemCmd.Parameters.AddWithValue("@id", item.Id.ToString());
                        itemCmd.Parameters.AddWithValue("@catId", category.Id.ToString());
                        itemCmd.Parameters.AddWithValue("@title", item.Title ?? "");
                        itemCmd.Parameters.AddWithValue("@desc", item.Description ?? "");
                        itemCmd.Parameters.AddWithValue("@year", item.ReleaseYear.HasValue ? item.ReleaseYear.Value : (object)DBNull.Value);
                        itemCmd.Parameters.AddWithValue("@comment", item.PersonalComment ?? "");
                        itemCmd.Parameters.AddWithValue("@link", item.Link ?? "");
                        itemCmd.Parameters.AddWithValue("@status", (int)item.Status);
                        itemCmd.Parameters.AddWithValue("@date", item.DateAdded);
                        
                        itemCmd.ExecuteNonQuery();
                    }
                }
                transaction.Commit(); // Підтверджуємо всі зміни
            }
            catch
            {
                transaction.Rollback(); // Якщо помилка - скасовуємо
                throw;
            }
        }
    }
}
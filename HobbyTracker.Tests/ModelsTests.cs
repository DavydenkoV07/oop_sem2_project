using System;
using System.Linq;
using NUnit.Framework;
using HobbyTracker.Models;

namespace HobbyTracker.Tests
{
    [TestFixture]
    public class ModelsTests
    {
        [Test]
        public void HobbyCategory_ShouldInitializeWithEmptyList()
        {
            // Arrange & Act
            var category = new HobbyCategory { Name = "Книги" };

            // Assert
            Assert.IsNotNull(category.Items);
            Assert.IsEmpty(category.Items); 
            Assert.AreEqual("Книги", category.Name);
        }

        [Test]
        public void HobbyItem_ShouldHaveCorrectDefaultValues()
        {
            // Arrange & Act
            var item = new HobbyItem { Title = "1984" };

            // Assert
            Assert.AreEqual("1984", item.Title);
            Assert.AreEqual(ItemStatus.Planned, item.Status);
            Assert.IsNull(item.ReleaseYear);
            Assert.IsTrue((DateTime.Now - item.DateAdded).TotalSeconds < 2);
        }

        [Test]
        public void HobbyCategory_CanAddNewItem()
        {
            // Arrange
            var category = new HobbyCategory { Id = Guid.NewGuid(), Name = "Фільми" };
            var item = new HobbyItem { Id = Guid.NewGuid(), Title = "Інтерстеллар", Status = ItemStatus.Completed };

            // Act
            category.Items.Add(item);

            // Assert
            Assert.That(category.Items, Has.Count.EqualTo(1));
            Assert.AreEqual("Інтерстеллар", category.Items.First().Title);
            Assert.AreEqual(ItemStatus.Completed, category.Items.First().Status);
        }

        [Test]
        public void HobbyItem_CanChangeStatus()
        {
            // Arrange
            var item = new HobbyItem { Title = "Матриця", Status = ItemStatus.Planned };

            // Act
            item.Status = ItemStatus.InProgress;

            // Assert
            Assert.AreEqual(ItemStatus.InProgress, item.Status);
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using HobbyTracker.Models;
using HobbyTracker.Services;

namespace HobbyTracker
{
    public partial class MainWindow : Window
    {
        private readonly HobbyManager _manager;
        private HobbyCategory _selectedCategory;

        public MainWindow()
        {
            InitializeComponent();
            _manager = new HobbyManager();
            UpdateCategoriesList();
        }

        // Оновлення списку категорій у лівому меню
        private void UpdateCategoriesList()
        {
            CategoriesListBox.ItemsSource = null;
            CategoriesListBox.ItemsSource = _manager.Categories;
        }

        // Клік: Додати нову категорію
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string name = NewCategoryTextBox.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                _manager.AddCategory(name);
                NewCategoryTextBox.Clear();
                UpdateCategoriesList();
            }
        }

        // Зміна обраної категорії в списку
        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = CategoriesListBox.SelectedItem as HobbyCategory;
            
            if (_selectedCategory != null)
            {
                SelectedCategoryTextBlock.Text = _selectedCategory.Name;
                AddItemPanel.IsEnabled = true; // Розблоковуємо форму додавання
                UpdateItemsList();
            }
            else
            {
                SelectedCategoryTextBlock.Text = "Оберіть розділ ліворуч";
                AddItemPanel.IsEnabled = false;
                ItemsDataGrid.ItemsSource = null;
            }
        }

        // Оновлення таблиці з елементами
        private void UpdateItemsList()
        {
            if (_selectedCategory != null)
            {
                ItemsDataGrid.ItemsSource = null;
                ItemsDataGrid.ItemsSource = _selectedCategory.Items;
            }
        }

        // Клік: Додати новий елемент (фільм/книгу)
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null) return;

            // Відкриваємо наше нове вікно як діалог
            var editorWindow = new ItemEditorWindow();
            editorWindow.Owner = this; // Щоб вікно з'являлося по центру головного

            if (editorWindow.ShowDialog() == true)
            {
                _manager.AddItemToCategory(_selectedCategory.Id, editorWindow.ItemData);
                UpdateItemsList();
            }
        }

        private void ItemsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_selectedCategory == null || ItemsDataGrid.SelectedItem == null) return;

            var selectedItem = (HobbyItem)ItemsDataGrid.SelectedItem;
            
            var editorWindow = new ItemEditorWindow(selectedItem);
            editorWindow.Owner = this;

            if (editorWindow.ShowDialog() == true)
            {
                if (editorWindow.IsDeleted)
                {
                    _selectedCategory.Items.Remove(selectedItem);
                }
                
                _manager.SaveChanges(); // Зберігаємо зміни у JSON
                UpdateItemsList();
            }
        }
    }
}
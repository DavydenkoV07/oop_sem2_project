using System;
using System.Windows;
using HobbyTracker.Models;

namespace HobbyTracker
{
    public partial class ItemEditorWindow : Window
    {
        public HobbyItem ItemData { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        public ItemEditorWindow(HobbyItem item = null)
        {
            InitializeComponent();
            
            // Наповнюємо ComboBox можливими статусами
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(ItemStatus));

            if (item != null)
            {
                // Режим РЕДАГУВАННЯ
                ItemData = item;
                TitleTextBox.Text = item.Title;
                YearTextBox.Text = item.ReleaseYear?.ToString();
                StatusComboBox.SelectedItem = item.Status;
                LinkTextBox.Text = item.Link;
                DescriptionTextBox.Text = item.Description;
                CommentTextBox.Text = item.PersonalComment;
            }
            else
            {
                // Режим ДОДАВАННЯ
                ItemData = new HobbyItem();
                StatusComboBox.SelectedItem = ItemStatus.Planned;
                DeleteButton.Visibility = Visibility.Collapsed; // При створенні кнопка видалення не потрібна
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка, щоб назва не була порожньою
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Назва не може бути порожньою!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Оновлюємо дані об'єкта
            ItemData.Title = TitleTextBox.Text;
            ItemData.Status = (ItemStatus)StatusComboBox.SelectedItem;
            ItemData.Link = LinkTextBox.Text;
            ItemData.Description = DescriptionTextBox.Text;
            ItemData.PersonalComment = CommentTextBox.Text;

            // Спроба розпарсити рік
            if (int.TryParse(YearTextBox.Text, out int year))
                ItemData.ReleaseYear = year;
            else
                ItemData.ReleaseYear = null;

            DialogResult = true; // Вікно закриється успішно
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Ви впевнені, що хочете видалити цей запис?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                IsDeleted = true;
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Вікно просто закриється без збереження
        }
    }
}
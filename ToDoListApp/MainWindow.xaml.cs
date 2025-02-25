using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToDoListApp.Data;
using ToDoListApp.Models;

namespace ToDoListApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetPlaceholderText(TitleTextBox);
            SetPlaceholderText(DescriptionTextBox);

            LoadTasks();
        }

        private bool isEditing = false;
        private TaskItem task;
        private void SetPlaceholderText(TextBox textBox)
        {
            if (textBox.Tag != null)
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = Brushes.Gray;
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Tag != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text) && textBox.Tag != null)
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void ClearForm()
        {
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            DueDatePicker.SelectedDate = null;
            PriorityComboBox.SelectedIndex = -1;
            IsCompletedCheckBox.IsChecked = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing && task != null) // Tryb edycji
            {
                SaveEditTask(sender, e, task);
            }
            else // Dodawanie nowego zadania
            {
                var newTask = new TaskItem
                {
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    DueDate = DueDatePicker.SelectedDate ?? DateTime.Now,
                    Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Niski",
                    IsDone = IsCompletedCheckBox.IsChecked ?? false
                };

                using (var db = new AppDbContext())
                {
                    db.Task.Add(newTask);
                    db.SaveChanges();
                }

                LoadTasks();
                ClearForm();
            }
        }
        private void LoadTasks()
        {
            using (var db = new AppDbContext())
            {
                var tasks = db.Task.ToList();
                TasksListView.ItemsSource = tasks;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Zidentyfikowanie przycisku, który został kliknięty
            var button = sender as Button;
            if (button == null)
                return;

            // Znalezienie obiektu TaskItem przypisanego do tego przycisku
            var task = button.DataContext as TaskItem;
            if (task == null)
                return;

            
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć to zadanie?", "Potwierdzenie", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Usunięcie zadania z bazy danych
                using (var db = new AppDbContext())
                {
                    db.Task.Remove(task);           
                    db.SaveChanges();               
                }

               
                LoadTasks();
            }
        }

        // Metoda do sprawdzenia, czy wprowadzone zmiany zostały zapisane
        private bool IsTaskModified()
        {
            // Sprawdzenie, czy jakiekolwiek pole formularza zostało zmodyfikowane
            return TitleTextBox.Text != task?.Title || DescriptionTextBox.Text != task?.Description ||
                   DueDatePicker.SelectedDate != task?.DueDate || PriorityComboBox.SelectedItem?.ToString() != task?.Priority ||
                   IsCompletedCheckBox.IsChecked != task?.IsDone;
        }

        // Metoda, która zostanie wywołana przy zamykaniu aplikacji
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (isEditing && IsTaskModified())              
            {
                MessageBoxResult result = MessageBox.Show("Masz niezapisane zmiany. Czy chcesz zapisać je przed zamknięciem?", "Potwierdzenie", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    SaveEditTask(null, null, task);         
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            if (button == null)
                return;

            task = button.DataContext as TaskItem;
            if (task == null)
                return;

            // Załadowanie do formularza (TextBox, ComboBox, DatePicker)
            TitleTextBox.Text = task.Title;
            DescriptionTextBox.Text = task.Description;
            DueDatePicker.SelectedDate = task.DueDate;

            // Ustawienie priorytetu w ComboBoxie
            var priority = task.Priority;
            PriorityComboBox.SelectedItem = PriorityComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == priority);

           
            // Zmiana nazwy przycisku na "Zapisz" zamiast "Dodaj"
            UpdateAddButtonContent("Zapisz");

            isEditing = true;
        }

        private void UpdateAddButtonContent(string content)
        {
            AddButton.Content = content;
        }

        private void SaveEditTask(object sender, RoutedEventArgs e, TaskItem task)
        {
            task.Title = TitleTextBox.Text;
            task.Description = DescriptionTextBox.Text;
            task.DueDate = DueDatePicker.SelectedDate ?? DateTime.Now;
            task.Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Niski";
            task.IsDone = IsCompletedCheckBox.IsChecked ?? false;

            using (var db = new AppDbContext())
            {
                db.Task.Update(task);
                db.SaveChanges();
            }

            LoadTasks();
            ClearForm();

            UpdateAddButtonContent("Dodaj");
            isEditing = false;
        }

        private void IsCompletedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (task != null)
            {
                task.IsDone = true;
                SaveEditTask(null, null, task);  
            }
        }
        private void IsCompletedCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            
            if (task != null)
            {
                task.IsDone = false;
                SaveEditTask(null, null, task);  
            }
        }
    }
}
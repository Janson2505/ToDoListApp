using System.Configuration;
using System.Data;
using System.Windows;
using ToDoListApp.Data;

namespace ToDoListApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using(var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            base.OnStartup(e);

        }
    }

}

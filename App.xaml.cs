using System.Configuration;
using System.Data;
using System.Windows;
using YClimb.Utilities;

namespace YClimb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var context = new ApplicationContext())
            {
                await context.Database.EnsureCreatedAsync();
                await context.AfterDatabaseCreated();
            }
        }
    }

}

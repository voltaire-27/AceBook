using AceBook.View;
using Microsoft.Maui.Storage;

namespace AceBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Start with a temporary blank page while we check credentials
            MainPage = new ContentPage();

            // Check for stored credentials in SecureStorage
            _ = InitializeAppAsync();
        }

        private async Task InitializeAppAsync()
        {
            try
            {
                // Get saved credentials from SecureStorage
                string savedEmail = await SecureStorage.GetAsync("email");
                string savedFirstName = await SecureStorage.GetAsync("firstName");
                string savedLastName = await SecureStorage.GetAsync("lastName");

                if (!string.IsNullOrEmpty(savedEmail))
                {
                   
                    MainPage = new AppShell();
                    await Shell.Current.GoToAsync($"//HomePage?firstName={savedFirstName}&lastName={savedLastName}&email={savedEmail}");
                }
                else
                {
                   
                    MainPage = new AppShell();
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Auto-login error: {ex.Message}");

              
                MainPage = new AppShell();
            }
        }
    }
}
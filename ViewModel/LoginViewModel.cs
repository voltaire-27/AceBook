    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Firebase.Database;
    using Firebase.Database.Query;
    using System.Diagnostics;
    using AceBook.Model;
    using AceBook.Includes;
    using Microsoft.Maui.Storage; // For SecureStorage

    namespace AceBook.ViewModel
    {
        public partial class LoginViewModel : ObservableObject
        {
            [ObservableProperty]
            private string email;

            [ObservableProperty]
            private string password;

            [ObservableProperty]
            private bool rememberMe;

    
            [ObservableProperty]
            private bool isLoading;

        
            public bool IsNotLoading => !IsLoading;

            public LoginViewModel()
            {
            
            }



        [RelayCommand]
            private async Task Login()
            {
                // Start loading
                IsLoading = true;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter your email and password.", "OK");
                    IsLoading = false;
                    return;
                }

                try
                {
                    var firebaseClient = PublicVariables.Client;
                    var users = await firebaseClient.Child("Users").OnceAsync<User>();
                    var user = users.FirstOrDefault(u => u.Object.Email == Email);

                    if (user == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "User not found. Please check your email.", "OK");
                        IsLoading = false;
                        return;
                    }

                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.Object.Password);

                    if (!isPasswordValid)
                    {
                        await Shell.Current.DisplayAlert("Error", "Invalid password. Please try again.", "OK");
                        IsLoading = false;
                        return;
                    }

                if (RememberMe)
                {
                    await SecureStorage.SetAsync("email", user.Object.Email);
                    await SecureStorage.SetAsync("firstName", user.Object.FirstName);
                    await SecureStorage.SetAsync("lastName", user.Object.LastName);
                }
                else
                {
                    SecureStorage.Remove("email");
                    SecureStorage.Remove("firstName");
                    SecureStorage.Remove("lastName");
                }
                Preferences.Set("LoggedInEmail", Email);

                    await Shell.Current.DisplayAlert("Success", "Login successful!", "OK");

                await Shell.Current.GoToAsync($"//HomePage?firstName={user.Object.FirstName}&lastName={user.Object.LastName}&email={user.Object.Email}");

            }
            catch (FirebaseException ex)
                {
                    Debug.WriteLine($"Firebase Error: {ex.Message}");
                    await Shell.Current.DisplayAlert("Error", "Failed to login. Please try again.", "OK");
                }
                finally
                {
                    // Stop loading
                    IsLoading = false;            
            }
            }


            [RelayCommand]
            private Task Navigate() =>
                Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database;
using Firebase.Database.Query;
using AceBook.Model;
using AceBook.Includes;
using System.Diagnostics;

namespace AceBook.ViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string dateOfBirth;

        [ObservableProperty]
        private string gender;

        [ObservableProperty]
        private string contactNumber;

        [ObservableProperty]
        private string address;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isLoading;

        public bool IsNotLoading => !IsLoading;

        [RelayCommand]
        private async Task SignUp()
        {
         
            IsLoading = true;

     
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                IsLoading = false; // Stop loading
                return;
            }

       
            var firebaseClient = PublicVariables.Client;
            var users = await firebaseClient
                .Child("Users")
                .OnceAsync<User>();

            bool emailExists = users.Any(u => u.Object.Email == Email);

            if (emailExists)
            {
                await Shell.Current.DisplayAlert("Error", "Email already exists. Please use a different email.", "OK");
                IsLoading = false; // Stop loading
                return;
            }

            // Create a new user object
            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                Gender = Gender,
                ContactNumber = ContactNumber,
                Address = Address,
                Email = Email,
                Password = BCrypt.Net.BCrypt.HashPassword(Password) // Hash the password for security
            };

            try
            {
           
                await firebaseClient
                    .Child("Users")
                    .PostAsync(user); 

               
                await Shell.Current.DisplayAlert("Success", "User registered successfully!", "OK");

                // Navigate back to the login page
                await Shell.Current.GoToAsync("..");            }
            catch (Exception ex)
            {
                // Handle other errors
                Debug.WriteLine($"Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred.", "OK");
            }
            finally
            {
                // Stop loading
                IsLoading = false;
            }
        }

        [RelayCommand]
        private Task Back() => Shell.Current.GoToAsync("..");
    }
}
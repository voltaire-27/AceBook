using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AceBook.Model;
using AceBook.Includes;
using Firebase.Database.Query;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AceBook.ViewModel
{
    [QueryProperty(nameof(FirstName), "firstName")]
    [QueryProperty(nameof(LastName), "lastName")]
    [QueryProperty(nameof(Email), "email")]
    public partial class AddPostViewModel : ObservableObject
    {
        private readonly CloudinaryService _cloudinaryService = new();

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string postContent;

        [ObservableProperty]
        private bool isImageUploadVisible;

        [ObservableProperty]
        private ImageSource selectedImage;

        [ObservableProperty]
        private string imageUrl;


        [RelayCommand]
        private void ToggleImageUpload()
        {
            IsImageUploadVisible = !IsImageUploadVisible;
        }

        [RelayCommand]
        private async Task SelectImage()
        {
            try
            {
                var file = await FilePicker.Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Select an Image",
                        FileTypes = FilePickerFileType.Images,
                    });

                if (file == null)
                    return;

                var url = await _cloudinaryService.UploadImage(file);
                ImageUrl = url;
                SelectedImage = ImageSource.FromUri(new Uri(url));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task CreatePost()
        {
            if (string.IsNullOrWhiteSpace(PostContent) && string.IsNullOrEmpty(ImageUrl))
            {
                await Shell.Current.DisplayAlert("Error", "Post content or image cannot be empty.", "OK");
                return;
            }

            var post = new Post
            {
                Content = PostContent,
                Author = $"{FirstName} {LastName}",
                Email = this.email,
                Timestamp = DateTime.UtcNow,
                ImageUrl = ImageUrl
            };

            try
            {
                var response = await PublicVariables.Client
                    .Child("posts")
                    .PostAsync(post);

                post.Id = response.Key; // Store the Firebase-generated ID

                // Clear fields and navigate back
                PostContent = string.Empty;
                ImageUrl = string.Empty;
                SelectedImage = null;
                IsImageUploadVisible = false;
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to create post: {ex.Message}", "OK");
            }
        }
    }
}
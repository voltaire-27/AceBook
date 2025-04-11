using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AceBook.Model;
using AceBook.Includes;
using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using AceBook.View;
using AceBook.Extensions;  


namespace AceBook.ViewModel
{
    [QueryProperty(nameof(FirstName), "firstName")]
    [QueryProperty(nameof(LastName), "lastName")]
    [QueryProperty(nameof(Email), "email")]
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Post> posts = new ObservableCollection<Post>();

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;


        private IDisposable _postListener;

        public IRelayCommand RefreshCommand { get; }

        public HomeViewModel()
        {
            LoadUserInfo();
            RefreshCommand = new AsyncRelayCommand(RefreshPosts);
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            
            if (e.PropertyName == nameof(Email) && !string.IsNullOrEmpty(Email))
            {
                SetupRealtimeListener();
                _ = LoadInitialPosts(); 
            }
        }

        private async Task LoadInitialPosts()
        {
            try
            {
                var allPosts = await PublicVariables.Client
                    .Child("posts")
                    .OnceAsync<Post>();

                var userPosts = allPosts
                    .Where(p => p.Object.Email == Email)
                    .OrderByDescending(p => p.Object.Timestamp)
                    .Select(p => p.Object)
                    .ToList();

                Posts.Clear();
                foreach (var post in userPosts)
                {
                    Posts.Add(post);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load posts: {ex.Message}", "OK");
            }
        }

        private void SetupRealtimeListener()
        {
            _postListener?.Dispose();


            _postListener = PublicVariables.Client
                .Child("posts")
                .AsObservable<Post>()
                .Subscribe(postEvent =>
                {
                    if (postEvent.Object != null && postEvent.Object.Email == Email)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {

                            var existingPost = Posts.FirstOrDefault(p => p.Timestamp == postEvent.Object.Timestamp);

                            if (existingPost != null)
                            {
                                // Update existing post
                                var index = Posts.IndexOf(existingPost);
                                Posts[index] = postEvent.Object;
                            }
                            else
                            {

                                Posts.Insert(0, postEvent.Object);
                            }
                        });
                    }
                });
        }

        private async void LoadUserInfo()
        {
            try
            {

                string savedEmail = await SecureStorage.GetAsync("email");
                string savedFirstName = await SecureStorage.GetAsync("firstName");
                string savedLastName = await SecureStorage.GetAsync("lastName");

                if (!string.IsNullOrEmpty(savedEmail))
                {
                    Email = savedEmail;
                    FirstName = savedFirstName;
                    LastName = savedLastName;
                }
                else
                {

                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user info: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Logout", "Do you want to logout?", "Yes", "No");

            if (confirm)
            {

                SecureStorage.Remove("email");
                SecureStorage.Remove("firstName");
                SecureStorage.Remove("lastName");


                _postListener?.Dispose();


                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        [RelayCommand]
        private async Task RefreshPosts()
        {
            IsRefreshing = true;
            await LoadInitialPosts();
            IsRefreshing = false;
        }
        [RelayCommand]
        public async Task NavigateAddPostPage()
        {
            await Shell.Current.GoToAsync($"{nameof(AddPost)}?firstName={FirstName}&lastName={LastName}&email={Email}");
        }



        [RelayCommand]
        public async Task ShowPostOptions(Post selectedPost)
        {
            var deleteCommand = new Command(async () =>
            {
                await DeletePost(selectedPost);
            });

            Func<Task> copyAction = async () =>
            {
                await CopyPostContent(selectedPost);
            };

            var bottomSheet = new MySheet(FirstName, LastName, deleteCommand, copyAction);
            await bottomSheet.ShowAsync();
        }

        private async Task DeletePost(Post post)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Post",
                "Are you sure you want to delete this post?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                // Find the post in Firebase
                var posts = await PublicVariables.Client
                    .Child("posts")
                    .OnceAsync<Post>();

                var postToDelete = posts.FirstOrDefault(p => p.Object.Timestamp == post.Timestamp);

                if (postToDelete != null)
                {
                    // Delete from Firebase
                    await PublicVariables.Client
                        .Child("posts")
                        .Child(postToDelete.Key)
                        .DeleteAsync();

                    var postToRemove = Posts.FirstOrDefault(p => p.Timestamp == post.Timestamp);
                    if (postToRemove != null)
                    {
                        Posts.Remove(postToRemove);
                    }

                    var mySheet = Shell.Current.CurrentPage.GetCurrentBottomSheet<MySheet>();
                    if (mySheet != null)
                    {
                        await mySheet.DismissAsync(); // Built-in BottomSheet method
                    }

                    // Optional: Show success message
                    await Shell.Current.DisplayAlert("Success", "Post deleted successfully", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete post: {ex.Message}", "OK");
            }
        }
        private async Task CopyPostContent(Post post)
        {
            try
            {
                string textToCopy = !string.IsNullOrEmpty(post.ImageUrl)
                    ? (!string.IsNullOrEmpty(post.Content)
                        ? $"Post: {post.Content}\n\nImage: {post.ImageUrl}"
                        : $"Image: {post.ImageUrl}")
                    : (!string.IsNullOrEmpty(post.Content)
                        ? post.Content
                        : "No content available");

                await Clipboard.Default.SetTextAsync(textToCopy);

                // Dismiss the bottom sheet
                var mySheet = Shell.Current.CurrentPage.GetCurrentBottomSheet<MySheet>();
                if (mySheet != null)
                {
                    await mySheet.DismissAsync();
                }

                await Shell.Current.DisplayAlert("Copied", "Post content copied to clipboard", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to copy content: {ex.Message}", "OK");
            }
        }

    }
}


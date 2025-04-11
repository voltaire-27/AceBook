using AceBook.ViewModel;

namespace AceBook.View;

public partial class AddPost : ContentPage
{
	public AddPost(AddPostViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
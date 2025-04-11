using System.Windows.Input;
using The49.Maui.BottomSheet;

namespace AceBook.View;

public partial class MySheet : BottomSheet
{

    public ICommand DeleteCommand { get; }
    public ICommand CopyCommand { get; }
    public MySheet(string firstName, string lastName, ICommand deleteCommand, Func<Task> copyAction)
    {
        InitializeComponent();
        FirstNameLabel.Text = firstName;
        LastNameLabel.Text = lastName;
        DeleteCommand = deleteCommand;
        CopyCommand = new Command(async () => await copyAction());


        var deleteButton = this.FindByName<Button>("DeleteButton");
        var copyButton = this.FindByName<Button>("CopyButton");

        if (deleteButton != null) deleteButton.Command = DeleteCommand;
        if (copyButton != null) copyButton.Command = CopyCommand;

    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MahAppsMetroMvvmApp.ViewModels
{
    /// <summary>
    /// What the window shows and what its button does.
    /// </summary>
    /// <remarks>
    /// The MVVM Toolkit writes the rest: ObservableProperty turns a field into a property that tells
    /// the view when it changes, and RelayCommand turns a method into the command a button binds to.
    /// Both need the class to be partial, since what they write is the other half of it.
    /// </remarks>
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name = "MahApps";

        [ObservableProperty]
        private string greeting = "The button below runs a command on this view model.";

        [RelayCommand]
        private void Greet()
        {
            this.Greeting = $"Hello {this.Name}";
        }
    }
}

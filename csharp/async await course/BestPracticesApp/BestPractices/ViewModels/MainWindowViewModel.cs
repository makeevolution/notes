using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BestPractices.ViewModels;

// Class needs to be partial to allow use of ObservableProperty
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentPage = new HomePageViewModel();
   
    [ObservableProperty] 
    private bool _isPaneOpen;


    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    } 

}

public class ListItemTemplate
{
    public ListItemTemplate(Type type)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");
    }
    public string Label { get; }
    public Type ModelType { get; }
}
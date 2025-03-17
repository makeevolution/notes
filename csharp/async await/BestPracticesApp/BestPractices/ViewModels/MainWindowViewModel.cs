using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BestPractices.ViewModels;

// Class needs to be partial to allow use of ObservableProperty
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] 
    private ViewModelBase _currentPage = new HomePageViewModel();
   
    [ObservableProperty] 
    private bool _isPaneOpen;

    [ObservableProperty] private ListItemTemplate? _selectedListItem;
    public ObservableCollection<ListItemTemplate> Items { get; } = new ObservableCollection<ListItemTemplate>()
    {
        new ListItemTemplate(typeof(HomePageViewModel)),
        new ListItemTemplate(typeof(ButtonPageViewModel)),
    };
    
    /// <summary>
    /// The CommunityToolkit.Mvvm has a method defined called OnYourPropertyChanged, which will be called when YourProperty is changed.
    /// We override this method here.
    /// In this case, when the SelectedListItem is changed, this method will be called.
    /// We override this method to change the CurrentPage to the selected page.
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var pageInstance = Activator.CreateInstance(value.ModelType);
        if (pageInstance is null) return;
        CurrentPage = (ViewModelBase) pageInstance;
    }
    
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
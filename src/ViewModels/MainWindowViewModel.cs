using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using rpkGUI.Models;
using rpkGUI.Services;

namespace rpkGUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly AptService _aptService;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isSearchResultsVisible = true;

        public ObservableCollection<Package> SearchResults { get; } = new();
        public ObservableCollection<Package> InstalledPackages { get; } = new();

        public MainWindowViewModel()
        {
            _aptService = new AptService();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;

            IsBusy = true;
            IsSearchResultsVisible = true;
            SearchResults.Clear();

            try
            {
                var packages = await Task.Run(() => _aptService.SearchPackagesAsync(SearchQuery));

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var pkg in packages)
                    {
                        SearchResults.Add(pkg);
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private async Task ListInstalledPackagesAsync()
        {
            IsBusy = true;
            IsSearchResultsVisible = false;
            InstalledPackages.Clear();

            try
            {
                var packages = await Task.Run(() => _aptService.ListPackages());

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var pkg in packages)
                    {
                        InstalledPackages.Add(pkg);
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
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
        private bool _isAptEnabled = true;

        [ObservableProperty]
        private bool _isFlatpakEnabled = false;

        [ObservableProperty]
        private bool _isSnapEnabled = false;

        [ObservableProperty]
        private bool _isPacstallEnabled = false;

        public ObservableCollection<Package> SearchResults { get; } = new();
        public ObservableCollection<Package> InstalledPackages { get; } = new();

        public MainWindowViewModel()
        {
            _aptService = new AptService();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            IsBusy = true;
            SearchResults.Clear();

            try
            {
                if (IsAptEnabled)
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

                // Will add support for more package managers as we go.
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
            InstalledPackages.Clear();

            try
            {
                if (IsAptEnabled)
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
                // Will add support for more package managers as we go.
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task InstallPackageAsync(Package package)
        {
            if (package == null)
                return;

            await _aptService.RunPackageActionAsync(package.Name, "install");
        }

        [RelayCommand]
        private async Task RemovePackageAsync(Package package)
        {
            if (package == null)
                return;

            await _aptService.RunPackageActionAsync(package.Name, "remove");
        }
    }
}

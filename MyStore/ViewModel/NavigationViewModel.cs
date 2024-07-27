using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyStore.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyStore.ViewModel
{
    public class NavigationViewModel : ObservableObject
    {
        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        public ICommand StorePageCommand { get; private set; }
        public ICommand CartPageCommand { get; private set; }

        private UserControl _currentView;

        public NavigationViewModel()
        {
            StorePageCommand = new RelayCommand(NavigateToStore);
            CartPageCommand = new RelayCommand(NavigateToCart);
            NavigateToStore();
        }

        private void NavigateToStore()
        {
            CurrentView = new StorePanel();
        }
        private void NavigateToCart()
        {
            CurrentView = new CartPanel();
        }
    }
}

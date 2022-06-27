using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp_1.Core;

namespace WpfApp_1.MVVM.ViewModel
{
    class MainViewMode : ObservableObjects
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand BookingViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }

        public BookingViewModel BookingVM { get; set; }
        public HomeViewModel HomeVM { get; set; }

        public LoginViewModel LoginVM { get; set; }
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewMode()
        {
            HomeVM = new HomeViewModel();
            LoginVM = new LoginViewModel();
            BookingVM = new BookingViewModel();
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => 
            {
                CurrentView = HomeVM;
            });

            BookingViewCommand = new RelayCommand(o =>
            {
                CurrentView = BookingVM;
            });

            LoginViewCommand = new RelayCommand(o =>
            {
                CurrentView = LoginVM;
            });
        }
    }
}

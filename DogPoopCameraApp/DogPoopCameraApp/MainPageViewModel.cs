
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace DogPoopDetectionSystem.DogPoopCameraApp
{
    public class MainPageViewModel:ObservableObject
    {
        public string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public MainPageViewModel()
        { 
        }

        public async Task Initialize()
        {
            Message = "Hello, World!";
        }
    }
}

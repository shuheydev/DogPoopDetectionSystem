using DogPoopCameraApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DogPoopDetectionSystem.DogPoopCameraApp
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel vm;

        public MainPage()
        {
            InitializeComponent();
        }

        int count = 0;
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = vm = new MainPageViewModel();
            vm.Initialize();

            cameraView.MediaCaptured += CameraView_MediaCaptured;
        }

        private async void CameraView_MediaCaptured(object sender, Xamarin.CommunityToolkit.UI.Views.MediaCapturedEventArgs e)
        {
            var image = e.Image;

            var storeImageService = new StoreImage();
            await storeImageService.InitializeAsync();
            await storeImageService.ToBlobStorage(image);


        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMinutes(15), () =>
            {
                count++;
                //cameraView.Shutter();
                vm.Message = $"{count.ToString()} : {DateTime.Now.ToString("yyyyMMddhhmmss")}";

                //タイマー終了条件を設定
                bool keepRecurring = count < 12;
                if (!keepRecurring)
                {
                    count = 0;
                }

                return keepRecurring;
            });
        }
    }
}

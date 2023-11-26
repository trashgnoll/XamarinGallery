﻿using PicApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace PicApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryPage : ContentPage
    {
        
        readonly string path = @"/storage/emulated/0/Pictures/";
        private ObservableCollection<PictureInfo> _pictureList { get; set; }

        public GalleryPage()
        {
            InitializeComponent();
            _pictureList = new ObservableCollection<PictureInfo>();
            pictureList.ItemsSource = _pictureList;
            InitializeData();
        }

        private void InitializeData()
        {
            LoadPictureList();
        }

        private async void LoadPictureList()
        {
            if (Device.RuntimePlatform != Device.Android || !Directory.Exists(path))
                return;
            DirectoryInfo dir = new DirectoryInfo(path);

            var files = dir.GetFileSystemInfos("*.jpg");
            foreach (var file in files)
                _pictureList.Add(new PictureInfo(file.Name, file.FullName, file.CreationTime));
        }

        private void OpenPicrureButton_Clicked(object sender, EventArgs e)
        {
            if (pictureList.SelectedItem is null)
                return;

            Navigation.PushAsync(new PhotoPage(pictureList.SelectedItem as PictureInfo));
        }

        private async void RemovePictureButton_Clicked(object sender, EventArgs e)
        {
            if (pictureList.SelectedItem is null)
                return;
            PictureInfo pic = pictureList.SelectedItem as PictureInfo;
            var answer = await DisplayAlert("Требуется подтверждение", $"Удалить {pic.NameFile}", "Да", "Нет");

            if (answer == false)
            {
                return;
            }

            try
            {
                if (File.Exists(pic.PathToPicture))
                    File.Delete(pic.PathToPicture);
                
                if (!File.Exists(pic.PathToPicture))
                    _pictureList.Remove(pic);
                else
                    await DisplayAlert("Ошибка", "Не получается удалить файл. Возможно, отсутствуют необходимые права", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }
    }
}
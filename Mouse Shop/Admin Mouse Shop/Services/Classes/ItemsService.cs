using Admin_Mouse_Shop.Model;
using Admin_Mouse_Shop.Services.Interfaces;
using Admin_Mouse_Shop.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Admin_Mouse_Shop.Services.Classes
{
    class ItemsService : IItemsService
    {
        private readonly IMyNavigationService _myNavigationService;
        private readonly IServerService _serverService;
        public ItemsService(IMyNavigationService myNavigationService, IServerService serverService)
        {
            _myNavigationService = myNavigationService;
            _serverService = serverService;
        }

        public void Add(Mouse item)
        {
            var window = App.Container.GetInstance<MainViewModel>();
            if (Find(item) == -1)
            {
                if (item == null || item.Company == null || item.Model == null || item.Price == 0 || item.DPI == 0 || item.ImagePath == null)
                {
                    MessageBox.Show("Fill in all of the attributes!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    FileInfo name = new(item.ImagePath);
                    _serverService.AddImage(item.ImagePath);
                    item.ImagePath = $"ftp://auris.cityhost.com.ua/www/telegram-bots-maga.cx.ua/Photos/{name.Name}";
                    item.ImageLink = $"https://www.telegram-bots-maga.cx.ua/Photos/{name.Name}";
                    window.Products.Add(item);
                }
            }
            else
            {
                MessageBox.Show("This product already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Set(Mouse prevItem, Mouse newItem)
        {
            var window = App.Container.GetInstance<MainViewModel>();
            if (newItem == null || newItem.Company == null || newItem.Model == null || newItem.Price == 0 || newItem.DPI == 0 || newItem.ImagePath == null)
            {
                MessageBox.Show("Fill in all of the attributes!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                FileInfo tmp = new(newItem.ImagePath);
                if(prevItem.ImagePath != $"ftp://auris.cityhost.com.ua/www/telegram-bots-maga.cx.ua/Photos/{tmp.Name}") 
                {
                    _serverService.DeleteImage(prevItem.ImagePath);
                    _serverService.AddImage(newItem.ImagePath);
                    newItem.ImagePath = $"ftp://auris.cityhost.com.ua/www/telegram-bots-maga.cx.ua/Photos/{tmp.Name}";
                    newItem.ImageLink = $"https://www.telegram-bots-maga.cx.ua/Photos/{tmp.Name}";
                }
                window.Products[Find(prevItem)] = newItem;
            }
        }

        public int Find(Mouse mouse)
        {
            var window = App.Container.GetInstance<MainViewModel>();
            for (int i = 0; i < window.Products.Count; i++)
            {
                if (window.Products[i].ToString() == mouse.ToString())
                {
                    return i;
                }
            }
            return -1;
        }
        public void Edit(int Id)
        {
            var window = App.Container.GetInstance<MainViewModel>();
            _myNavigationService.SendChange<ChangeViewModel>(window.Products[Id]);
            _myNavigationService.NavigateTo<ChangeViewModel>();
        }

        public void Delete(int Id)
        {
            var window = App.Container.GetInstance<MainViewModel>();
            var res = MessageBox.Show($"Are you sure you want to delete {window.Products[Id].Company} {window.Products[Id].Model}?\nThis is irreversible!", "Accept?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(res == MessageBoxResult.Yes) {
                _myNavigationService.NavigateTo<AddViewModel>();
                _serverService.DeleteImage(window.Products[Id].ImagePath);
                window.Products.RemoveAt(Id);
            }
        }

    }
}

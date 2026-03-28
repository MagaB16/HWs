using Admin_Mouse_Shop.Messages;
using Admin_Mouse_Shop.Model;
using Admin_Mouse_Shop.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.SqlServer;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace Admin_Mouse_Shop.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly IMessenger _messenger;
        private readonly IItemsService _itemsService;
        private readonly IServerService _serverService;

        public ObservableCollection<Mouse> Products { get; set; } = new();
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                Set(ref _currentViewModel, value);
            }
        }
        public void ReceiveMessage(NavigationMessage message)
        {
            CurrentViewModel = App.Container.GetInstance(message.ViewModelType) as ViewModelBase;
        }
        public void ReceiveMessage(DataMessage message)
        {
            _itemsService.Add(message.Data as Mouse);
        }

        internal void MainClose()
        {
            //_serverService.FtpUploadString(JsonSerializer.Serialize(Products), "products.json");
            using SqlConnection conn = new("Data Source=localhost;Database=Store;Trusted_Connection=True;TrustServerCertificate=True;");
            //File.WriteAllText("last_id.txt", App.Container.GetInstance<AddViewModel>().Id.ToString());
            conn.Open();
            SqlCommand query = new("DROP TABLE Mouse;", conn);
            query.ExecuteNonQuery();
            conn.Close();
            conn.Open();
            query = new("CREATE TABLE Mouse([ImagePath] [nvarchar](141) NOT NULL,[ImageLink] [nvarchar](121) NOT NULL,[Model] [nvarchar](14) NOT NULL,[Company] [nvarchar](8) NOT NULL,[DPI] [int] NOT NULL,[Wireless] [bit] NOT NULL,[Gaming] [bit] NOT NULL,[Price] [float] NOT NULL,[Id] [int] IDENTITY(0,1) NOT NULL);", conn);
            query.ExecuteNonQuery();
            conn.Close();
            for (int i = 0; i < Products.Count; i++)
            {
                conn.Open();
                int Wireless = Products[i].Wireless == false ? 0 : 1;
                int Gaming = Products[i].Gaming == false ? 0 : 1;
                query = new($"INSERT INTO Mouse (ImagePath, ImageLink, Model, Company, DPI, Wireless, Gaming, Price) VALUES('{Products[i].ImagePath}', '{Products[i].ImageLink}', '{Products[i].Model}', '{Products[i].Company}', {Products[i].DPI}, {Wireless}, {Gaming}, {Products[i].Price})", conn);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        internal void MainOpen()
        {
            //Products = JsonSerializer.Deserialize<ObservableCollection<Mouse>>(_serverService.FtpDownloadString("products.json"));
            using SqlConnection conn = new("Data Source=localhost;Database=Store;Trusted_Connection=True;TrustServerCertificate=True;");
            conn.Open();

            var query = new SqlCommand("SELECT * FROM Mouse", conn);
            SqlDataReader reader = query.ExecuteReader();
            var schema = reader.GetColumnSchema();

            while (reader.Read())
            {
                Products.Add(new Mouse()
                {
                    ImagePath = reader.GetString(0),
                    ImageLink = reader.GetString(1),
                    Model = reader.GetString(2),
                    Company = reader.GetString(3),
                    DPI = reader.GetInt32(4),
                    Wireless = reader.GetBoolean(5),
                    Gaming = reader.GetBoolean(6),
                    Price = Convert.ToSingle(reader.GetDouble(7)),
                    Id = reader.GetInt32(8)
                });
            }
            conn.Close();
            conn.Open();
            query = new SqlCommand($"SELECT MAX(Id) FROM [Mouse]", conn);
            var reader2 = query.ExecuteScalar();
            App.Container.GetInstance<AddViewModel>().Id = Convert.ToInt32((reader2.ToString() == "" ? "-1" : reader2.ToString()));
            conn.Close();
        }

        public MainViewModel(IMessenger messenger, IItemsService itemsService, IMyNavigationService myNavigationService, IServerService serverService)
        {
            CurrentViewModel = App.Container.GetInstance<AddViewModel>();
            _itemsService = itemsService;

            _messenger = messenger;
            _messenger.Register<NavigationMessage>(this, ReceiveMessage);
            _messenger.Register<DataMessage>(this, ReceiveMessage);
            _serverService = serverService;
        }

        public RelayCommand<object> EditCommand
        {
            get => new(param =>
            {
                _itemsService.Edit((int)param);
            });
        }

        public RelayCommand<object> DeleteCommand
        {
            get => new(param =>
            {
                _itemsService.Delete((int)param);
            });
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using MyStore.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyStore.ViewModel
{
    public class StoreViewModel : ObservableObject
    {
        public ObservableCollection<Product> Products { get; private set; }

        private const string _pathToProductsJson = @"ApplicationData.Current.LocalFolder\Products.json";
        public StoreViewModel() 
        {
            string fullPathToProducts = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToProductsJson);
            Products = new ObservableCollection<Product>();
            LoadProducts(fullPathToProducts);
        }

        private async void LoadProducts(string path)
        {
            string jsonString = await File.ReadAllTextAsync(path);
            var products = JsonConvert.DeserializeObject<List<Product>>(jsonString);

            foreach (var item in products)
            {
                Products.Add(item);
            }
        }
    }
}

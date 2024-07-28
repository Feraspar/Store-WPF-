using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using System.Windows.Input;

namespace MyStore.ViewModel
{
    public class StoreViewModel : ObservableObject
    {
        public ObservableCollection<Product> Products { get; private set; }
        public ObservableCollection<Product> ProductsInCart { get; private set; }
        public ICommand AddToCartCommand { get; private set; }

        private const string _pathToProductsJson = @"Resources\Products.json";
        private const string _pathToCartJson = @"Resources\Cart.json";
        private string _fullPathToProducts = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, _pathToProductsJson);
        private string _fullPathToCart = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, _pathToCartJson);
        public StoreViewModel() 
        {
            Products = new ObservableCollection<Product>();
            ProductsInCart = new ObservableCollection<Product>();
            AddToCartCommand = new RelayCommand<Product>(AddToCart);

            LoadProducts(_fullPathToProducts, _fullPathToCart);
        }

        private async void LoadProducts(string pathToProducts, string pathToCart)
        {
            string productsJsonString = await File.ReadAllTextAsync(pathToProducts);
            string cartJsonString = await File.ReadAllTextAsync(pathToCart);

            var products = JsonConvert.DeserializeObject<List<Product>>(productsJsonString);
            var productsInCart = JsonConvert.DeserializeObject<List<Product>>(cartJsonString);

            if (products != null)
            {
                foreach (var item in products)
                {
                    Products.Add(item);
                }
            }
            if (productsInCart != null)
            {
                foreach (var item in productsInCart)
                {
                    ProductsInCart.Add(item);
                }
            }
        }

        private async void AddToCart(Product product)
        {
            ProductsInCart.Add(product);
            await SaveCart();

            MessageBox.Show($"{product.Name} добавлен в корзину");
        }

        private async Task SaveCart()
        {
            string jsonString = JsonConvert.SerializeObject(ProductsInCart, Formatting.Indented);
            await File.WriteAllTextAsync(_fullPathToCart, jsonString);
        }
    }
}

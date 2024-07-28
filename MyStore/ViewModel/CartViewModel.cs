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
using System.Windows.Input;

namespace MyStore.ViewModel
{
    public class CartViewModel : ObservableObject
    {
        public ObservableCollection<Product> ProductsInCart { get; private set; }
        public ICommand DeleteProductCommand { get; private set; }
        public int ProductsCount
        {
            get => _productsCount;
            set => SetProperty(ref _productsCount, value);
        }
        public decimal ProductsFullPrice
        {
            get => _productsFullPrice;
            set => SetProperty(ref _productsFullPrice, value);
        }

        private int _productsCount;
        private decimal _productsFullPrice;
        private const string _pathToCartJson = @"ApplicationData.Current.LocalFolder\Cart.json";
        public CartViewModel() 
        {
            string fullPathToCart = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToCartJson);

            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);
            ProductsInCart = new ObservableCollection<Product>();
            LoadCart(fullPathToCart);
        }

        private void GetCountAndFullPriceCart()
        {
            decimal fullPrice = 0;

            foreach (var product in ProductsInCart)
            {
                fullPrice += product.Price;
            }

            ProductsCount = ProductsInCart.Count;
            ProductsFullPrice = fullPrice;
        }

        private async void LoadCart(string pathToCart)
        {
            string cartJsonString = await File.ReadAllTextAsync(pathToCart);
            var productsInCart = JsonConvert.DeserializeObject<List<Product>>(cartJsonString);

            if (productsInCart != null)
            {
                foreach (var item in productsInCart)
                {
                    ProductsInCart.Add(item);
                }
            }

            GetCountAndFullPriceCart();
        }
        private async void DeleteProduct(Product product)
        {
            if (product != null)
            {
                ProductsInCart.Remove(product);
            }
            await SaveCart();
            GetCountAndFullPriceCart();
        }
        private async Task SaveCart()
        {
            string fullPathToCart = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToCartJson);

            string cartJsonString = JsonConvert.SerializeObject(ProductsInCart, Formatting.Indented);
            await File.WriteAllTextAsync(fullPathToCart, cartJsonString);
        }
    }
}

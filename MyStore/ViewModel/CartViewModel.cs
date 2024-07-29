using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyStore.Model;
using MyStore.Services;
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

        private JsonService _jsonService;

        private int _productsCount;
        private decimal _productsFullPrice;
        private const string _pathToCartJson = @"LocalFiles\Cart.json";
        private string _fullPathToCart;
        public CartViewModel() 
        {
            //_fullPathToCart = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, _pathToCartJson);
            _fullPathToCart = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToCartJson);
            _jsonService = new JsonService();
            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);
            ProductsInCart = new ObservableCollection<Product>();
            LoadCart(_fullPathToCart);
        }

        private void GetCountAndFullPriceCart()
        {
            decimal fullPrice = 0;
            int countProducts = 0;

            foreach (var product in ProductsInCart)
            {
                fullPrice += product.Price;
                countProducts += product.Counter;
            }

            ProductsCount = countProducts;
            ProductsFullPrice = fullPrice;
        }

        private async void LoadCart(string pathToCart)
        {
            List<Product> productsInCart = await _jsonService.LoadProducts(pathToCart);

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
            await _jsonService.SaveProducts(_fullPathToCart, ProductsInCart.ToList());
        }
    }
}

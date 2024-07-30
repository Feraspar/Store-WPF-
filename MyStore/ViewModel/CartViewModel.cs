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
        public ObservableCollection<string> SortOptions { get; private set; }
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
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    SortCart();
                    SaveSortProps();
                }
            }
        }
        public decimal TotalPrice => ProductsInCart.Sum(p => p.Price * p.Counter);

        private JsonService _jsonService;

        private int _productsCount;
        private decimal _productsFullPrice;
        private string _selectedSortOption;
        private const string _pathToCartJson = @"LocalFiles\Cart.json";
        private string _fullPathToCart;
        public CartViewModel() 
        {
            _fullPathToCart = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, _pathToCartJson);
            //_fullPathToCart = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToCartJson);
            _jsonService = new JsonService();
            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);
            ProductsInCart = new ObservableCollection<Product>();
            SortOptions = new ObservableCollection<string>
            {
                "В алфавитном порядке",
                "По возрастанию цены",
                "По убыванию цены"
            };

            LoadCart(_fullPathToCart);
        }

        private void GetCountAndFullPriceCart()
        {
            decimal fullPrice = 0;
            int countProducts = 0;

            foreach (var product in ProductsInCart)
            {
                fullPrice += product.Price * product.Counter;
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

            LoadSortProps();
            GetCountAndFullPriceCart();
        }
        private async void DeleteProduct(Product product)
        {
            if (product != null)
            {
                if (product.Counter != 1)
                {
                    product.Counter--;
                }
                else
                    ProductsInCart.Remove(product);
            }

            await SaveCart();
            SortCart();
            GetCountAndFullPriceCart();
        }
        private async Task SaveCart()
        {
            await _jsonService.SaveProducts(_fullPathToCart, ProductsInCart.ToList());
        }
        private void SortCart()
        {
            List<Product> sortedProducts;

            switch (SelectedSortOption)
            {
                case "В алфавитном порядке":
                    sortedProducts = ProductsInCart.OrderBy(p => p.Name).ToList();
                    break;
                case "По возрастанию цены":
                    sortedProducts = ProductsInCart.OrderBy(p => p.Price * p.Counter).ToList();
                    break;
                case "По убыванию цены":
                    sortedProducts = ProductsInCart.OrderByDescending(p => p.Price * p.Counter).ToList();
                    break;
                default:
                    sortedProducts = ProductsInCart.ToList();
                    break;
            }
            ProductsInCart.Clear();

            foreach (var item in sortedProducts)
            {
                ProductsInCart.Add(item);
            }
        }
        private void LoadSortProps()
        {
            if (Properties.Settings.Default.SelectedSortOption != null)
                SelectedSortOption = Properties.Settings.Default.SelectedSortOption;
        }
        private void SaveSortProps()
        {
            Properties.Settings.Default.SelectedSortOption = SelectedSortOption;
            Properties.Settings.Default.Save();
        }
    }
}

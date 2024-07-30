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
using System.Windows;
using System.Windows.Input;

namespace MyStore.ViewModel
{
    /// <summary>
    /// ViewModel для панели с товарами магазина, 
    /// </summary>
    public class StoreViewModel : ObservableObject
    {
        public ObservableCollection<Product> Products { get; private set; }     // Коллекция продуктов для отображения в ListView
        public ObservableCollection<Product> ProductsInCart { get; private set; }   // Коллекция продуктов, добавленных в корзину    
        public ICommand AddToCartCommand { get; private set; }

        private JsonService _jsonService;

        private const string _pathToProductsJson = @"LocalFiles\Products.json";     // Относительный путь к json строке товаров
        private const string _pathToCartJson = @"LocalFiles\Cart.json";     // Относительный путь к json строке корзины
        private readonly string _fullPathToProducts;
        private readonly string _fullPathToCart;
        public StoreViewModel() 
        {
            _fullPathToProducts = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, _pathToProductsJson);       // Создание полного пути из относительного
            _fullPathToCart = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, _pathToCartJson);       // Создание полного пути из относительного
            //_fullPathToCart = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToCartJson);
            //_fullPathToProducts = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pathToProductsJson);

            _jsonService = new JsonService();
            Products = new ObservableCollection<Product>();
            ProductsInCart = new ObservableCollection<Product>();
            AddToCartCommand = new RelayCommand<Product>(AddToCart);    //Команда добавления товара в корзину

            LoadProducts(_fullPathToProducts, _fullPathToCart);
        }

        private async void LoadProducts(string pathToProducts, string pathToCart)       // Асинхронная загрузка данных из json строки
        {
            List<Product> products = await _jsonService.LoadProducts(pathToProducts);
            List<Product> productsInCart = await _jsonService.LoadProducts(pathToCart);

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

        private async void AddToCart(Product product)       // Метод добавления товара в корзину
        {
            if (product != null)
            {
                Product existingItem = ProductsInCart.FirstOrDefault(n => n.Id == product.Id);
                if (existingItem != null)
                {
                    existingItem.Counter++;
                    await SaveCart();

                    MessageBox.Show($"{product.Name} добавлен в корзину");
                }
                else
                {
                    ProductsInCart.Add(product);
                    await SaveCart();

                    MessageBox.Show($"{product.Name} добавлен в корзину");
                }
            }
        }

        private async Task SaveCart()       // Сохранение json строки для корзины
        {
            await _jsonService.SaveProducts(_fullPathToCart, ProductsInCart.ToList());
        }
    }
}

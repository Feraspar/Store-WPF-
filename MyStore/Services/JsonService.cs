using MyStore.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Services
{
    /// <summary>
    /// Класс для преобразования json строки в список объектов и сохранения списка продуктов в json строку
    /// </summary>
    public class JsonService
    {
        public async Task<List<Product>> LoadProducts(string path)  // Получение списка продуктов из json строки
        {
            if (!File.Exists(path))
            {
                return new List<Product>();
            }
            string jsonString = await File.ReadAllTextAsync(path);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(jsonString);
            return products;
        }

        public async Task SaveProducts(string path, List<Product> products)     // Сохранение списка продуктов в json строку
        {
            string jsonString = JsonConvert.SerializeObject(products, Formatting.Indented);
            await File.WriteAllTextAsync(path, jsonString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Model
{
    /// <summary>
    /// Модель товара
    /// </summary>
    public class Product : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }
        public int Counter
        {
            get => _counter;
            set
            {
                if (_counter != value)
                {
                    _counter = value;
                    OnPropertyChanged(nameof(Counter));
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }
        public decimal TotalPrice
        {
            get => Price * Counter;
        }

        public event PropertyChangedEventHandler PropertyChanged;   // Событие, оповещающее об изменении свойств

        private string _name;
        private decimal _price;
        private decimal _totalPrice;
        private int _counter = 1;

        protected void OnPropertyChanged(string propertyName)   
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

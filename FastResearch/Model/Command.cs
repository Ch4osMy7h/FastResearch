using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch.Model
{
    [Serializable]
    public class OptionPair : INotifyPropertyChanged
    {
        private string _option;
        public string option
        {
            get
            {
                return _option;
            }

            set
            {
                _option = value;
                OnPropertyChanged("option");
            }
        }
        private string _myValue;
        public string myValue
        {
            get
            {
                return _myValue;
            }

            set
            {
                _myValue = value;
                OnPropertyChanged("myValue");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Serializable]
    public class Command : INotifyPropertyChanged
    {
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }
        public string executable { get; set; }
        public string file { get; set; }
        public ObservableCollection<OptionPair> options;

        private string _description = string.Empty;
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetCommand() => executable + " " + file + " " +   
            (options != null && options.Count > 0 ? 
            options.Select(it => new string("-" + it.option + " " + it.myValue)).ToList().Aggregate((acc, item) => acc + " " + item) : "");

    }
}

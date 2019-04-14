using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch.Model
{
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
        public List<Tuple<string, string>> options { get; set; }

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
            options.Select(it => new string("-" + it.Item1 + " " + it.Item2)).ToList().Aggregate((acc, item) => acc + " " + item) : "");

    }
}

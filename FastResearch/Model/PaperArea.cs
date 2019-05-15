using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastResearch.Model;

namespace FastResearch.Model
{
    /// <summary>
    /// PaperArea类
    /// </summary>
    public class PaperArea : INotifyPropertyChanged
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
        public string _description { get; set; }
        public List<Paper> _papers { get; set; }
        public PaperArea()
        {
            this._name = "None";
            this._description = "None";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

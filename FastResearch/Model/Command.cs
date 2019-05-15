using SQLite;
using SQLiteNetExtensions.Attributes;
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
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [ForeignKey(typeof(Command))]
        public int commandId { get; set; }

        public OptionPair()
        {
            _option = "";
            myValue = "";
            _isChecked = true;
        }

        public OptionPair(string option = "", string myValue = "", bool _isChecked = true)
        {
            _option = option;
            _myValue = myValue;
            _isChecked = isChecked;
        }

        private bool _isChecked;
        public bool isChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value;
                OnPropertyChanged("isChecked");
            }
        }


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

        /// <summary>
        /// 用于修改OptionPair的Button的Flyout里的绑定
        /// </summary>
        private string _tempValue;
        public string tempValue
        {
            get
            {
                return _tempValue;
            }

            set
            {
                _tempValue = value;
                OnPropertyChanged("tempValue");
            }
        }
        /// <summary>
        /// 用于修改OptionPair的Button的Flyout里的绑定
        /// </summary>
        private string _tempOption;
        public string tempOption
        {
            get
            {
                return _tempOption;
            }

            set
            {
                _tempOption = value;
                OnPropertyChanged("tempOption");
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
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
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
        public string executable { get; set; } = "python";
        public string file { get; set; }


        
        private List<OptionPair> _optionsList;
        /// <summary>
        /// 用于和数据库交互的OptionPair列表
        /// </summary>
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<OptionPair> optionsList { get => _optionsList; set => _optionsList = value; }

        /// <summary>
        /// 用于和UI交互的OptionPair列表
        /// </summary>
        public ObservableCollection<OptionPair> options = new ObservableCollection<OptionPair>();

        public OptionPair tempPair = new OptionPair();

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
        /// <summary>
        /// 使ObservableCollection的内容更新到List里，以便更新数据库
        /// </summary>
        public void Update()
        {
            optionsList = options.ToList();
        }
        /// <summary>
        /// 使List的内容更新到ObservableCollection里，以便更新UI
        /// </summary>
        public void InverseUpdate()
        {
            options = new ObservableCollection<OptionPair>(optionsList);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 通过当前Command的信息，生成可执行命令的字符串
        /// </summary>
        /// <returns>可执行命令字符串</returns>
        public string GetCommand() => executable + " " + file + " " +   
            (options != null && options.Count > 0 && options.Where(m => m.isChecked == true).Count() > 0 ? 
            options.Where(m=>m.isChecked == true).Select(it => "--" + it.option + " " + it.myValue).ToList().Aggregate((acc, item) => acc + " " + item) : "");
        
    }
}

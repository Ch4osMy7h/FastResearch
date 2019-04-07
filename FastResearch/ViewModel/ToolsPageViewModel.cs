using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastResearch;
using FastResearch.Model;
using FastResearch.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace FastResearch
{
    public class ToolsPageViewModel : ViewModelBase
    {
        public ToolsPageViewModel()
        {
            this.commandItems.Add(new Command() { name = "dsadas" });
            this.commandItems.Add(new Command() { name = "dsajio" });
        }

        private ObservableCollection<Command> commandItems = new ObservableCollection<Command>();

        public ObservableCollection<Command> CommandItems
        {
            get
            {
                return this.commandItems;
            }
        }
    }
}

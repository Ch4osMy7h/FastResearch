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
            this.commandItems.Add(new Command() { name = "dsadas", description = "你妈死了", executable = "python", file = "a.py", options = new List<Tuple<string, string>>{Tuple.Create("fuck", "1"), Tuple.Create("suck", "2")}});
            this.commandItems.Add(new Command() { name = "dsajio", description = "我们两个都是你的哥哥" });
        }

        private ObservableCollection<Command> commandItems = new ObservableCollection<Command>();

        public ObservableCollection<Command> CommandItems
        {
            get
            {
                return this.commandItems;
            }
        }

        public void AddCommand(String commandName)
        {
            ToolsPageService service = SimpleIoc.Default.GetInstance<ToolsPageService>();
            service.AddCommand(commandName);
            commandItems.Add(new Command() { name = commandName });
        }
    }
}

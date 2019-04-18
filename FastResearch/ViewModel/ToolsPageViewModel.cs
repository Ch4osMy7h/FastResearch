using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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
            this.CommandItems.Add(new Command() { name = "dsadas", description = "你妈死了", executable = "python", file = "a.py", options = new ObservableCollection<OptionPair>{new OptionPair{ option = "fuck", myValue = "1" }, new OptionPair { option = "suck", myValue = "2" } } });
            this.CommandItems.Add(new Command() { name = "dsajio", description = "我们两个都是你的哥哥" });
            this.CommandItems.Add(new Command() { name = "普公司的", description = "大苏打！！" });
            
        }

        public ObservableCollection<Command> CommandItems { get; set; } = new ObservableCollection<Command>();

        public void AddCommand(String commandName)
        {
            ToolsPageService service = SimpleIoc.Default.GetInstance<ToolsPageService>();
            //service.AddCommand(commandName);
            CommandItems.Add(new Command() { name = commandName });
        }

        public Command CopyCommand(Command command)
        {
            /*
            object copyObject;
            
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, command);
                ms.Seek(0, SeekOrigin.Begin);
                copyObject = formatter.Deserialize(ms);
                ms.Close();
            }
            return (Command)copyObject;*/
            return DeepCopy<Command>(command);
        }

        public void DeleteCommand(int pos)
        {

            Debug.Print(pos.ToString());
            Debug.Print(CommandItems.Count.ToString() + " ");
            try
            {
                CommandItems.RemoveAt(pos);
            }
            catch {}
            
        }

        public T DeepCopy<T>(T obj)
        {
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}

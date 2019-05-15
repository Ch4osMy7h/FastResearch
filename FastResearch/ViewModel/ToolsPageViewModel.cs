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
using FastResearch.DatabaseManager;
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
            CommandItems = CommandDataBase.GetCommand(); 
            foreach(var command in CommandItems)
            {
                command.InverseUpdate();//将每个command里从数据库获得的List转化为ObservableCollection
            }
        }

        public ObservableCollection<Command> CommandItems { get; set; } = new ObservableCollection<Command>();

        public void AddCommand(Command command)
        {
            CommandItems.Add(command);
            CommandDataBase.Insert(command);
        }

        public void DeleteCommand(Command command)
        {
            try
            {
                CommandItems.Remove(command);
            }
            catch
            {

            }
            
            CommandDataBase.Delete(command);
        }

        public void UpdateCommand(Command command)
        {
            CommandDataBase.Update(command);
        }

        public void AddOption(Command command)
        {
            //从目标command里的tempPair构建出新的optionPair并更新相关信息
            OptionPair option = new OptionPair(command.tempPair.option, command.tempPair.myValue, command.tempPair.isChecked)
            {
                commandId = command.tempPair.commandId
            };
            command.options.Add(option);
            command.Update();
            CommandDataBase.Insert(option);
        }

        public void DeleteOption(Command command, OptionPair option)
        {
            command.options.Remove(option);
            command.Update();
            CommandDataBase.Delete(option);
        }

        public void UpdateOption(Command command, OptionPair option)
        {
            option.option = DeepCopy(option.tempOption);
            option.myValue = DeepCopy(option.tempValue);
            command.Update();
            CommandDataBase.Update(option);
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

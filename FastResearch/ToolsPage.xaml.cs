using FastResearch.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FastResearch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToolsPage : Page
    {
        public ToolsPage()
        {
            this.InitializeComponent();
            this.ViewModel = new ToolsPageViewModel();
            TileView.MaximizedItemHeight = 400.0;
        }

        public ToolsPageViewModel ViewModel { get; set; }


        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddDialog addDialog = new AddDialog();
            await addDialog.ShowAsync();
            if(addDialog.Result == AddResult.AddOK)
            {
                ViewModel.AddCommand(addDialog.command);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //SelectedIndex为-1时为预览视图
            if (TileView.SelectedIndex != -1)
            {
                ViewModel.DeleteCommand((Command)TileView.SelectedItem);
            }
            TileView.SelectedIndex = -1;
        }

        private void DeletePairButton_Click(object sender, RoutedEventArgs e)
        {
            //通过当前点击的Button的Tag属性获取ListView的ItemTemplate相关的OptionPair对象
            OptionPair pair = (sender as Button).Tag as OptionPair;
            Command command = (Command)TileView.SelectedItem;
            ViewModel.DeleteOption(command, pair);
            ViewModel.UpdateCommand(command);
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            var dp = new DataPackage();
            dp.SetText(((Command)TileView.SelectedItem).GetCommand());//将当前命令对象的命令文本形式写入剪贴板
            Clipboard.SetContent(dp);      
        }

        private void AddPairButton_Click(object sender, RoutedEventArgs e)
        {
            Command command = (Command)TileView.SelectedItem;
            //将当前command里的tempPair值置空
            command.tempPair.myValue = string.Empty;
            command.tempPair.option = string.Empty;
        }

        private void AddSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Command command = (Command)TileView.SelectedItem;
            ViewModel.AddOption(command);
            ViewModel.UpdateCommand(command);
        }

        private void EditPairButton_Click(object sender, RoutedEventArgs e)
        {
            OptionPair pair = (sender as Button).Tag as OptionPair;
            pair.tempOption = ViewModel.DeepCopy(pair.option);
            pair.tempValue = ViewModel.DeepCopy(pair.myValue);
        }

        private void EditSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Command command = (Command)TileView.SelectedItem;
            OptionPair pair = (sender as Button).Tag as OptionPair;
            ViewModel.UpdateOption(command, pair);
            ViewModel.UpdateCommand(command);
        }
    }
}

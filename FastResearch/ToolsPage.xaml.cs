using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            //this.AreaButton.Content = this.ViewModel.getPaperAreaFirstOrNot();
            //this.ViewModel.getPapers(this.ViewModel.getPaperAreaFirstOrNot());
        }

        public ToolsPageViewModel ViewModel { get; set; }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddDialog addDialog = new AddDialog();
            await addDialog.ShowAsync();
            if(addDialog.Result == AddResult.AddOK)
            {
                ViewModel.AddCommand(addDialog.name);
                
            }
        }

       private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
       {
            var command = (Model.Command)e.ClickedItem;
            Debug.WriteLine(command.name);
            DetailDialog detailDialog = new DetailDialog(command);
            var old_command = ViewModel.CopyCommand(command);
       
            await detailDialog.ShowAsync();
            if(detailDialog.result == EditResult.EditOK)
            {
                
                CommandItemGrid.ItemsSource = ViewModel.CommandItems;
            }
            else
            {
                ((Model.Command)e.ClickedItem).name = old_command.name;
                ((Model.Command)e.ClickedItem).description = old_command.description;
            }
            //System.Diagnostics.Debug.WriteLine(command.GetCommand());
            
       }
    }
}

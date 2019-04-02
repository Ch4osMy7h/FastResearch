using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FastResearch;
using System.Diagnostics;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FastResearch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PapersPage : Page
    {
        public PapersPage()
        {
            this.InitializeComponent();
            this.ViewModel = new PaperAreaViewModel();
            this.AreaButton.Content = this.ViewModel.getPaperAreaFirstOrNot();
            this.ViewModel.getPapers(this.ViewModel.getPaperAreaFirstOrNot());
        }

        public PaperAreaViewModel ViewModel { get; set; }

        private void NavLinksList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if(this.ViewModel.IsPaperAreaMenu)
            {
               
                Model.PaperArea item = (Model.PaperArea)e.ClickedItem;
                this.ViewModel.getPapers(item._name);
                this.NewAreaButton.Content = "Add Paper";
                this.AreaButton.Content = item._name;
                this.ViewModel.IsPaperAreaMenu = false;
            } else
            {
                //实现读取论文
            }
        }

        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {

            this.NewAreaButton.Content = "Add Paper Area";
            this.AreaButton.Content = "Paper Area";
            this.ViewModel.IsPaperAreaMenu = true;
            this.ViewModel.readPaperArea();
        }

        private void PaperItemSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.IsPaperAreaMenu)
            {
                String paperArea = PaperItemInputBox.Text;
                this.ViewModel.addPaperArea(paperArea);
                this.ViewModel.readPaperArea();

            } else
            {
                String paper = PaperItemInputBox.Text;
                this.ViewModel.addPaper(paper, (string)this.AreaButton.Content);
                this.ViewModel.getPapers((string)this.AreaButton.Content);
            } 
        }
        
        private void AppBarButtonBind_Click(object sender, RoutedEventArgs e)
        {
            //LoadDocument();
        }

        private void PaperItemBindButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
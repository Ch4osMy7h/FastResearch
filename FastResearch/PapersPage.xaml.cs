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
        }

        public PaperAreaViewModel ViewModel { get; set; }

        private void NavLinksList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {

            this.NewAreaButton.Content = "Add Paper Area";
            this.AreaButton.Content = "Paper Area";
            this.ViewModel.readPaperArea();
        }

        private void PaperAreaSaveButton_Click(object sender, RoutedEventArgs e)
        {
            String paperArea = PaperAreaInputBox.Text;
            this.ViewModel.addPaperArea(paperArea);
            this.ViewModel.readPaperArea();
        }
    }
}
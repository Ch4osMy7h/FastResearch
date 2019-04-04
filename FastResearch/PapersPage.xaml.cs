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
using FastResearch.Model;

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
            Bind.Visibility = Visibility.Collapsed;
        }

        public PaperAreaViewModel ViewModel { get; set; }
        public StorageFile addPaperFile {get; set;}

        private async void NavLinksList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Model.PaperArea item = (Model.PaperArea)e.ClickedItem;
            if (this.ViewModel.IsPaperAreaMenu)
            {
                Bind.Visibility = Visibility.Visible;
                this.ViewModel.getPapers(item._name);
                this.NewAreaButton.Content = "Add Paper";
                this.AreaButton.Content = item._name;
                this.ViewModel.IsPaperAreaMenu = false;
            } else
            {
                //实现读取论文 

                string paperPath = this.ViewModel.getPdfDocument(item._name);
                Debug.WriteLine(paperPath);
                StorageFile file = await StorageFile.GetFileFromPathAsync(paperPath);
                List<BitmapImage> bitmapImages =
                        await PdfReader.PdfReader.LoadDocument(file, NewAreaButton.Name, PaperItemInputBox.Name);
                this.ViewModel.getPapers(item._name);
            }
        }

        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {
            Bind.Visibility = Visibility.Collapsed;
            PaperItemSaveButton.Visibility = Visibility.Visible;
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
            }
            else
            {
                
            }
        }

        private void AppBarButtonBind_Click(object sender, RoutedEventArgs e)
        {
            //LoadDocument();
        }


        private async void Bind_Checked(object sender, RoutedEventArgs e)
        {

            String paper = PaperItemInputBox.Text;
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            addPaperFile = await picker.PickSingleFileAsync();
            await addPaperFile.RenameAsync(paper + ".pdf");
            StorageFile newFile = await addPaperFile.CopyAsync(ViewModel.rootLocalDataFolder);
            this.ViewModel.addPaper(paper, (string)AreaButton.Content, newFile.Path);
            List<BitmapImage> bitmapImages =
                await PdfReader.PdfReader.LoadDocument(newFile, NewAreaButton.Name, PaperItemInputBox.Name);
            PdfView.ItemsSource = bitmapImages;
            this.ViewModel.getPapers((string)AreaButton.Content);

        }

        private void Bind_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
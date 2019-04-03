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
                PdfReader.PdfFileManger.DeSerializeFile();
                StorageFile file = PdfReader.PdfFileManger.mapPaperToFile(item._name);
                List<BitmapImage> bitmapImages =
                        await PdfReader.PdfReader.LoadDocument(file, NewAreaButton.Name, PaperItemInputBox.Name);
                PdfView.ItemsSource = bitmapImages;
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

        private async void PaperItemSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.IsPaperAreaMenu)
            {
                String paperArea = PaperItemInputBox.Text;
                this.ViewModel.addPaperArea(paperArea);
                this.ViewModel.readPaperArea();
            }
            else
            {
                String paper = PaperItemInputBox.Text;
                this.ViewModel.addPaper(paper, (string)this.AreaButton.Content);
                this.ViewModel.getPapers((string)this.AreaButton.Content);

                StorageFile file = PdfReader.PdfFileManger.copyPaperToFileManger(addPaperFile, (string)this.AreaButton.Content, paper).Result;
                PdfReader.PdfFileManger.SerializeFile();
                List<BitmapImage> bitmapImages =
                        await PdfReader.PdfReader.LoadDocument(file, NewAreaButton.Name, PaperItemInputBox.Name);
                PdfView.ItemsSource = bitmapImages;
            }
        }

        private void AppBarButtonBind_Click(object sender, RoutedEventArgs e)
        {
            //LoadDocument();
        }


        private async void Bind_Checked(object sender, RoutedEventArgs e)
        {

            try
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".pdf");
                addPaperFile = await picker.PickSingleFileAsync();
                await addPaperFile.RenameAsync(PaperItemInputBox.Text + ".pdf");
  
                List<StorageFolder> storageFolders = PdfReader.PdfFileManger.PaperAreaFolder;
                Debug.WriteLine(storageFolders[0].Name);
                StorageFolder folder = storageFolders.Find(x => x.Name == (string)this.AreaButton.Content);
                Debug.WriteLine(folder.Path);
            
                StorageFile newFile = await addPaperFile.CopyAsync(folder);
                Debug.WriteLine("copy success");

                PdfReader.PdfFileManger.Papers.Add(new Paper() { name = PaperItemInputBox.Text, paperLocation = newFile });
            }
            catch
            {
                Debug.WriteLine("这句有bUG");
            }
            
            Debug.WriteLine("1234");
            PdfReader.PdfFileManger.SerializeFile();
            Debug.WriteLine("12345");
            List<BitmapImage> bitmapImages =
                    await PdfReader.PdfReader.LoadDocument(addPaperFile, NewAreaButton.Name, PaperItemInputBox.Name);
            PdfView.ItemsSource = bitmapImages;
        }

        private void Bind_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
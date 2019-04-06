﻿using System;
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
using Syncfusion.Pdf.Parsing;
using Windows.System;
using Windows.UI.Core;
using Syncfusion.Pdf.Interactive;
using Syncfusion.UI.Xaml.Controls.Navigation;
using Syncfusion.Windows.PdfViewer;
using System.Reflection;
using System.Drawing;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FastResearch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PapersPage : Page
    {
        DispatcherTimer m_sliderTimer;
        int m_sliderTickCount = 0;
        int m_sliderCurrentValue = 0;
        bool m_isButtonClicked;
        bool m_isSinglePageView;
        bool isSearchToolbarVisible = false;
        bool isAnnotationToolbarVisible = false;
        bool isAnnotationMode;
        bool isBookmarkClicked = false;
        enum AnnotationMode
        {
            None,
            InkAnnotationMode,
            TextMarkupAnnotationMode,
            ShapesAnnotationMode,
            PopupAnnotationMode
        }

        AnnotationMode currentAnnotationMode;


        public PapersPage()
        {
            this.InitializeComponent();
            this.ViewModel = new PaperAreaViewModel();
            this.AreaButton.Content = this.ViewModel.getPaperAreaFirstOrNot();
            this.ViewModel.getPapers(this.ViewModel.getPaperAreaFirstOrNot());
            PaperItemList.ItemsSource = this.ViewModel.PapersItems;
            Bind.Visibility = Visibility.Visible;
            PaperItemSaveButton.Visibility = Visibility.Collapsed;
        }

        public PaperAreaViewModel ViewModel { get; set; }
        public StorageFile addPaperFile { get; set; }

        private async void NavLinksList_OnItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            Model.PaperArea item = (Model.PaperArea)e.ClickedItem;
            if (this.ViewModel.IsPaperAreaMenu)
            {
                Bind.Visibility = Visibility.Visible;
                this.ViewModel.getPapers(item._name);
                PaperItemList.ItemsSource = this.ViewModel.PapersItems;
                this.NewAreaButton.Content = "Add Paper";
                this.AreaButton.Content = item._name;
                this.ViewModel.IsPaperAreaMenu = false;
            }
            else
            {
                //实现读取论文 
                try
                {

                    string paperPath = this.ViewModel.getPdfDocument(item._name);
                    Debug.WriteLine(paperPath);
                    StorageFile file = await StorageFile.GetFileFromPathAsync(paperPath);
                    PdfLoadedDocument pdf =
                            await PdfReader.PdfReader.LoadDocument(file, NewAreaButton.Name, PaperItemInputBox.Name);
                    this.ViewModel.getPapers((string)AreaButton.Content);

                    BookmarkButton.IsEnabled = false;

                    
                    PaperItemList.ItemsSource = this.ViewModel.PapersItems;
                    if(InkButton.IsChecked.Value)
                    {
                        InkButton.IsChecked = false;
                    }
                    pdfViewer.LoadDocument(pdf);
                    if (BookmarkGrid != null && BookmarkGrid.Visibility == Visibility.Visible)
                    {
                        BookmarkGrid.Visibility = Visibility.Collapsed;
                        isBookmarkClicked = false;
                    }
                    if (PageCounttext != null)
                        PageCounttext.Text = string.Format("of {0}", pdfViewer.PageCount.ToString());
                    if(pdf.Bookmarks.Count > 0)
                    {
                        BookmarkButton.IsEnabled = true;
                    }
                    LoadNavigator(pdf);
                }
                catch (Exception ex)
                {
                    ContentDialog noWifiDialog = new ContentDialog
                    {
                        Title = "Wrong",
                        Content = "文件已丢失，请重新",
                        CloseButtonText = "Close"
                    };

                    ContentDialogResult result = await noWifiDialog.ShowAsync();
                }
            }
        }

        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {
            PaperItemSaveButton.Visibility = Visibility.Visible;
            Bind.Visibility = Visibility.Collapsed;
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
                PaperItemList.ItemsSource = this.ViewModel.PapersItems;
            }
            else
            {

            }
        }

        private void AppBarButtonBind_Click(object sender, RoutedEventArgs e)
        {
            //LoadDocument();
        }

        private void PaperViewFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private async void Bind_Click(object sender, RoutedEventArgs e)
        {
            String paper = PaperItemInputBox.Text;
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            addPaperFile = await picker.PickSingleFileAsync();
            //当没有选择到文件的时候，返回w
            if (addPaperFile == null) return;
            await addPaperFile.RenameAsync(paper + ".pdf");
            StorageFile newFile = await addPaperFile.CopyAsync(ViewModel.rootLocalDataFolder);
            this.ViewModel.addPaper(paper, (string)AreaButton.Content, newFile.Path);
            PdfLoadedDocument pdf =
                await PdfReader.PdfReader.LoadDocument(newFile, NewAreaButton.Name, PaperItemInputBox.Name);
            pdfViewer.LoadDocument(pdf);

            this.ViewModel.getPapers((string)AreaButton.Content);
        }







        /// <summary>
        /// pdf内容查找快捷键的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viewer_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            CoreVirtualKeyStates controlKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if ((controlKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down && e.Key == VirtualKey.F)
            {
                PageSearchTextBox.Focus(FocusState.Programmatic);
            }
            if (e.Key == VirtualKey.Enter && PageSearchTextBox.FocusState != FocusState.Unfocused)
            {
                if (pdfViewer.SearchText(PageSearchTextBox.Text))
                {
                    NextButton.Visibility = Visibility.Visible;
                    PrevButton.Visibility = Visibility.Visible;
                    NotFoundTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    NotFoundTextBlock.Visibility = Visibility.Visible;
                    NextButton.Visibility = Visibility.Collapsed;
                    PrevButton.Visibility = Visibility.Collapsed;
                }
                e.Handled = true;
            }
        }

        private void closeBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            BookmarkGrid.Visibility = Visibility.Collapsed;
            isBookmarkClicked = false;
        }

        private void Bookmark_Click(object sender, RoutedEventArgs e)
        {
            if (!isBookmarkClicked)
            {
                BookmarkGrid.Visibility = Visibility.Visible;
                isBookmarkClicked = true;
            }
            else
            {
                BookmarkGrid.Visibility = Visibility.Collapsed;
                isBookmarkClicked = false;
            }
        }

        private void LoadNavigator(PdfLoadedDocument ldoc)
        {
            PdfBookmarkBase bookmarkBase = ldoc.Bookmarks;
            PdfBookmark bookmark;
            SfTreeNavigatorItem navigatorItem;

            treeNavigator.Items.Clear();

            for (int i = 0; i < bookmarkBase.Count; i++)
            {
                bookmark = bookmarkBase[i] as PdfLoadedBookmark;
                navigatorItem = AddChildBookmarks(bookmark);
                navigatorItem.ItemClicked += NavigatorItem_ItemClicked;
                navigatorItem.PointerEntered += NavigatorItem_PointerEntered;
                navigatorItem.PointerExited += NavigatorItem_PointerExited;
                treeNavigator.Items.Add(navigatorItem);
            }
        }



        private void NavigatorItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            SfTreeNavigatorItem item = sender as SfTreeNavigatorItem;
            item.HeaderTemplate = Resources["HeaderTemplate1"] as DataTemplate;
        }

        private void NavigatorItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            SfTreeNavigatorItem item = sender as SfTreeNavigatorItem;
            item.HeaderTemplate = Resources["HeaderTemplate2"] as DataTemplate;
        }




        private SfTreeNavigatorItem AddChildBookmarks(PdfBookmark bookmark)
        {
            if (bookmark != null)
            {
                SfTreeNavigatorItem child = new SfTreeNavigatorItem() { Padding = new Thickness(10) };

                string header = bookmark.Title;
                if (header.Length > 60)
                    header = header.Substring(0, 58) + ". . . . . . . . ";
                child.Header = header;
                child.ItemClicked += NavigatorItem_ItemClicked;
                child.PointerEntered += NavigatorItem_PointerEntered;
                child.PointerExited += NavigatorItem_PointerExited;
                child.Tag = bookmark;

                //这段需要研究下
                if (bookmark.Count != 0)
                {
                    for (int i = 0; i < bookmark.Count; i++)
                    {
                        SfTreeNavigatorItem innerChild = new SfTreeNavigatorItem();
                        string innerHeader = bookmark.Title;
                        if (innerHeader.Length > 60)
                            innerHeader = header.Substring(0, 58) + ". . . . . . . . ";
                        child.Header = innerHeader;
                        innerChild.ItemClicked += NavigatorItem_ItemClicked;
                        innerChild.PointerEntered += NavigatorItem_PointerEntered;
                        innerChild.PointerExited += NavigatorItem_PointerExited;
                        innerChild.Tag = bookmark[i];
                        child.Items.Add(innerChild);
                    }
                }

                if (child.Items != null)
                {
                    for (int i = 0; i < child.Items.Count; i++)
                        child.Items[i] = AddChildBookmarks(bookmark[i]);
                }
                return child;
            }
            else
                return null;
        }

        private void NavigatorItem_ItemClicked(object Sender, Syncfusion.UI.Xaml.Controls.Navigation.ItemClickEventArgs args)
        {
            SfTreeNavigatorItem item = Sender as SfTreeNavigatorItem;
            PdfBookmark bookmark = item.Tag as PdfBookmark;
            pdfViewer.GoToBookmark(bookmark);
        }

        private void PageSearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            NextButton.Visibility = Visibility.Collapsed;
            PrevButton.Visibility = Visibility.Collapsed;
        }

        private void PageSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PageSearchTextBox.SelectAll();
            NotFoundTextBlock.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
            PrevButton.Visibility = Visibility.Collapsed;
        }

        private void CloseSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchToolbar.Visibility = Visibility.Collapsed;
            isSearchToolbarVisible = false;
            NotFoundTextBlock.Visibility = Visibility.Collapsed;
            PageSearchTextBox.Text = "";
            pdfViewer.ClearTextSelectionCommand.Execute(true);
        }

        private void InkButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.InkAnnotationCommand.Execute(true);
        }

        private void HighlightButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.HighlightAnnotationCommand.Execute(true);
        }

        private void UnderlineButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.UnderlineAnnotationCommand.Execute(true);
        }

        private void StrikethroughButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.StrikeoutAnnotationCommand.Execute(true);
        }

        private void LineButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.LineAnnotationCommand.Execute(true);
        }

        private void RectangleButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.RectangleAnnotationCommand.Execute(true);
        }

        private void EllipseButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.EllipseAnnotationCommand.Execute(true);
        }

        private void PopupButton_Checked(object sender, RoutedEventArgs e)
        {
            pdfViewer.PopupAnnotationCommand.Execute(true);
        }

        private void InkButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.InkAnnotationCommand.Execute(false);
        }

        private void HighlightButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.HighlightAnnotationCommand.Execute(false);
        }

        private void UnderlineButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.UnderlineAnnotationCommand.Execute(false);
        }

        private void StrikethroughButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.StrikeoutAnnotationCommand.Execute(false);
        }

        private void LineButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.LineAnnotationCommand.Execute(false);
        }

        private void RectangleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.RectangleAnnotationCommand.Execute(false);
        }

        private void EllipseButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.EllipseAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void PopupButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.PopupAnnotationCommand.Execute(false);
        }

        private void CloseAnnotations_Click(object sender, RoutedEventArgs e)
        {
            AnnotationToolbar.Visibility = Visibility.Collapsed;
            isAnnotationToolbarVisible = false;
            UncheckOthers(sender);
        }

        private void UncheckOthers(object sender)
        {
            ToggleButton checkedButton = sender as ToggleButton;

            if ((checkedButton == null) || ((InkButton != checkedButton) && (InkButton.IsChecked.Value)))
                InkButton.IsChecked = false;
            if ((checkedButton == null) || ((HighlightButton != checkedButton) && (HighlightButton.IsChecked.Value)))
                HighlightButton.IsChecked = false;
            if ((checkedButton == null) || ((UnderlineButton != checkedButton) && (UnderlineButton.IsChecked.Value)))
                UnderlineButton.IsChecked = false;
            if ((checkedButton == null) || ((StrikethroughButton != checkedButton) && (StrikethroughButton.IsChecked.Value)))
                StrikethroughButton.IsChecked = false;
            if ((checkedButton == null) || ((LineButton != checkedButton) && (LineButton.IsChecked.Value)))
                LineButton.IsChecked = false;
            if ((checkedButton == null) || ((RectangleButton != checkedButton) && (RectangleButton.IsChecked.Value)))
                RectangleButton.IsChecked = false;
            if ((checkedButton == null) || ((EllipseButton != checkedButton) && (EllipseButton.IsChecked.Value)))
                EllipseButton.IsChecked = false;
            if ((checkedButton == null) || ((PopupButton != checkedButton) && (PopupButton.IsChecked.Value)))
                PopupButton.IsChecked = false;
        }

        private void BindButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PageDestinationTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int destPage = 0;
            if (!string.IsNullOrEmpty(PageDestinationTextBox.Text))
            {
                bool result = int.TryParse(PageDestinationTextBox.Text, out destPage);
                if (e.Key == VirtualKey.Enter && result)
                {
                    if (destPage > 0 && destPage <= pdfViewer.PageCount)
                    {
                        pdfViewer.GotoPage(destPage);
                        e.Handled = true;
                        LoseFocus(sender);
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog(string.Format("There is no page numbered '{0}' in this document.", destPage.ToString()));
                        messageDialog.Options = MessageDialogOptions.None;
                        messageDialog.Title = "FastResearch";
                        messageDialog.Commands.Add(new UICommand("OK"));
                        await messageDialog.ShowAsync();
                    }
                }
            }
        }

        private void LoseFocus(object sender)
        {
            var control = sender as Control;
            var isToolbarStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isToolbarStop;
        }

        /// <summary>
        /// 判断是否为整数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageDestinationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var text = textBox.Text;
            int result;
            var isInterger = int.TryParse(text, out result);
            if (isInterger)
                return;
            if (text.Length > 0)
                textBox.Text = text.Remove(text.Length - 1);
        }

        private void PageDestinationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PageDestinationTextBox.Text = string.Format("{0}", pdfViewer.PageNumber);
        }

        private void PageDestinationTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PageDestinationTextBox.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeZoom(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (pdfViewer.Zoom <= 75)
            {
                ZoomOutButton.IsEnabled = false;
                if (clickedButton.Name == "ZoomInButton")
                    ZoomOutButton.IsEnabled = true;
            }
            else if (pdfViewer.Zoom >= 300)
            {
                ZoomInButton.IsEnabled = false;
                if (clickedButton.Name == "ZoomOutButton")
                    ZoomInButton.IsEnabled = true;
            }
            else
            {
                ZoomInButton.IsEnabled = true;
                if (!OnePageButton.IsEnabled)
                    ZoomOutButton.IsEnabled = false;
                else
                    ZoomOutButton.IsEnabled = true;
            }

            FitWidthButton.IsEnabled = true;
            OnePageButton.IsEnabled = true;
        }

        private void ViewModeChange(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "FitWidthButton")
            {
                pdfViewer.ViewMode = PageViewMode.FitWidth;
                FitWidthButton.IsEnabled = false;
                OnePageButton.IsEnabled = true;
                ZoomInButton.IsEnabled = true;
                ZoomOutButton.IsEnabled = true;
            }
            else if (btn.Name == "OnePageButton")
            {
                pdfViewer.ViewMode = PageViewMode.OnePage;
                OnePageButton.IsEnabled = false;
                FitWidthButton.IsEnabled = true;
                ZoomOutButton.IsEnabled = false;
                ZoomInButton.IsEnabled = true;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSearchToolbarVisible)
            {
                AnnotationToolbar.Visibility = Visibility.Collapsed;
                isAnnotationToolbarVisible = false;
                PageSearchTextBox.Focus(FocusState.Programmatic);
                SearchToolbar.Visibility = Visibility.Visible;
                isSearchToolbarVisible = true;
            }
            else
            {
                SearchToolbar.Visibility = Visibility.Collapsed;
                isSearchToolbarVisible = false;
            }
        }

        private void AnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnnotationToolbarVisible)
            {
                SearchToolbar.Visibility = Visibility.Collapsed;
                isSearchToolbarVisible = false;
                AnnotationToolbar.Visibility = Visibility.Visible;
                isAnnotationToolbarVisible = true;
            }
            else
            {
                AnnotationToolbar.Visibility = Visibility.Collapsed;
                UncheckOthers(AnnotationButton);
                pdfViewer.ResetAnnotationModeCommand.Execute(true);
                m_isButtonClicked = true;
                isAnnotationToolbarVisible = false;
            }
        }
    }
}
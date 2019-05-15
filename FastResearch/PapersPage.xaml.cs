using FastResearch.Model;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using Syncfusion.UI.Xaml.Controls.Navigation;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


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
        private string curPaperName = "";


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
            this.DataContext = pdfViewer;
            this.Unloaded += MainPage_Unloaded;
            this.pdfViewer.SemanticZoomChanged += PdfViewer_SemanitcZoomChanged;
            //solve the problem that can't not press a button in titlebar 
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(BackgroundElement);
      
            this.AreaButton.Content = this.ViewModel.getPaperAreaFirstOrNot();
            this.ViewModel.getPapers(this.ViewModel.getPaperAreaFirstOrNot());
            Bind.Visibility = Visibility.Visible;
            PaperItemSaveButton.Visibility = Visibility.Collapsed;
        }


        private void PdfViewer_SemanitcZoomChanged(object sender, SemanticZoomViewChangedEventArgs e)
        {
            
        }

        public PaperAreaViewModel ViewModel { get; set; }
        public StorageFile addPaperFile { get; set; }
        public PaperArea CurClickItem { get; private set; }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.pdfViewer != null)
                this.pdfViewer.Unload(true);
            UnlinkChildrens(this);
        }



        void UnlinkChildrens(UIElement element)
        {
            if (element == null)
                return;
            if (element is Panel)
            {
                for (int i = 0; i < (element as Panel).Children.Count; i++)
                {
                    UIElement childElement = (element as Panel).Children[i];
                    UnlinkChildrens(childElement);
                    (element as Panel).Children.Remove(childElement);
                    i--;
                }
            }
            else if (element is ItemsControl)
            {
                for (int j = 0; j < (element as ItemsControl).Items.Count; j++)
                {
                    UIElement childElement = ((element as ItemsControl).Items[j] as UIElement);
                    if (childElement != null)
                    {
                        UnlinkChildrens(childElement);
                        (element as ItemsControl).Items.Remove(childElement);
                        j--;
                    }
                }
            }
            else if (element is ContentControl)
            {
                UnlinkChildrens((element as ContentControl).Content as UIElement);
                (element as ContentControl).Content = null;
            }
            else if (element is UserControl)
            {
                UnlinkChildrens((element as UserControl).Content as UIElement);
                (element as UserControl).Content = null;
            }
        }


        /// <summary>
        /// ListView Item点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NavLinksList_OnItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            Model.PaperArea item = (Model.PaperArea)e.ClickedItem;
            //获取当前click的ListItem
            CurClickItem = item;
            if (this.ViewModel.IsPaperAreaMenu)
            {
               
                
                PaperItemSaveButton.Visibility = Visibility.Collapsed;
                Bind.Visibility = Visibility.Visible;
                this.ViewModel.getPapers(item.name);
            
                this.NewAreaButton.Content = "Add Paper";
                this.AreaButton.Content = item.name;
                this.PaperItemInputBox.Text = "";
                this.ViewModel.IsPaperAreaMenu = false;
            }
            else
            {
                //实现读取论文 
                try
                {
                    curPaperName = item.name;
                    string paperPath = this.ViewModel.getPdfDocument(item.name);
                    StorageFile file = await StorageFile.GetFileFromPathAsync(paperPath);
                    PdfLoadedDocument pdf =
                            await PdfReader.PdfReader.LoadDocument(file, NewAreaButton.Name, PaperItemInputBox.Name);
                    this.ViewModel.getPapers((string)AreaButton.Content);

                    BookmarkButton.IsEnabled = false;
                    pdfViewer.PageChanged += PdfViewer_PageChanged;

                   // PaperItemList.ItemsSource = this.ViewModel.PapersItems;
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


        /// <summary>
        /// 论文跳转按钮可用判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PdfViewer_PageChanged(object sender, PageChangedEventArgs e)
        {
            PageDestinationTextBox.Text = string.Format("{0}", pdfViewer.PageNumber.ToString());
            if (pdfViewer.PageNumber == 1)
                PrevPageButton.IsEnabled = false;
            else
                PrevPageButton.IsEnabled = true;
            if (pdfViewer.PageNumber == pdfViewer.PageCount)
                NextPageButton.IsEnabled = false;
            else
                NextPageButton.IsEnabled = true;
        }


        /// <summary>
        ///论文领域界面跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {
            PaperItemSaveButton.Visibility = Visibility.Visible;
            Bind.Visibility = Visibility.Collapsed;
            this.NewAreaButton.Content = "Add Paper Area";
            this.AreaButton.Content = "Paper Area";
            this.ViewModel.IsPaperAreaMenu = true;
            this.ViewModel.readPaperArea();
        }


        /// <summary>
        /// 论文保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PaperItemSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.IsPaperAreaMenu)
            {
                String paperArea = PaperItemInputBox.Text;
                if(this.ViewModel.addPaperArea(paperArea) == false)
                {
                    ContentDialog noWifiDialog = new ContentDialog
                    {
                        Title = "Wrong",
                        Content = "不能插入同名的领域",
                        CloseButtonText = "Close"
                    };

                    ContentDialogResult result = await noWifiDialog.ShowAsync();
                }
                this.ViewModel.readPaperArea();
                //PaperItemList.ItemsSource = this.ViewModel.PapersItems;
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

        /// <summary>
        /// 论文绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Bind_Click(object sender, RoutedEventArgs e)
        {
            PaperItemSaveButton.Visibility = Visibility.Collapsed;
            String paper = PaperItemInputBox.Text;
            if(paper.Length == 0)
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Wrong",
                    Content = "文件名不能为空",
                    CloseButtonText = "Close"
                };
                
                ContentDialogResult result = await noWifiDialog.ShowAsync();
                return ;
            }
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            addPaperFile = await picker.PickSingleFileAsync();
            //当没有选择到文件的时候，返回
            if (addPaperFile == null) return;
            StorageFile newFile;
            try
            {
                newFile = await addPaperFile.CopyAsync(ViewModel.rootLocalDataFolder);
                await newFile.RenameAsync(paper + ".pdf", NameCollisionOption.GenerateUniqueName);
            } catch (Exception ex)
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Wrong",
                    Content = "Paper的名字不能重复",
                    CloseButtonText = "Close"
                };

                ContentDialogResult result = await noWifiDialog.ShowAsync();
                return ;
            }
           
            if (this.ViewModel.addPaper(paper, (string)AreaButton.Content, newFile.Path) == false)
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Wrong",
                    Content = "Paper的名字不能重复",
                    CloseButtonText = "Close"
                };

                ContentDialogResult result = await noWifiDialog.ShowAsync();
            }
            PdfLoadedDocument pdf =
                await PdfReader.PdfReader.LoadDocument(newFile, NewAreaButton.Name, PaperItemInputBox.Name);
            pdfViewer.LoadDocument(pdf);
            if (PageCounttext != null)
                PageCounttext.Text = string.Format("of {0}", pdfViewer.PageCount.ToString());
          
            pdfViewer.PopupAnnotationAdded += PdfViewer_PopupAnnotationAdded;
            if (pdf.Bookmarks.Count > 0)
                BookmarkButton.IsEnabled = true;
            LoadNavigator(pdf);


            this.ViewModel.getPapers((string)AreaButton.Content);
            //方便后面保存操作

            curPaperName = (string)AreaButton.Content;
            PaperItemInputBox.Text = "";
        }

        private void PdfViewer_PopupAnnotationAdded(object sender, PopupAnnotationAddedEventArgs e)
        {
            PopupButton.IsChecked = false;
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

        /// <summary>
        /// 关闭书签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            BookmarkGrid.Visibility = Visibility.Collapsed;
            isBookmarkClicked = false;
        }

        /// <summary>
        /// 书签点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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




        /// <summary>
        /// 读取书签
        /// </summary>
        /// <param name="ldoc"></param>
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

        /// <summary>
        /// 界面判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigatorItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            SfTreeNavigatorItem item = sender as SfTreeNavigatorItem;
            item.HeaderTemplate = Resources["HeaderTemplate1"] as DataTemplate;
        }

        /// <summary>
        /// 界面判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigatorItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            SfTreeNavigatorItem item = sender as SfTreeNavigatorItem;
            item.HeaderTemplate = Resources["HeaderTemplate2"] as DataTemplate;
        }



        /// <summary>
        /// 子书签添加
        /// </summary>
        /// <param name="bookmark"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 书签点击
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
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
            UncheckOthers(InkButton);
            pdfViewer.InkAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void InkButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.InkAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void HighlightButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(HighlightButton);
            pdfViewer.HighlightAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void HighlightButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.HighlightAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void UnderlineButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(UnderlineButton);
            pdfViewer.UnderlineAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void UnderlineButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.UnderlineAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void StrikethroughButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(StrikethroughButton);
            pdfViewer.StrikeoutAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void StrikethroughButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.StrikeoutAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void LineButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(LineButton);
            pdfViewer.LineAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void LineButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.LineAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }
        private void PopupButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(PopupButton);
            pdfViewer.PopupAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }
        private void PopupButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.PopupAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void RectangleButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(RectangleButton);
            pdfViewer.RectangleAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void RectangleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.RectangleAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void EllipseButton_Checked(object sender, RoutedEventArgs e)
        {
            UncheckOthers(EllipseButton);
            pdfViewer.EllipseAnnotationCommand.Execute(true);
            m_isButtonClicked = true;
        }

        private void EllipseButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pdfViewer.EllipseAnnotationCommand.Execute(false);
            m_isButtonClicked = true;
        }

        private void CloseAnnotations_Click(object sender, RoutedEventArgs e)
        {
            AnnotationToolbar.Visibility = Visibility.Collapsed;
            isAnnotationToolbarVisible = false;
            UncheckOthers(sender);
        }

        /// <summary>
        /// 取消button锁定
        /// </summary>
        /// <param name="sender"></param>
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

       

        /// <summary>
        /// 论文保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Save(pdfViewer.Save(), curPaperName);
            this.ViewModel.getPapers((string)AreaButton.Content);
            m_isButtonClicked = true;
        }

        /// <summary>
        /// 论文跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void DeletButton_Click(object sender, RoutedEventArgs e)
        {
            string paper = CurClickItem.name;
            this.ViewModel.deletePaper(paper);
            this.ViewModel.getPapers((string)this.AreaButton.Content);
        }

        private void ListItemDelete_Click(object sender, RoutedEventArgs e)
        {
            string paper = CurClickItem.name;
            //this.ViewModel.DeletePaperArea(paper);
            this.ViewModel.getPaperArea();
        }

        private void PaperItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.ViewModel.IsPaperAreaMenu)
            {
                string paper = CurClickItem.name;
                Debug.WriteLine(paper);
                this.ViewModel.deletePaper(paper);
            } else
            {
                string paper = CurClickItem.name;
                this.ViewModel.deletePaper(paper);
            }
        }


        /// <summary>
        /// ListView item右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PaperItemList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                ListView listView = (ListView)sender;
                ContextMenuFlyout.ShowAt(listView, e.GetPosition(listView));
                var a = ((FrameworkElement)e.OriginalSource).DataContext as PaperArea;
                CurClickItem = a;
            } catch 
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Wrong",
                    Content = "错误的操作，未选中目标",
                    CloseButtonText = "Close"
                };

                ContentDialogResult result = await noWifiDialog.ShowAsync();
            }
            
        }


        /// <summary>
        /// 通过右键删除PaperArea和Paper
        /// </summary>
        /// <param name="paperArea"></param>
        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ViewModel.IsPaperAreaMenu)
                {
                    string paperArea = CurClickItem.name;
                    List<String> papersPath = this.ViewModel.getPaperAreaPath(paperArea);
                    this.ViewModel.deletePaperArea(paperArea);
                    this.ViewModel.readPaperArea();
                    curPaperName = CurClickItem.name;
                    
                    foreach(String path in papersPath) {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                        Debug.WriteLine(path);
                        await file.DeleteAsync();
                    }
                    //PaperItemList.ItemsSource = this.ViewModel.PapersItems;
                }
                else
                {
                    string paper = CurClickItem.name;
                    string paperPath = this.ViewModel.getPdfDocument(paper);
                    StorageFile file = await StorageFile.GetFileFromPathAsync(paperPath);
                    await file.DeleteAsync();
                    this.ViewModel.deletePaper(paper);
                    this.ViewModel.getPapers((string)this.AreaButton.Content);
                    //PaperItemList.ItemsSource = this.ViewModel.PapersItems;
                }
            } catch
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Wrong",
                    Content = "错误的操作，未选中目标",
                    CloseButtonText = "Close"
                };

                ContentDialogResult result = await noWifiDialog.ShowAsync();
            }

        }

        private async void ReName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RenameDialog renameDialog = new RenameDialog();
                if (this.ViewModel.IsPaperAreaMenu)
                {
                    string paperArea = CurClickItem.name;
                    renameDialog.isPaperAreaMenu = true;
                    renameDialog.CurPaperName = paperArea;
                    await renameDialog.ShowAsync();
                    if (renameDialog.Result == RenameResult.AddOK)
                    {
                        this.ViewModel.readPaperArea();
                    }

                    curPaperName = CurClickItem.name;
                    //PaperItemList.ItemsSource = this.ViewModel.PapersItems;
                }
                else
                {
                    string paper = CurClickItem.name;

                    renameDialog.CurPaperName = paper;
                    renameDialog.isPaperAreaMenu = false;
                    string paperPath = this.ViewModel.getPdfDocument(paper);
                    StorageFile file = await StorageFile.GetFileFromPathAsync(paperPath);
                    await renameDialog.ShowAsync();
                    if (renameDialog.Result == RenameResult.AddOK)
                    {
                        this.ViewModel.getPapers((string)this.AreaButton.Content);
                    }
                }
            } catch
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Wrong",
                    Content = "错误的操作，未选中目标",
                    CloseButtonText = "Close"
                };

                ContentDialogResult result = await noWifiDialog.ShowAsync();
            }
        }
    }
}
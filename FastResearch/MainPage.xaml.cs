using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

namespace FastResearch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        public MainPage()
        {
            this.InitializeComponent();
            ChangeTitleBar();
        }
        void ChangeTitleBar()//把标题栏的颜色改变
        {
            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;//获取标题栏
            coreTitleBar.ExtendViewIntoTitleBar = true;//变没
            var appTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;//获取最大化、最小化、关闭按钮
            appTitleBar.ButtonBackgroundColor = Colors.Black;//背景变黑
        }
        private readonly  List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Tools", typeof(ToolsPage)),
            ("Papers", typeof(PapersPage)),
        };
        private void NavView_Loaded(object sender, RoutedEventArgs e) 
        {
            // You can also add items in code.
            NvFastResearch.MenuItems.Add(new NavigationViewItemSeparator());
            NvFastResearch.MenuItems.Add(new NavigationViewItem
            {
                Content = "Tools",
                Icon = new SymbolIcon((Symbol)0xF1AD),
                Tag = "Toolspage"
            });
            _pages.Add(("Tools", typeof(ToolsPage)));



            // NavView doesn't load any page by default, so load home page.
            NvFastResearch.SelectedItem = NvFastResearch.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("home", new EntranceNavigationTransitionInfo());

        }
        private void NavView_ItemInvoked(NavigationView sender,
                                         NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }
        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            var preNavPageType = ContentFrame.CurrentSourcePageType;
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }
    }
}

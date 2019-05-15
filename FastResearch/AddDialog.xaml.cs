using FastResearch.Model;
using System;
using System.Collections.Generic;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace FastResearch
{
    public enum AddResult
    {
        AddOK,
        AddCancel
    }

    public sealed partial class AddDialog : ContentDialog
    {
        public AddResult Result { get; private set; }

        public String name { get { return nameTextBox.Text; } }

        public Command command { get; set; }

        public AddDialog()
        {
            this.InitializeComponent();
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (String.IsNullOrEmpty(nameTextBox.Text))
            {
                args.Cancel = true;
                errorTextBlock.Text = "名称不能为空！";
            }
            else if (String.IsNullOrEmpty(fileTextBox.Text))
            {
                args.Cancel = true;
                errorTextBlock.Text = "文件名不能为空！";
            }
            else
            {
                command = new Command() { name = nameTextBox.Text, description = descriptionTextBox.Text, file = fileTextBox.Text };
                this.Result = AddResult.AddOK;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = AddResult.AddCancel;
        }

        private async void fileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".py");
            try
            {
                var file = await picker.PickSingleFileAsync();
                fileTextBox.Text = file.Name;
            } catch
            {
                
            }
          
        }
    }
}

using FastResearch.Services;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FastResearch
{

    public enum RenameResult
    {
        AddOK,
        AddCancel
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class RenameDialog : ContentDialog
    {

        public RenameResult Result { get; private set; }


        public Boolean isPaperAreaMenu { get; set; }
        public string CurPaperName
        {
            get; set;
        }

        private string NewPaperName
        {
            get; set;
        }
        public RenameDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (String.IsNullOrEmpty(nameTextBox.Text))
            {
                args.Cancel = true;
                errorTextBlock.Text = "名称不能为空！";
            }
            else
            {
                if(this.isPaperAreaMenu)
                {
                    NewPaperName = nameTextBox.Text;
               
                    DatabaseManager.UserDataBase.UpdatePaperArea(NewPaperName, CurPaperName);
                    this.Result = RenameResult.AddOK;
                } else
                {
                    NewPaperName = nameTextBox.Text;
                    PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
                    string paperPath = service.getPaperPath(CurPaperName);
                    
                    StorageFile file = await StorageFile.GetFileFromPathAsync(paperPath);
                    await file.RenameAsync(NewPaperName+ ".pdf");
                    DatabaseManager.UserDataBase.UpdatePaper(NewPaperName, CurPaperName);
                    DatabaseManager.UserDataBase.UpdatePaperPath(NewPaperName, file.Path);
                    this.Result = RenameResult.AddOK;
                }
               
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = RenameResult.AddCancel;
        }
    }
}

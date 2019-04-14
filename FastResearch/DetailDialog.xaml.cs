using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.ApplicationModel.DataTransfer;
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
    public enum EditResult
    {
        EditOK,
        EditCancel
    }

    public sealed partial class DetailDialog : ContentDialog
    {

        public DetailDialog(Model.Command command)
        {
            this.command = command;
            this.InitializeComponent();

        }

        public Model.Command command;
        public EditResult result;

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dp = new DataPackage();
            dp.SetText(command.GetCommand());//将当前命令对象的命令文本形式写入剪贴板
            Clipboard.SetContent(dp);
            args.Cancel = true;
            messageTextBlock.Text = "已成功复制到你的剪贴板";
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (String.IsNullOrEmpty(nameTextBox.Text))
            {
                args.Cancel = true;
                messageTextBlock.Text = "名称不能为空！";
            }
            else
            {
                result = EditResult.EditOK;
            }
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //command = old_command;//如果取消，应该恢复到修改之前的command
            result = EditResult.EditCancel;
        }
    }
}

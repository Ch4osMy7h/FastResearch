using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FastResearch.PdfReader
{
    /// <summary>
    /// 对于绑定的论文进行管理
    /// </summary>
    public static class PdfFileManger
    {
        /// <summary>
        /// 创建一个总文件夹用来管理
        /// </summary>
        public static StorageFolder RootPaperItems { get; set; }
        public static List<StorageFolder> PaperAreaFolder { get; set; }

        public static async void InitFileMange()
        {
            try
            {
                //创建总文件夹
                // Create sample file; replace if exists.
                PaperAreaFolder = new List<StorageFolder>();
                Windows.Storage.StorageFolder storageFolder =
                           Windows.Storage.ApplicationData.Current.LocalFolder;
                //Windows.Storage.StorageFile sampleFile =
                //    await storageFolder.CreateFileAsync("sample.txt",
                //        Windows.Storage.CreationCollisionOption.ReplaceExisting);
                //Debug.WrWindows.Storage.StorageFile sampleFile =
                RootPaperItems = await storageFolder.CreateFolderAsync("Root",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                Debug.WriteLine("无法创建");
            }
        }
        
        public static async void addPaperAreaFolder(string name) 
        {
            Debug.WriteLine("success load init addPaperAreaFolder");
            StorageFolder newPaperAreaFolder = await RootPaperItems.CreateFolderAsync(name);
            //这边需要添加判重机制
            PaperAreaFolder.Add(newPaperAreaFolder);
        }
    }
}

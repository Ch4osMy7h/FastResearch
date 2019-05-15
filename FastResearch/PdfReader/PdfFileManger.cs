using FastResearch.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        public static StorageFolder RootPaperItems { get; set; }
        [DataMember]
        public static List<StorageFolder> PaperAreaFolder { get; set; }
        //讲论文文件绑定
        [DataMember]
        public static List<Paper> Papers { get; set; }

        public static string PapersPath { get; set; }
        public static string PaperAreaFolderPath { get; set; }


        /// <summary>
        /// 创建文件管理的总文件夹
        /// </summary>
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
                    Windows.Storage.CreationCollisionOption.OpenIfExists);
            }
            catch
            {
                Debug.WriteLine("无法创建");
            }
        }

        /// <summary>
        /// 添加PaperArea对应的文件夹
        /// </summary>
        /// <param name="name"> 论文领域 </param>
        public static async void addPaperAreaFolder(string name) 
        {
            
            //StorageFolder newPaperAreaFolder = await RootPaperItems.CreateFolderAsync(name);
            //这边需要添加判重机制
            //PaperAreaFolder.Add(newPaperAreaFolder);
        }

        /// <summary>
        /// 查找Paper对应的Path
        /// </summary>
        /// <param name="paper"> 论文名</param>
        /// <returns></returns>
        public static StorageFile mapPaperToFile(string paper)
        {
            return Papers.Find(x => x.name == paper).paperLocation;

        }

        /// <summary>
        /// 清空数据库时候用来重建存放论文的文件夹
        /// </summary>
        public static async void rebuildRootPaper()
        {
            Windows.Storage.StorageFolder storageFolder =
                       Windows.Storage.ApplicationData.Current.LocalFolder;

            //Windows.Storage.StorageFile sampleFile =
            //    await storageFolder.CreateFileAsync("sample.txt",
            //        Windows.Storage.CreationCollisionOption.ReplaceExisting);
            //Debug.WrWindows.Storage.StorageFile sampleFile =
            RootPaperItems = await storageFolder.CreateFolderAsync("Root",
                Windows.Storage.CreationCollisionOption.OpenIfExists);
            await RootPaperItems.DeleteAsync();
            RootPaperItems = await storageFolder.CreateFolderAsync("Root",
                Windows.Storage.CreationCollisionOption.OpenIfExists);
        }

    }

}

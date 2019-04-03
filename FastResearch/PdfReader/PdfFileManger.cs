using FastResearch.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
        public static List<StorageFolder> PaperAreaFolder { get; set; }
        //讲论文文件绑定
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
                Papers = new List<Paper>();
                PaperAreaFolder = new List<StorageFolder>();
                PapersPath = RootPaperItems.Path + "\\Paper";
                PaperAreaFolderPath = RootPaperItems.Path + "\\PaperFolder";
                Debug.WriteLine(PapersPath);
                await RootPaperItems.CreateFileAsync("Paper", CreationCollisionOption.OpenIfExists);
                await RootPaperItems.CreateFileAsync("PaperFolder", CreationCollisionOption.OpenIfExists);
                SerializeFile();
                DeSerializeFile();
                Debug.WriteLine("go here");
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
            
            StorageFolder newPaperAreaFolder = await RootPaperItems.CreateFolderAsync(name);
            //这边需要添加判重机制
            PaperAreaFolder.Add(newPaperAreaFolder);
        }



        public static async Task<StorageFile> copyPaperToFileManger(StorageFile file, string paperArea, string paperName)
        {
            
            Debug.WriteLine(paperArea);
            Debug.WriteLine(PaperAreaFolder[0].Name);
            StorageFolder folder = PaperAreaFolder.Find(x => x.Name == paperArea);
            Debug.WriteLine(folder.Path);
            StorageFile newFile = null;
 
            newFile = await file.CopyAsync(folder);
            Papers.Add(new Paper() { name = paperName, paperLocation = newFile });

            return newFile;
        }

        /// <summary>
        /// 序列化 存储PaperAreaFolder以及Paper的Location
        /// </summary>
        public static async void SerializeFile()
        {
            //存储Paper 
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<Paper>));
            
 
            StorageFile file = await RootPaperItems.GetFileAsync("Paper");

            PapersPath = file.Path;
           
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                var stream = await file.OpenStreamForWriteAsync();
                Debug.WriteLine("write stream: " + stream.ToString());
                serializer.WriteObject(stream, Papers);

                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Debug.WriteLine( "File " + file.Name + " was saved.");
                }
                else
                {
                    Debug.WriteLine("File " + file.Name + " couldn't be saved.");
                   
                }
            }
            serializer = new DataContractSerializer(typeof(List<StorageFolder>));
            file = await RootPaperItems.GetFileAsync("PaperFolder");
            PaperAreaFolderPath = file.Path;

            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                var stream = await file.OpenStreamForWriteAsync();
                Debug.WriteLine("write stream: " + stream.ToString());
                serializer.WriteObject(stream, PaperAreaFolder);

                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Debug.WriteLine("File " + file.Name + " was saved.");
                }
                else
                {
                    Debug.WriteLine("File " + file.Name + " couldn't be saved.");

                }
            }




        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns> Papers对应文件</returns>
        public static async void DeSerializeFile()
        {
            DataContractSerializer deserializer = new DataContractSerializer(typeof(List<Paper>));
            StorageFile storageFile = await RootPaperItems.GetFileAsync("Paper");
            
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForReadAsync();

                Papers = deserializer.ReadObject(stream) as List<Paper>;
            }
            else
            {
                Debug.WriteLine("反序列化失败");
            }
            Debug.WriteLine("成功读取Paper");
            deserializer = new DataContractSerializer(typeof(List<StorageFolder>));
            storageFile = await StorageFile.GetFileFromPathAsync(PaperAreaFolderPath);
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForReadAsync();
                PaperAreaFolder = deserializer.ReadObject(stream) as List<StorageFolder>;
            }
            else
            {
                Debug.WriteLine("反序列化失败");
            }

        }

        public static StorageFile mapPaperToFile(string paper)
        {
            return Papers.Find(x => x.name == paper).paperLocation;

        }

    }

}

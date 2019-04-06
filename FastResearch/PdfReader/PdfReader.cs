using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using FastResearch.PdfReader;
using System.Diagnostics;
using System.IO;
using Syncfusion.Pdf.Parsing;

namespace FastResearch.PdfReader
{
    public static class PdfReader
    {
        /// <summary>
        /// 显示pdf页面
        /// </summary>
        /// <param name="paperArea"> 论文领域</param>
        /// <param name="paperName"> 论文名字</param>
        /// <returns></returns>
        public static async Task<PdfLoadedDocument> LoadDocument(StorageFile file, String paperArea, String paperName)
        {
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            Stream fileStream = stream.AsStreamForRead();
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(buffer);
            return loadedDocument;

           // PdfDocument pdf = await PdfDocument.LoadFromFileAsync(file);
           // //PdfFileManger.copyPaperToFileManger(file, paperArea,  paperName);
           // // 获取PDF文档的总页数
           // uint pageCount = pdf.PageCount;

            // List<BitmapImage> pageimages = new List<BitmapImage>();

            // //设置pdf



            // // 获取页面列表
            // for (uint p = 0; p < pageCount; p++)
            // {
            //     Debug.WriteLine(p);
            //     PdfPage page = pdf.GetPage(p);

            //     await page.PreparePageAsync();
            //     // 将页面内容保存为图像
            //     InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
            //     await page.RenderToStreamAsync(ms);
            //     BitmapImage bmp = new BitmapImage();
            //     // 设置图像宽度
            //     //bmp.DecodePixelWidth =;
            //     bmp.SetSource(ms);
            //     // 释放资源
            //     ms.Dispose();
            //     page.Dispose();
            //     pageimages.Add(bmp);
            // }
            // Debug.WriteLine("go here");
            //return pageimages;
        }
    }
}

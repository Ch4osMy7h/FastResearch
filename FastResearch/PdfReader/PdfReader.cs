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

namespace FastResearch.PdfReader
{
    class PdfReader
    {
        public async Task<List<BitmapImage>> loadDocument(StorageFile pdfFile)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            StorageFile file = await picker.PickSingleFileAsync();

            PdfDocument pdf = await PdfDocument.LoadFromFileAsync(pdfFile);

            // 获取PDF文档的总页数
            uint pageCount = pdf.PageCount;

            List<BitmapImage> pageimages = new List<BitmapImage>();
            // 获取页面列表
            for (uint p = 0; p < pageCount; p++)
            {
                PdfPage page = pdf.GetPage(p);
                await page.PreparePageAsync();
                // 将页面内容保存为图像
                InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(ms);
                BitmapImage bmp = new BitmapImage();
                // 设置图像宽度
                bmp.DecodePixelWidth = 2000;
                bmp.SetSource(ms);
                // 释放资源
                ms.Dispose();
                page.Dispose();
                pageimages.Add(bmp);
            }
           return pageimages;
        }
    }
}

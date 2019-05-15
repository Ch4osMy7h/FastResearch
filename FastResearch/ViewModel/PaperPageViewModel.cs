using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FastResearch;
using FastResearch.Model;
using FastResearch.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Windows.Storage;
using Windows.UI.Popups;

namespace FastResearch
{
    /// <summary>
    /// 论文领域类
    /// </summary>
  
    public class PaperAreaViewModel : ViewModelBase
    {

        private ObservableCollection<PaperArea> papersItems = new ObservableCollection<PaperArea>();
        public bool IsPaperAreaMenu { get; set; }


        /// <summary>
        /// 获取PaperAreal类
        /// </summary>

        public PaperAreaViewModel()
        {
            IsPaperAreaMenu = false;
            //PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            //List<String> PaperAreaName = service.PaperArea();
            //foreach (string paper_name in PaperAreaName)
            //{
            //    this.PaperAreas.Add(paper_name);
            //}
        }

        /// <summary>
        /// 获得论文领域的第一篇论文
        /// </summary>
        /// <returns></returns>
        public string getPaperAreaFirstOrNot()
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            List<String> paperArea = service.PaperArea();
            string name;
            if (paperArea.Count != 0)
            {
                name = paperArea[0];
            }
            else
            {
                name = "Null";
            }
            return name;
        }


        public ObservableCollection<PaperArea> PapersItems
        {
            get
            {
                return this.papersItems;
            }
            set => this.papersItems = value;
        }
        public StorageFolder rootLocalDataFolder
        {
            get
            {
                return PdfReader.PdfFileManger.RootPaperItems;
            }
        }


        /// <summary>
        /// 添加论文领域
        /// </summary>
        /// <param name="paperArea"> 论文领域 </param>
        /// <returns></returns>
        public bool addPaperArea(String paperArea)
        {
            List<String> curPaperArea = getPaperArea();
            if(curPaperArea.Find(x => x == paperArea) != null)
            {
                return false;
            }
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            
            service.AddPaperArea(paperArea);
            PdfReader.PdfFileManger.addPaperAreaFolder(paperArea);
            return true;
        }


        /// <summary>
        /// 获得论文对应的路径
        /// </summary>
        /// <param name="paper"> 论文名</param>
        /// <returns></returns>
        public string getPdfDocument(string paper)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            return service.getPaperPath(paper);
        }

        /// <summary>
        /// 更新论文路径
        /// </summary>
        /// <param name="paper"> 论文名</param>
        /// <param name="paperPath"> 论文路径</param>
        /// <returns></returns>
        public bool updatePaperPath(string paper, string paperPath)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            return service.updatePaperPath(paper, paperPath);
        }

        /// <summary>
        /// 添加论文
        /// </summary>
        /// <param name="paperName"> 论文名</param>
        /// <param name="paperArea"> 论文领域</param>
        /// <param name="paperPath"> 论文路径</param>
        /// <returns></returns>
        public bool addPaper(String paperName, String paperArea, String paperPath)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            List<string> papers = service.getPapers(paperArea);
            if(papers.Find(x => x == paperName) != null)
            {
                return false;
            }
            service.AddPaper(paperName, paperArea, paperPath);
            return true;
        }

        /// <summary>
        /// 获得论文领域
        /// </summary>
        public void readPaperArea()
        {
            try
            {
                PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
                List<String> PaperAreaName = service.PaperArea();
                //Debug.WriteLine(PaperAreaName.Count);
                //this.papersItems = new ObservableCollection<PaperArea>();
                this.papersItems.Clear();
                foreach (string paper_name in PaperAreaName)
                {
                    this.papersItems.Add(new PaperArea { name = paper_name });
                }
            } catch
            {
                Debug.WriteLine("读取error");
            }
        }

        /// <summary>
        /// 获得论文领域
        /// </summary>
        /// <returns></returns>
        public List<String> getPaperArea()
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            List<String> paperAreas = service.PaperArea();
            return paperAreas;
        }

        /// <summary>
        /// 获得paperArea对应所有论文
        /// </summary>
        /// <param name="paperArea"></param>
        public void getPapers(string paperArea)
        {
            try
            {
                PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
                List<string> papers = service.getPapers(paperArea);
                this.papersItems.Clear();
                foreach (string paper_name in papers)
                {
                    this.papersItems.Add(new PaperArea { name = paper_name });
                }
            } catch
            {
                Debug.WriteLine("读取error");
            }
        }

        /// <summary>
        /// 保存修改过的论文
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="curPaperName"></param>
        internal async void Save(Stream stream, string curPaperName)
        {
            string paperPath = this.getPdfDocument(curPaperName);
            StorageFile stFile = await StorageFile.GetFileFromPathAsync(paperPath);

            if (stFile != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream st = fileStream.AsStreamForWrite();
                st.SetLength(0);
                st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                st.Flush();
                st.Dispose();
                fileStream.Dispose();
                MessageDialog msgDialog = new MessageDialog("File has been saved successfully.");
                IUICommand cmd = await msgDialog.ShowAsync();
            }
            
        }



        /// <summary>
        /// 删除论文
        /// </summary>
        /// <param name="paper"></param>
        public void deletePaper(string paper)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            service.deletePaper(paper);
        }

        /// <summary>
        /// 删除论文领域名
        /// </summary>
        /// <param name="paperArea"></param>
        public void deletePaperArea(string paperArea)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            service.deletePaperArea(paperArea);
        }

        public List<String> getPaperAreaPath(string paperArea)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            return service.getPaperAreaPath(paperArea);
        }

        //深复制
        public T DeepCopy<T>(T obj)
        {
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}

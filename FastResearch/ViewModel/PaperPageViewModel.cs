using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public ObservableCollection<PaperArea> papersItems = new ObservableCollection<PaperArea>();
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
        }
        public StorageFolder rootLocalDataFolder
        {
            get
            {
                return PdfReader.PdfFileManger.RootPaperItems;
            }
        }
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

        public string getPdfDocument(string paper)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            return service.getPaperPath(paper);
        }

        public bool updatePaperPath(string paper, string paperPath)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            return service.updatePaperPath(paper, paperPath);
        }

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
                    this.papersItems.Add(new PaperArea { _name = paper_name });
                }
            } catch
            {
                Debug.WriteLine("读取error");
            }
        }
        public List<String> getPaperArea()
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            List<String> paperAreas = service.PaperArea();
            return paperAreas;
        }
        public void getPapers(string paperArea)
        {
            try
            {
                PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
                List<string> papers = service.getPapers(paperArea);
                this.papersItems.Clear();
                foreach (string paper_name in papers)
                {
                    this.papersItems.Add(new PaperArea { _name = paper_name });
                }
            } catch
            {
                Debug.WriteLine("读取error");
            }
        }

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



        internal void Bind(string text)
        {
            //throw new NotImplementedException();
        }

        public void Delete(string paper)
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            service.deletePaper(paper);
        }
    }
}

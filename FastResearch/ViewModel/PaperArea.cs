using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastResearch;
using FastResearch.Model;
using FastResearch.Services;
using GalaSoft.MvvmLight.Ioc;

namespace FastResearch
{
    /// <summary>
    /// 论文领域类
    /// </summary>
  
    public class PaperAreaViewModel
    {

        private ObservableCollection<PaperArea> paperareas = new ObservableCollection<PaperArea>();
        private PaperAreaService service;
        /// <summary>
        /// 获取PaperAreal类
        /// </summary>
        public ObservableCollection<PaperArea> PaperAreas
        {
            
            get
            {
                return this.paperareas;
            }
        }
        public PaperAreaViewModel()
        {
            PaperAreaService service = SimpleIoc.Default.GetInstance<PaperAreaService>();
            List<String> PaperAreaName = service.PaperArea();
            foreach (string paper_name in PaperAreaName)
            {
                this.PaperAreas.Add(new PaperArea() { _name = paper_name });
            }
        }
        public void addPaperArea(String PaperName)
        {
            service.AddPaperArea(PaperName);
        }
    }
}

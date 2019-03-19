using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch
{
    /// <summary>
    /// 论文领域类
    /// </summary>
    public class PaperArea
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PaperArea ()
        {
            this.Name = "None";
            this.Description = "None";
        }
    }

    public class PaperAreaViewModel
    {

        private ObservableCollection<PaperArea> paperareas = new ObservableCollection<PaperArea>();
        public ObservableCollection<PaperArea> PaperAreas { get { return this.paperareas; } }
        public PaperAreaViewModel() {
            this.paperareas.Add(new PaperArea() { Name = "TransE", Description = "Thunlp" });
            this.paperareas.Add(new PaperArea() { Name = "TransR", Description = "Thunlp" });
            this.paperareas.Add(new PaperArea() { Name = "TransD", Description = "Thunlp" });
        }
    }
}

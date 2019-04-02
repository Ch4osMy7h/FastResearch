using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastResearch;

namespace FastResearch.Services
{
    /// <summary>
    /// PaperArea服务类 用于从数据库从检索出所有的PaperArea 以及对数据库中的增删改查
    /// </summary>
    class PaperAreaService
    {
        private List<string> _paperArea = new List<string>();
        public List<string> PaperArea()
        {


                _paperArea = DatabaseManager.UserDataBase.getPaperArea();            
                return _paperArea;
        }
        public void AddPaperArea(string paperArea)
        {
            DatabaseManager.UserDataBase.addPaperArea(paperArea);
        }
        public void AddPaper(string paper, string paperArea)
        {
            DatabaseManager.UserDataBase.addPaper(paperArea, paper);
        }
        public List<String> getPapers(string paperArea)
        {
            return DatabaseManager.UserDataBase.GetPaperName(paperArea);
        }
    }
}

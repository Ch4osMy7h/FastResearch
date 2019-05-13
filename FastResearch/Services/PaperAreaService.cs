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
        public void AddPaper(string paper, string paperArea, string paperPath)
        {
            DatabaseManager.UserDataBase.addPaper(paperArea, paper, paperPath);
        }
        public List<String> getPapers(string paperArea)
        {
            return DatabaseManager.UserDataBase.GetPaperName(paperArea);
        }
        public string getPaperPath(string paper)
        {
            return DatabaseManager.UserDataBase.GetPaperPath(paper);
        }

        public bool updatePaperPath(string paper, string paperPath)
        {
            return DatabaseManager.UserDataBase.UpdatePaperPath(paper, paperPath);
        }

        public void deletePaper(string paper)
        {
            DatabaseManager.UserDataBase.deletePaper(paper);
        }

    
        public void deletePaperArea(string paperArea)
        {
          
            DatabaseManager.UserDataBase.deletePaperArea(paperArea);
        }
    }
}

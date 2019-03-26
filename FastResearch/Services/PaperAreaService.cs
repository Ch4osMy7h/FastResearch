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


                _paperArea = DatabaseManager.UserDataBase.GetPaperArea();            
                return _paperArea;
        }
        public void AddPaperArea(string PaperArea)
        {
            DatabaseManager.UserDataBase.AddData(PaperArea);
        }
    }
}

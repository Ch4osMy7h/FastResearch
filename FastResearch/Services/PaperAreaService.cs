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

        /// <summary>
        /// 加入论文领域
        /// </summary>
        /// <param name="paperArea"></param>
        public void AddPaperArea(string paperArea)
        {
            DatabaseManager.UserDataBase.addPaperArea(paperArea);
        }

        /// <summary>
        /// 加入论文
        /// </summary>
        /// <param name="paper"> 论文名</param>
        /// <param name="paperArea"> 论文领域</param>
        /// <param name="paperPath"> 论文存放路径</param>
        public void AddPaper(string paper, string paperArea, string paperPath)
        {
            DatabaseManager.UserDataBase.addPaper(paperArea, paper, paperPath);
        }

        /// <summary>
        /// 查看论文领域
        /// </summary>
        /// <param name="paperArea"></param>
        /// <returns></returns>
        public List<String> getPapers(string paperArea)
        {
            return DatabaseManager.UserDataBase.GetPaperName(paperArea);
        }

        /// <summary>
        /// 查看论文路径
        /// </summary>
        /// <param name="paper">论文名</param>
        /// <returns></returns>
        public string getPaperPath(string paper)
        {
            return DatabaseManager.UserDataBase.GetPaperPath(paper);
        }


        /// <summary>
        /// 更新论文路径
        /// </summary>
        /// <param name="paper">论文名</param>
        /// <param name="paperPath"> 论文路径</param>
        /// <returns></returns>
        public bool updatePaperPath(string paper, string paperPath)
        {
            return DatabaseManager.UserDataBase.UpdatePaperPath(paper, paperPath);
        }


        /// <summary>
        /// 删除论文
        /// </summary>
        /// <param name="paper"></param>
        public void deletePaper(string paper)
        {
            DatabaseManager.UserDataBase.deletePaper(paper);
        }


        /// <summary>
        /// 删除论文领域
        /// </summary>
        /// <param name="paperArea"></param>
        public void deletePaperArea(string paperArea)
        {
          
            DatabaseManager.UserDataBase.deletePaperArea(paperArea);
        }

        /// <summary>
        /// 查看论文领域
        /// </summary>
        /// <param name="paperArea"></param>
        /// <returns></returns>
        public List<String> getPaperAreaPath(string paperArea)
        {
            return DatabaseManager.UserDataBase.getPaperAreaPath(paperArea);
        }

    }
}

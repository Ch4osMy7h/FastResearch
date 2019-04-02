using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastResearch.Model;

namespace FastResearch.Model
{
    public class PaperArea
    {
        public string _name { get; set; }
        public string _description { get; set; }
        public List<Paper> _papers { get; set; }
        public PaperArea()
        {
            this._name = "None";
            this._description = "None";
        }
    }
}

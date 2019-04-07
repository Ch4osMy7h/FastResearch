using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch.Model
{
    public class Command
    {
        public string name;
        public string executable;
        public string file;
        public List<Tuple<string, string>> args;
        public string description;
    }
}

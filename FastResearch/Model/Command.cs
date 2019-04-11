using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastResearch.Model
{
    [Serializable]
    public class Command
    {
        public string name;
        public string executable;
        public string file;
        public List<Tuple<string, string>> options;
        public string description;
        public string GetCommand() => executable + " " + file + " " +   
            (options != null && options.Count > 0 ? 
            options.Select(it => new string("-" + it.Item1 + " " + it.Item2)).ToList().Aggregate((acc, item) => acc + " " + item) : "");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FastResearch.Model
{
    [Serializable]
    public class Paper
    {
        public string name;
        public StorageFile paperLocation;
        public Paper()
        {
           this.name = "";
           this.paperLocation = null;
        }
    }
}

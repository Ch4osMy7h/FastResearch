using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FastResearch.Model
{
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

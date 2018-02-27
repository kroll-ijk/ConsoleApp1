using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Result
    {
        string pPath;
        string pHash;

        public string Path
        {
            get { return pPath; }
            set { }
        }
        public string Hash
        {
            get { return pHash; }
            set { }
        }
        public Result( string _path, string _hash )
        {
            pPath = _path;
            pHash = _hash;

        }

    }
}

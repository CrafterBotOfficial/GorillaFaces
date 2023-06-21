using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GorillaFaces.Models
{
    public class Package
    {
        public string Name { get; set; }
        public string Author { get; set; }

        public Package(string name, string author)
        {
            Name = name;
            Author = author;
        }
    }
}

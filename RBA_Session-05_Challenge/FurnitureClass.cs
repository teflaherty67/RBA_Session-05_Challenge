using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RBA_Session_05_Challenge
{
    internal class FurnitureSet
    { 
        public string Set { get; set; }
        public string Room { get; set; }
        public string[] Furniture { get; set; }
        public FurnitureSet(string set, string name, string furnitureList)
        {
            Set = set;
            Room = name;
            Furniture = furnitureList.Split(',');
        }
    }

    internal class FurnitureType
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string Type { get; set; }
        public FurnitureType(string name, string family, string type)
        {
            Name = name;
            Family = family;
            Type = type;
        } 
    }

}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    class Table
    {
        BlockingCollection<Glass> Glasses = new BlockingCollection<Glass>(); 
    }
}

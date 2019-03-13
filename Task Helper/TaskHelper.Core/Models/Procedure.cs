using System;
using System.Collections.Generic;
using System.Text;

namespace TaskHelper.Core.Models
{
    public class Procedure
    {
        public int id { get; set; }
        public int procedure_id { get; set; }
        public int group_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
    }
}

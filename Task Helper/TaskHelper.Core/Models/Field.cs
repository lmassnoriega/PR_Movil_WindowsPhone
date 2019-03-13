using System;
using System.Collections.Generic;
using System.Text;

namespace TaskHelper.Core.Models
{
    public class Field
    {
        public int id { get; set; }
        public string caption { get; set; }
        public string field_type { get; set; }
        public List<string> possible_values { get; set; }
    }
}

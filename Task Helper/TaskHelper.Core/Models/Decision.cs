using System;
using System.Collections.Generic;
using System.Text;

namespace TaskHelper.Core.Models
{
    public class Decision
    {
        public List<Branch> branch { get; set; }
        public string go_to_step { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TaskHelper.Core.Models
{
    public class Step
    {
        public int id { get; set; }
        public int step_id { get; set; }
        public int procedure_id { get; set; }
        public string content { get; set; }
        public StepInfo content2 { get; set; }
        public string url { get; set; }
    }
}

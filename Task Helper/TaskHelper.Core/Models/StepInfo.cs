using System;
using System.Collections.Generic;
using System.Text;

namespace TaskHelper.Core.Models
{
    public class StepInfo
    {
        public List<Field> Fields { get; set; }
        public List<Decision> Decisions { get; set; }
    }
}

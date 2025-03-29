using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Model
{
    public class CalculatorSettings
    {
        public bool DigitGroupingEnabled { get; set; }
        public bool IsProgrammerMode { get; set; }
        public int SelectedBase { get; set; }
    }
}

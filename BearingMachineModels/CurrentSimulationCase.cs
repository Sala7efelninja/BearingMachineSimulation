using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingMachineModels
{
    public class CurrentSimulationCase
    {
        public CurrentSimulationCase()
        {

        }
        public int index { get; set; }
        public Bearing Bearing { get; set; }
        public int BearingIndex {get;set; }
        public int AccumulatedHours { get; set; }
        public int Hours { get; set; }
        public int HoursR { get; set; }
        public int RandomDelay { get; set; }
        public int Delay { get; set; }        
    }
}

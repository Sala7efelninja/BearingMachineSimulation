using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BearingMachineModels
{
    public class SimSys:SimulationSystem
    {
        public SimSys()
        {
            this.DelayTimeDistribution = new List<TimeDistribution>();
            BearingLifeDistribution = new List<TimeDistribution>();

            CurrentSimulationTable = new List<CurrentSimulationCase>();
            CurrentPerformanceMeasures = new PerformanceMeasures();

            ProposedSimulationTable = new List<ProposedSimulationCase>();
            ProposedPerformanceMeasures = new PerformanceMeasures();
           
        }
        public void startSimulation(string fileName)
        {
            inputFromFile(fileName);
        }
        void inputFromFile(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            DowntimeCost = Int32.Parse(lines[1]);
            RepairPersonCost = Int32.Parse(lines[4]);
            BearingCost = Int32.Parse(lines[7]);
            NumberOfHours = Int32.Parse(lines[10]);
            NumberOfBearings = Int32.Parse(lines[13]);
            RepairTimeForOneBearing = Int32.Parse(lines[16]);
            RepairTimeForAllBearings = Int32.Parse(lines[19]);
            string[] dd;
            //Filling Bearing Life Distribustio
            for(int i=0;i<3;i++)
            {
                dd = lines[22 + i].Split(',');
                BearingLifeDistribution.Add(new TimeDistribution());
                BearingLifeDistribution[i].Time = Int32.Parse(dd[0]);
                BearingLifeDistribution[i].Probability = Int32.Parse(dd[1]);
            }
            //Filling Dely Time Distribution
            for (int i = 0;i< 10; i++)
            {
                dd = lines[22 + 2 + 3 + i].Split(',');
                DelayTimeDistribution.Add(new TimeDistribution());
                DelayTimeDistribution[i].Time = Int32.Parse(dd[0]);
                DelayTimeDistribution[i].Probability = Int32.Parse(dd[1]);
            }
        }

    }
}

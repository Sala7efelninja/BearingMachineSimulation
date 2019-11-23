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
        List<List<int>> random;
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
            setBearingLifeDistribution();
            setDelayTimeDIstribution();
            generateRandomNumbers();
            runCurrentPolicy();
        }
        private void inputFromFile(string fileName)
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

            //Filling Dely Time Distribution
            for (int i = 0; i < 3; i++)
            {
                dd = lines[22 + i].Split(',');
                DelayTimeDistribution.Add(new TimeDistribution());
                DelayTimeDistribution[i].Time = Int32.Parse(dd[0]);
                DelayTimeDistribution[i].Probability = Decimal.Parse(dd[1]);
            }

            //Filling Bearing Life Distribustio
            for (int i=0;i<10;i++)
            {
                dd = lines[22 + 2 + 3 + i].Split(',');
                BearingLifeDistribution.Add(new TimeDistribution());
                BearingLifeDistribution[i].Time = Int32.Parse(dd[0]);
                BearingLifeDistribution[i].Probability = Decimal.Parse(dd[1]);
            }
        }
        
        private void setBearingLifeDistribution()
        {
            Decimal C = 0;
            int m = 1;
            for (int i = 0; i < BearingLifeDistribution.Count; i++)
            {
                C += BearingLifeDistribution[i].Probability;
                BearingLifeDistribution[i].CummProbability = C;
                BearingLifeDistribution[i].MinRange = m;
                BearingLifeDistribution[i].MaxRange = Decimal.ToInt32(C * 100);
                m = BearingLifeDistribution[i].MaxRange + 1;
            }
        }

        private void setDelayTimeDIstribution()
        {
            Decimal C = 0;
            int m = 1;
            for (int i = 0; i < DelayTimeDistribution.Count; i++)
            {
                C += DelayTimeDistribution[i].Probability;
                DelayTimeDistribution[i].CummProbability = C;
                DelayTimeDistribution[i].MinRange = m;
                DelayTimeDistribution[i].MaxRange = Decimal.ToInt32(C*10);
                m = DelayTimeDistribution[i].MaxRange + 1;

            }
        }

        private void generateRandomNumbers()
        {
            int b1, b2, b3;
            b1 = b2 = b3 = 0;
            Random r1 = new Random();
            Random r2 = new Random();
            Random r3 = new Random();
            random = new List<List<int>>(NumberOfBearings);
            while (b1< NumberOfHours || b2< NumberOfHours || b3< NumberOfHours)
            {
                if(b1<NumberOfHours)
                {
                    random[0].Add(r1.Next(1, 100));
                    b1 += getBearingLifeTime(random[0][random[0].Count-1]);
                }
                if(b2< NumberOfHours)
                {
                    random[1].Add(r2.Next(1, 100));
                    b2 += getBearingLifeTime(random[1][random[1].Count - 1]);
                }
                if (b3< NumberOfHours)
                {
                    random[2].Add(r1.Next(1, 100));
                    b3 += getBearingLifeTime(random[2][random[2].Count - 1]);
                }
            }
        }

        private int getBearingLifeTime(int random)
        {
            for(int i=0;i<BearingLifeDistribution.Count;i++)
            {
                if (BearingLifeDistribution[i].MinRange <= random && BearingLifeDistribution[i].MaxRange >= random)
                    return BearingLifeDistribution[i].Time;
            }
            return 0;
        }

        private void runCurrentPolicy()
        {
            int b1 = 0, b2 = 0, b3 = 0;
            while (b1<NumberOfHours||b2<Number)
            {

            }
        }
        
    }
}

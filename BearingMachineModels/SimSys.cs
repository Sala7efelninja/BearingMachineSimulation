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
        List<List<int>> randomHours;
        Random r;
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
            runProposedPolicy();

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
            for (int i=0;i<lines.Length-22-2-3;i++)
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
            randomHours = new List<List<int>>(NumberOfBearings);
            while (b1< NumberOfHours || b2< NumberOfHours || b3< NumberOfHours)
            {
                if(b1<NumberOfHours)
                {
                    randomHours[0].Add(r1.Next(1, 100));
                    b1 += getBearingLifeTime(randomHours[0][randomHours[0].Count-1]);
                }
                if(b2< NumberOfHours)
                {
                    randomHours[1].Add(r2.Next(1, 100));
                    b2 += getBearingLifeTime(randomHours[1][randomHours[1].Count - 1]);
                }
                if (b3< NumberOfHours)
                {
                    randomHours[2].Add(r1.Next(1, 100));
                    b3 += getBearingLifeTime(randomHours[2][randomHours[2].Count - 1]);
                }
            }
        }

        private int getBearingLifeTime(int randomHour)
        {
            return getValue(BearingLifeDistribution, randomHour);
        }

        private void runCurrentPolicy()
        {
            int b1 = 0, b2 = 0, b3 = 0;
            r = new Random();
           
            for (int i=0; b1 < NumberOfHours || b2 < NumberOfHours || b3 < NumberOfHours;i++) 
            {
                if (b1 < NumberOfHours)
                {
                    b1 += replaceOneBearing(0, i, b1);
                }
                if(b2<NumberOfHours)
                {
                    b2 += replaceOneBearing(1, i, b2);
                }
                if(b3<NumberOfHours)
                {
                    b3 += replaceOneBearing(2, i, b3);
                }

            }
        }
        private int replaceOneBearing(int index,int i,int b)
        {
            CurrentSimulationCase currentSimulationCase = new CurrentSimulationCase();
            
            currentSimulationCase.Bearing = getBearing(index, i); 
            b += currentSimulationCase.Bearing.Hours;
            currentSimulationCase.AccumulatedHours = b;
            currentSimulationCase.RandomDelay = r.Next(1, 10);//#TODO it should be 0 , 9 ??
            currentSimulationCase.Delay = getDelay(currentSimulationCase.RandomDelay);
            CurrentSimulationTable.Add(currentSimulationCase);
            return b;
        }
        private int getDelay(int randomDelay)
        {
            return getValue(DelayTimeDistribution,randomDelay);
        }

        private int getValue(List<TimeDistribution> list,int random)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].MinRange <= random && list[i].MaxRange >= random)
                    return list[i].Time;
            }
            return 0;

        }
        private Bearing getBearing(int index, int i)
        {
            Bearing bearing = new Bearing();
            bearing.Index=index + 1;
            bearing.RandomHours = randomHours[index][i];
            bearing.Hours = getBearingLifeTime(bearing.RandomHours);
            return bearing;
            
        }
        private int getFirstFaliure(List<Bearing> L)
        {
            return Math.Min(L[0].Hours, Math.Min(L[1].Hours, L[2].Hours));
        }

        private void runProposedPolicy()
        {
            int c = 0, hours = 0;
            r = new Random();
            for(int i =0; c < NumberOfHours; i++)
            {
                // need to add more Random numbers
                ProposedSimulationCase proposedSimulationCase = new ProposedSimulationCase();
                proposedSimulationCase.Bearings.Add(getBearing(0, i));
                proposedSimulationCase.Bearings.Add(getBearing(1, i));
                proposedSimulationCase.Bearings.Add(getBearing(2, i));

                proposedSimulationCase.FirstFailure = getFirstFaliure(proposedSimulationCase.Bearings);
                c += proposedSimulationCase.FirstFailure;
                proposedSimulationCase.AccumulatedHours = c;
                proposedSimulationCase.RandomDelay = r.Next(1, 10);
                proposedSimulationCase.Delay = getDelay(proposedSimulationCase.RandomDelay);
                ProposedSimulationTable.Add(proposedSimulationCase);
            }

        }

        private void calCurrentPerformanceMeasures()
        {
            CurrentPerformanceMeasures.BearingCost = calBearingCost(CurrentSimulationTable.Count);
            CurrentPerformanceMeasures.DelayCost = calDelayCost();
            int n = calDowntime();
            CurrentPerformanceMeasures.DowntimeCost = n * DowntimeCost;
            CurrentPerformanceMeasures.RepairPersonCost = n * DowntimeCost;
            CurrentPerformanceMeasures.TotalCost = calToatalCost(CurrentPerformanceMeasures);
        }

        private decimal calDelayCost()
        {
            throw new NotImplementedException();
        }

        private int calDowntime()
        {
            throw new NotImplementedException();
        }

        private decimal calToatalCost(PerformanceMeasures performanceMeasures)
        {
            return performanceMeasures.BearingCost + performanceMeasures.DelayCost + performanceMeasures.DowntimeCost + performanceMeasures.RepairPersonCost;
        }

        
        private decimal calBearingCost(int count)
        {
            return count*BearingCost;
            
        }
    }


}

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

            randomHours = new List<List<int>>();
           
        }
        public void startSimulation(string fileName)
        {
            inputFromFile(fileName);
            setBearingLifeDistribution();
            setDelayTimeDIstribution();
            generateRandomNumbers();
            runCurrentPolicy();
            runProposedPolicy();
            calCurrentPerformanceMeasures();
            calProposedPerformanceMeasures();

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
            int[] b = new int[NumberOfBearings];
            r=new Random();
            bool done = false;
            randomHours = new List<List<int>>();
            for (int i = 0; i < NumberOfBearings; i++)
                randomHours.Add(new List<int>());

            while (!done)
            {
                done = true;
                for(int i=0;i<NumberOfBearings;i++)
                {
                    if (b[i] < NumberOfHours)
                    {
                        done = false;
                        randomHours[i].Add(r.Next(1, 101));
                        b[i] += getBearingLifeTime(randomHours[i][randomHours[i].Count - 1]);
                    }
                }
            }
        }

        private int getBearingLifeTime(int randomHour)
        {
            return getValue(BearingLifeDistribution, randomHour);
        }

        private void runCurrentPolicy()
        {
            int[] b=new int[NumberOfBearings];
            bool done = false;
            r = new Random();
           
            for (int i=0;!done;i++) 
            {
                done = true;
                for(int j=0;j<NumberOfBearings;j++)
                {
                    if (b[j] < NumberOfHours)
                    {
                        done = false;
                        b[j] = replaceOneBearing(j, i, b[j]);
                    }
                }
            }
        }
        private int replaceOneBearing(int index,int i,int b)
        {
            CurrentSimulationCase currentSimulationCase = new CurrentSimulationCase();
            
            currentSimulationCase.Bearing = getBearing(index, i); 
            b += currentSimulationCase.Bearing.Hours;
            currentSimulationCase.AccumulatedHours = b;
            currentSimulationCase.RandomDelay = r.Next(1, 11);//#TODO it should be 0 , 9 ??
            currentSimulationCase.Delay = getDelay(currentSimulationCase.RandomDelay);
            currentSimulationCase.BearingIndex = currentSimulationCase.Bearing.Index;
            currentSimulationCase.Hours = currentSimulationCase.Bearing.Hours;
            currentSimulationCase.HoursR = currentSimulationCase.Bearing.RandomHours;
            currentSimulationCase.index = i;
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
            int min = int.MaxValue;
            for (int i = 0; i < NumberOfBearings; i++)
                min = Math.Min(min, L[i].Hours);
            return min;
        }

        private void runProposedPolicy()
        {
            int c = 0, hours = 0;
            r = new Random();
            for(int i =0; c < NumberOfHours; i++)
            {
                // need to add more Random numbers
                ProposedSimulationCase proposedSimulationCase = new ProposedSimulationCase();
                for(int j=0;j<NumberOfBearings;j++)
                {
                    addRandomNumber(j, i);
                    proposedSimulationCase.Bearings.Add(getBearing(j, i));
                }
                
                proposedSimulationCase.FirstFailure = getFirstFaliure(proposedSimulationCase.Bearings);
                c += proposedSimulationCase.FirstFailure;
                proposedSimulationCase.AccumulatedHours = c;
                proposedSimulationCase.RandomDelay = r.Next(1, 11);
                proposedSimulationCase.Delay = getDelay(proposedSimulationCase.RandomDelay);
                ProposedSimulationTable.Add(proposedSimulationCase);
            }

        }

        private void addRandomNumber(int j, int i)
        {
            if(randomHours[j].Count<=i)
            {
               
                randomHours[j].Add(r.Next(0, 101));
            }
        }

        private void calCurrentPerformanceMeasures()
        {
            int noOfBearings = CurrentSimulationTable.Count ;

            CurrentPerformanceMeasures.BearingCost = noOfBearings * BearingCost;
            CurrentPerformanceMeasures.DelayCost = calCurrentDelayCost();

            int downTime = noOfBearings * RepairTimeForOneBearing;//one Bearing down takes 20 mins

            CurrentPerformanceMeasures.DowntimeCost = downTime * DowntimeCost;
            CurrentPerformanceMeasures.RepairPersonCost = calRepaireCost(downTime);
            CurrentPerformanceMeasures.TotalCost = calToatalCost(CurrentPerformanceMeasures);
        }
        private void calProposedPerformanceMeasures()
        {
            int noOfBearings = ProposedSimulationTable.Count * NumberOfBearings;

            ProposedPerformanceMeasures.BearingCost = noOfBearings * BearingCost;
            ProposedPerformanceMeasures.DelayCost = calProposedDelayCost();

            int downTime = ProposedSimulationTable.Count * RepairTimeForAllBearings; //one Bearing down takes 20 mins

            ProposedPerformanceMeasures.DowntimeCost = downTime * DowntimeCost;
            ProposedPerformanceMeasures.RepairPersonCost = calRepaireCost(downTime);
            ProposedPerformanceMeasures.TotalCost = calToatalCost(ProposedPerformanceMeasures);
        }

        

        private decimal calRepaireCost(int downtime)
        {
            return downtime * RepairPersonCost / 60;
        }

        
        private int calProposedDelayCost()
        {
            int n = 0;
            for (int i = 0; i < ProposedSimulationTable.Count; i++)
            {
                n += ProposedSimulationTable[i].Delay;
            }
            return n* DowntimeCost;
        }
        private int calCurrentDelayCost()
        {
            int n = 0;
            for (int i = 0; i < CurrentSimulationTable.Count; i++)
            {
                n += CurrentSimulationTable[i].Delay;
            }
            return n* DowntimeCost;
        }

        private decimal calToatalCost(PerformanceMeasures performanceMeasures)
        {
            return performanceMeasures.BearingCost + performanceMeasures.DelayCost + performanceMeasures.DowntimeCost + performanceMeasures.RepairPersonCost;
        }

    }


}

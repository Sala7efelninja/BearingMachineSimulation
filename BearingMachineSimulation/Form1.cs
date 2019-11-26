using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BearingMachineTesting;
using BearingMachineModels;
namespace BearingMachineSimulation
{
    public partial class Form1 : Form
    {
        SimSys system;
        public Form1()
        {
            InitializeComponent();
            List<string> tests = new List<string>() { Constants.FileNames.TestCase1, Constants.FileNames.TestCase2, Constants.FileNames.TestCase3 };
            comboBox1.DataSource = tests;



            //results = TestingManager.Test(system, Constants.FileNames.TestCase3);
            //MessageBox.Show(results);
        }

        DataGridView currentGridView(List<CurrentSimulationCase> table, int noOfBearing)
        {
            DataGridView grid = new DataGridView();
            grid.Columns.Add("index", "Index");
            for (int i = 0; i < noOfBearing; i++)
            {
                grid.Columns.Add("RH" + i, "RD");
                grid.Columns.Add("H" + i, "life Hours");
                grid.Columns.Add("AH" + i, "Accumlated life Hours");
                grid.Columns.Add("RD" + i, "RD");
                grid.Columns.Add("D" + i, "Delay");

            }
            grid.Rows.Add(table.Count);
            for (int j = 0; j < noOfBearing; j++)
            {
                int k = 0;
                for (int i = 0; i < table.Count; i++)//add rows
                {
                    if (table[i].Bearing.Index == j + 1 && grid.Rows.Count > k)
                    {


                        grid.Rows[k].Cells["index"].Value = k + 1;
                        grid.Rows[k].Cells["RH" + j].Value = table[i].Bearing.RandomHours;
                        grid.Rows[k].Cells["H" + j].Value = table[i].Bearing.Hours;
                        grid.Rows[k].Cells["AH" + j].Value = table[i].AccumulatedHours;
                        grid.Rows[k].Cells["RD" + j].Value = table[i].RandomDelay;
                        grid.Rows[k].Cells["D" + j].Value = table[i].Delay;
                        k++;
                    }
                }

            }
            return grid;

        }
        DataGridView ProposedGridView(List<ProposedSimulationCase> table, int noOfBearing)
        {
            DataGridView grid = new DataGridView();
            grid.Columns.Add("index", "Index");
            for (int i = 0; i < noOfBearing; i++)
            {
                grid.Columns.Add("RH" + i, "RD");
                grid.Columns.Add("H" + i, "life Hours");
            }
            grid.Columns.Add("F", "First Faliure");
            grid.Columns.Add("AH", "Accumlated life Hours");
            grid.Columns.Add("RD", "RD");
            grid.Columns.Add("D", "Delay");
            grid.Rows.Add(table.Count);
            for (int i = 0; i < table.Count; i++)//add rows
            {
                grid.Rows[i].Cells["index"].Value = i + 1;
                for (int j = 0; j < noOfBearing; j++)
                {
                    grid.Rows[i].Cells["RH" + j].Value = table[i].Bearings[j].RandomHours;
                    grid.Rows[i].Cells["H" + j].Value = table[i].Bearings[j].Hours;
                }
                grid.Rows[i].Cells["F"].Value = table[i].FirstFailure;
                grid.Rows[i].Cells["AH"].Value = table[i].AccumulatedHours;
                grid.Rows[i].Cells["RD"].Value = table[i].RandomDelay;
                grid.Rows[i].Cells["D"].Value = table[i].Delay;

            }

            return grid;

        }

        DataGridView performanceMeasures(PerformanceMeasures current, PerformanceMeasures proposed)
        {
            DataGridView grid = new DataGridView();
            grid.Columns.Add("", "");
            grid.Columns.Add("BC", "BearingCost");
            grid.Columns.Add("DelC", "DelayCost");
            grid.Columns.Add("DowC", "DowntimeCost");
            grid.Columns.Add("RC", "RepairPersonCost");
            grid.Columns.Add("TC", "TotalCost");
            grid.Rows.Add(2);
            grid.Rows[0].Cells[""].Value = "CurrentMethhod";
            grid.Rows[0].Cells["BC"].Value = current.BearingCost;
            grid.Rows[0].Cells["DelC"].Value = current.DelayCost;
            grid.Rows[0].Cells["DowC"].Value = current.DowntimeCost;
            grid.Rows[0].Cells["RC"].Value = current.RepairPersonCost;
            grid.Rows[0].Cells["TC"].Value = current.TotalCost;
            grid.Rows[1].Cells[""].Value = "ProposedMethod";
            grid.Rows[1].Cells["BC"].Value = proposed.BearingCost;
            grid.Rows[1].Cells["DelC"].Value = proposed.DelayCost;
            grid.Rows[1].Cells["DowC"].Value = proposed.DowntimeCost;
            grid.Rows[1].Cells["RC"].Value = proposed.RepairPersonCost;
            grid.Rows[1].Cells["TC"].Value = proposed.TotalCost;

            return grid;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            system = new SimSys();
            string file = comboBox1.SelectedItem.ToString();
            system.startSimulation(file);
            string results = TestingManager.Test(system, file);
            MessageBox.Show(results);
            if (tabPage1.Controls.Count > 0)
                tabPage1.Controls.RemoveAt(0);
            tabPage1.Controls.Add(currentGridView(system.CurrentSimulationTable, system.NumberOfBearings));
            tabPage1.Controls[0].Width = tabPage1.Width;
            tabPage1.Controls[0].Height = tabPage1.Height;

            if (tabPage2.Controls.Count > 0)
                tabPage2.Controls.RemoveAt(0);
            tabPage2.Controls.Add(ProposedGridView(system.ProposedSimulationTable, system.NumberOfBearings));
            tabPage2.Controls[0].Width = tabPage2.Width;
            tabPage2.Controls[0].Height = tabPage2.Height;

            if (tabPage3.Controls.Count > 0)
                tabPage3.Controls.RemoveAt(0);
            tabPage3.Controls.Add(performanceMeasures(system.CurrentPerformanceMeasures, system.ProposedPerformanceMeasures));
            tabPage3.Controls[0].Width = tabPage3.Width;
            tabPage3.Controls[0].Height = tabPage3.Height;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

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
            system = new SimSys();
            system.startSimulation(Constants.FileNames.TestCase2);
            string results =TestingManager.Test(system, Constants.FileNames.TestCase2);
            MessageBox.Show(results);
            dataGridView1.DataSource = system.CurrentSimulationTable;
            dataGridView1.DataSource = system.CurrentSimulationTable;

            //results = TestingManager.Test(system, Constants.FileNames.TestCase3);
            //MessageBox.Show(results);


        }
    }
}

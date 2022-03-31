using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicrodropDesktop
{
    public partial class Form1 : Form
    {
        private PortController controller;
        public Form1()
        {
            InitializeComponent();
            controller = new PortController("COM21");
            controller.SetLogMessage += AddLogMessage;
            controller.OpenPort();
            controller.GetFrequency(0);
            cbPulseNo.SelectedIndex=0;
        }

        private void AddLogMessage(string message)
        {
            BeginInvoke((Action)(() =>
            {
                lbLog.Items.Insert(0, message);
            }));

        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            controller?.SendRequest();
        }

        private void btnFrequency_Click(object sender, EventArgs e)
        {
            controller?.GetFrequency(2);
        }

        private void btnStrobe_Click(object sender, EventArgs e)
        {
            controller?.GetStrobe(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            controller?.SetFrequency(2, 362);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            controller?.GetPulseLength(1,3);
        }

        private (int, int) GetPulseData()
        {
            var channel = int.Parse((string)cbPulseNo.SelectedItem);
            var value = int.Parse(tbPulseValue.Text);
            return (channel, value);
        }

        private void btSetImpulseLength_Click(object sender, EventArgs e)
        {
            var (channel, value) = GetPulseData();
            controller?.SetPulseLength(1, channel, value);
        }

        private void btnSetImpulseVoltage_Click(object sender, EventArgs e)
        {
            var (channel, value) = GetPulseData();
            controller?.SetPulseVoltage(1, channel, value);
        }

        private void btnSetPulseDelay_Click(object sender, EventArgs e)
        {
            var (channel, value) = GetPulseData();
            controller?.SetPulseDelay(1, channel, value);
        }
    }
}

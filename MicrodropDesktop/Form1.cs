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
        private readonly PortController _controller;
        public Form1()
        {
            InitializeComponent();
            _controller = new PortController("COM16");
            _controller.SetLogMessage += AddLogMessage;
            _controller.OpenPort();
            _controller.GetFrequency(0);
            cbPulseNo.SelectedIndex = 0;
            cbHead.SelectedItem = 0;
        }

        private void AddLogMessage(string message)
        {
            BeginInvoke((Action)(() =>
            {
                lbLog.Items.Insert(0, message);
            }));

        }

        private void BtnRequest_Click(object sender, EventArgs e)
        {
            _controller?.SendRequest();
        }

        private void BtnFrequency_Click(object sender, EventArgs e)
        {
            _controller?.GetFrequency(2);
        }

        private void BtnStrobe_Click(object sender, EventArgs e)
        {
            _controller?.GetStrobe(1);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _controller?.SetFrequency(2, 362);
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            _controller?.GetPulseLength(1, 3);
        }

        private (int, int) GetPulseData()
        {
            var channel = int.Parse((string)cbPulseNo.SelectedItem);
            var value = int.Parse(tbPulseValue.Text);
            return (channel, value);
        }

        private int GetHead()
        {
            return cbHead.SelectedIndex + 1;
        }

        private void BtSetImpulseLength_Click(object sender, EventArgs e)
        {
            var (channel, value) = GetPulseData();
            _controller?.SetPulseLength(1, channel, value);
        }

        private void BtnSetImpulseVoltage_Click(object sender, EventArgs e)
        {
            var (channel, value) = GetPulseData();
            _controller?.SetPulseVoltage(1, channel, value);
        }

        private void BtnSetPulseDelay_Click(object sender, EventArgs e)
        {
            var (channel, value) = GetPulseData();
            _controller?.SetPulseDelay(1, channel, value);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            var head = GetHead();
            _controller?.SetActive(head, 1);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            var head = GetHead();
            _controller?.SetActive(head, 0);
        }
    }
}

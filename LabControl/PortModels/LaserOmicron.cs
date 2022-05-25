using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace LabControl.PortModels
{
    internal class LaserOmicron : LaserSerial
    {
        private readonly List<LCommand> _command;

        public LaserOmicron(string name) : base(name)
        {
            _command = new List<LCommand>
            {
                new LCommand(1, "?GFw\r", "Get Firmware", "GFw"),
                new LCommand(2, "?GFw|\r", "Get Firmware with splitter |", "GFw|"),
                new LCommand(3, "?GFH\r", "Get Firmware Head", "GFH"),
                new LCommand(4, "?GSN\r", "Get Serial Number", "GSN"),
                new LCommand(5, "?GSH\r", "Get Serial Number Head", "GSH"),
                new LCommand(6, "?GSI\r", "Get Spec Info", "GSI"),
                new LCommand(7, "?GMP\r", "Get Maximum Power", "GMP"),
                new LCommand(8, "?GWH\r", "Get Working Hours", "GWH"),
                new LCommand(9, "?MDP\r", "Measure Diode Power", "MDP"),
                new LCommand(10, "?MTD\r", "Measure Temperature diode", "MTD"),
                new LCommand(11, "?MTA\r", "Measure Temperature ambient", "MTA"),
                new LCommand(12, "?MTB\r", "Measure Temperature board", "MTB"),
                new LCommand(13, "?GAS\r", "Get Actual Status", "GAS"),
                new LCommand(14, "?GFB\r", "Get Falure Byte", "GFB"),
                new LCommand(15, "?GLF\r", "Get Latched Failurе", "GLF"),
                new LCommand(16, "?GLP\r", "Get Level Power value", "GLP"),
                new LCommand(17, "?SLP0FA\r", "Set Level Power", "SLP"),
                new LCommand(18, "?GPP\r", "Get Power setpoint as percentage", "GPP"),
                new LCommand(19, "?SPP050\r", "Set Power as Percentage", "SPP"),
                new LCommand(20, "?TPP070\r", "Temporary Power as Percentage", "TPP"),
                new LCommand(21, "?GOM\r", "Get operating mode", "GOM"),
                new LCommand(22, "?SOM0F\r", "Set operating mode", "SOM"),
                new LCommand(23, "?SAP1\r", "Set auto power-up", "SAP"),
                new LCommand(24, "?SAS1\r", "Set Auto Start", "SAS"),
                new LCommand(30, "?TPP\r", "Get temp Power", "GPP"),
                new LCommand(35, "?POn\r", "Power On", "POn"),
                new LCommand(36, "?POf\r", "Power Off", "POf"),
                new LCommand(37, "?LOn\r", "Laser On", "LOn"),
                new LCommand(38, "?LOf\r", "Laser Off", "LOf"),
            };

        }

        public override void Init()
        {
            try
            {
                if (Name.Length == 0)
                {
                    Name = "COM6";
                }

                Port = new SerialPort(Name)
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None,
                    RtsEnable = true
                };

                Port.DataReceived += DataReceivedHandler;

                Port.Open();
                SendCommand(1);
                SendCommand(7);
                SendCommand(18);
                SendCommand(16);
                SendCommand(21);
                SendCommand(13);
                SendCommand(30);
                SendCommand(23);
            }
            catch (Exception ex)
            {
                OnSetLogMessage(ex.Message);
            }
        }

        private void SendCommand(int cmd)
        {
            if (Port.IsOpen)
                Port.Write(GetCmdById(cmd).SCommand);
        }

        public LCommand GetCmdById(int id)
        {
            return _command.FirstOrDefault(x => x.Id == id);
        }

        public override void On()
        {
            SendCommand(35);
            SendCommand(37);
        }

        public override void SetPowerLevel(int lvl)
        {
            var pwr = lvl * 4095 / 800;
            var cmd = new LCommand(40, $"?SLP{pwr:X3}\r", "Set Level Power", "SLP");

            if (Port.IsOpen)
            {
                Port.Write(cmd.SCommand);
            }
        }

        public override void PowerLevelReduce(int pwr, int time)
        {
            throw new NotImplementedException();
        }

        public override void Off()
        {
            SendCommand(36);
            SendCommand(38);
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                var mXdata = new byte[cnt + 1];
                try
                {
                    sp.Read(mXdata, 0, cnt);
                }
                catch (Exception ex)
                {
                    OnSetLogMessage(ex.Message);
                }
                var ascii = Encoding.ASCII;
                var answers = ascii.GetString(TrimReceivedData(mXdata)).Split('\r');
            }
            catch (Exception ex)
            {
                OnSetLogMessage(ex.Message);
            }
        }

        public class LCommand
        {
            public readonly int Id;
            public string Description { get; }
            public readonly string SCommand;
            public readonly string SAnswer;

            public LCommand(int i, string cmd, string descr, string ans)
            {
                Id = i;
                Description = descr;
                SCommand = cmd;
                SAnswer = ans;
            }
        }
    }
}

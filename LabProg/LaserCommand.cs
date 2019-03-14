using System.Collections.Generic;
using System.Linq;

namespace LabProg
{
    internal class LaserCommand
    {
        public List<LCommand> CmdList = new List<LCommand>();

        public LaserCommand()
        {
            CmdList.Add(new LCommand(1, "?GFw\r", "Get Firmware", "GFw"));
            CmdList.Add(new LCommand(2, "?GFw|\r", "Get Firmware with splitter |", "GFw|"));
            CmdList.Add(new LCommand(3, "?GFH\r", "Get Firmware Head", "GFH"));
            CmdList.Add(new LCommand(4, "?GSN\r", "Get Serial Number", "GSN"));
            CmdList.Add(new LCommand(5, "?GSH\r", "Get Serial Number Head", "GSH"));
            CmdList.Add(new LCommand(6, "?GSI\r", "Get Spec Info", "GSI"));
            CmdList.Add(new LCommand(7, "?GMP\r", "Get Maximum Power", "GMP"));
            CmdList.Add(new LCommand(8, "?GWH\r", "Get Working Hours", "GWH"));
            CmdList.Add(new LCommand(9, "?MDP\r", "Measure Diode Power", "MDP"));
            CmdList.Add(new LCommand(10, "?MTD\r", "Measure Temperature diode", "MTD"));
            CmdList.Add(new LCommand(11, "?MTA\r", "Measure Temperature ambient", "MTA"));
            CmdList.Add(new LCommand(12, "?MTB\r", "Measure Temperature board", "MTB"));
            CmdList.Add(new LCommand(13, "?GAS\r", "Get Actual Status", "GAS"));
            CmdList.Add(new LCommand(14, "?GFB\r", "Get Falure Byte", "GFB"));
            CmdList.Add(new LCommand(15, "?GLF\r", "Get Latched Failurе", "GLF"));
            CmdList.Add(new LCommand(16, "?GLP\r", "Get Level Power value", "GLP"));
            CmdList.Add(new LCommand(17, "?SLP0FA\r", "Set Level Power", "SLP"));
            CmdList.Add(new LCommand(18, "?GPP\r", "Get Power setpoint as percentage", "GPP"));
            CmdList.Add(new LCommand(19, "?SPP050\r", "Set Power as Percentage", "SPP"));
            CmdList.Add(new LCommand(20, "?TPP070\r", "Temporary Power as Percentage", "TPP"));
            CmdList.Add(new LCommand(21, "?GOM\r", "Get operating mode", "GOM"));
            CmdList.Add(new LCommand(22, "?SOM0F\r", "Set operating mode", "SOM"));
            CmdList.Add(new LCommand(23, "?SAP1\r", "Set auto power-up", "SAP"));
            CmdList.Add(new LCommand(24, "?SAS1\r", "Set Auto Start", "SAS"));
        }

        public LCommand GetCmdById(int id)
        {
            return CmdList.Where(x => x.Id == id).FirstOrDefault();
        }
    }
    public class LCommand
    {
        public readonly int Id;
        public readonly string Description;
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

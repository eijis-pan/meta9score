using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meta9score
{
    public class BilliardsModuleEventLoggerEventArgs : EventArgs
    {
        public string text;
        public int? intValue;
        public int? intValue2;
        public string[]? players;
        public PoolState? poolState;
        public string? player;
        public bool[]? ballProcketedFlags;

        public BilliardsModuleEventLoggerEventArgs(string text)
        {
            this.text = text;
        }

        public BilliardsModuleEventLoggerEventArgs(string text, int? intValue)
        {
            this.text = text;
            this.intValue = intValue;
        }

        public BilliardsModuleEventLoggerEventArgs(string text, int? intValue, int? intValue2)
        {
            this.text = text;
            this.intValue = intValue;
            this.intValue2 = intValue2;
        }

        public BilliardsModuleEventLoggerEventArgs(string text, PoolState poolState)
        {
            this.text = text;
            this.poolState = poolState;
        }

        public BilliardsModuleEventLoggerEventArgs(string text, string[] players)
        {
            this.text = text;
            this.players = players;
        }

        public BilliardsModuleEventLoggerEventArgs(string text, string player)
        {
            this.text = text;
            this.player = player;
        }

        public BilliardsModuleEventLoggerEventArgs(string text, bool[] ballProcketedFlags)
        {
            this.text = text;
            this.ballProcketedFlags = ballProcketedFlags;
        }
    }
}

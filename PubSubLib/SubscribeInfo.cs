using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubLib
{
    class SubscribeInfo
    {
        IStockCallback chanelToClient;
        public IStockCallback ChanelToClient
        {
            get { return chanelToClient; }
            set { chanelToClient = value; }
        }
        string symbol;
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        double triggerPrice;
        public double RriggerPrice
        {
            get { return triggerPrice; }
            set { triggerPrice = value; }
        }
    }
}

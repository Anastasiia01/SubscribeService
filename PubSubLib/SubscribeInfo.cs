using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace PubSubLib
{
    class SubscribeInfo
    {
        IStockCallback channelToClient;
        public IStockCallback ChannelToClient
        {
            get { return channelToClient; }
            set { channelToClient = value; }
        }
        Dictionary<string, double> symToPrice;

        public SubscribeInfo()
        {
            symToPrice = new Dictionary<string, double>();
        }

        public void AddSymbol(string symbol, double price)
        {
            if (!symToPrice.ContainsKey(symbol))
            {
                symToPrice.Add(symbol, price);
            }
            else
            {
                symToPrice[symbol] = price;
            }
        }

        public void RemoveSymbol(string symbol)
        {
            if (symToPrice.ContainsKey(symbol))
            {
                symToPrice.Remove(symbol);
            }
        }

        public double getStockTriggerPrice(string symbol)
        {
            if (symToPrice.ContainsKey(symbol))
            {
                return symToPrice[symbol];
            }
            else
            {
                return -1;
            }
        }

        public int GetNumOfSymbolsSubscribed()
        {
            return symToPrice.Count;
        }


        /*string symbol;
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        double triggerPrice;
        public double TriggerPrice
        {
            get { return triggerPrice; }
            set { triggerPrice = value; }
        }*/
    }
}

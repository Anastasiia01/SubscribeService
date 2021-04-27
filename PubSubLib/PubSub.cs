using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PubSubLib
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PubSub : ILongCompute, IStocks, IPriceChange
    {
        static List<SubscribeInfo> SubscriberList = new List<SubscribeInfo>();
        ComputeResult cr = new ComputeResult();
        Object olock = new object();

        #region ILongCompute Members
        public void Compute(int a, int b, string clientId)
        {
            Thread.Sleep(5000);
            lock (olock)
            {
                cr.Result = 45.667 + a + b;
                cr.ResultTime = DateTime.Now;
                cr.ClientID = clientId;
                // trigger callback in client
                IComputeCallback callbackChannel =
                OperationContext.Current.GetCallbackChannel<IComputeCallback>();
                if (((ICommunicationObject)callbackChannel).State == CommunicationState.Opened)
                    callbackChannel.OnComputeResult(cr);
            }
            Thread.Sleep(5000);
            lock (olock)
            {
                cr.Result = cr.Result + a + b;
                cr.ResultTime = DateTime.Now;
                cr.ClientID = clientId;
                //trigger callback in client
                IComputeCallback callbackChannel =
                OperationContext.Current.GetCallbackChannel<IComputeCallback>();
                if (((ICommunicationObject)callbackChannel).State == CommunicationState.Opened)
                    callbackChannel.OnComputeResult(cr);
            }
        }
        #endregion


        #region IStocks Members
        public bool SubscribeToStockPrice(string stocksym, double triggerPrice)
        {
            try
            {
                IStockCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IStockCallback>();
                bool ifNewSubscriber = true;
                foreach (SubscribeInfo sub in SubscriberList)
                {
                    if(sub.ChannelToClient == callbackChannel) //found subscriber
                    {
                        ifNewSubscriber = false;
                        sub.AddSymbol(stocksym, triggerPrice);
                    }
                }
                if (ifNewSubscriber)
                {
                    SubscribeInfo subscriber = new SubscribeInfo();                  
                    SubscriberList.Add(subscriber);
                    subscriber.ChannelToClient = callbackChannel;
                    subscriber.AddSymbol(stocksym, triggerPrice);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("Subscription Error"));
            }
            return true;
        }

        public bool UnSubscribeToStockPrice(string stocksym)
        {
            try
            {
                IStockCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IStockCallback>();
                SubscribeInfo subToRemove = null;
                foreach (SubscribeInfo sub in SubscriberList)
                {
                    if (sub.ChannelToClient == callbackChannel) //found subscriber
                    {
                        sub.RemoveSymbol(stocksym);
                        if (sub.GetNumOfSymbolsSubscribed() == 0)
                        {
                            subToRemove = sub;                            
                        }
                    }
                }
                if (subToRemove != null)
                    SubscriberList.Remove(subToRemove);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("UnSubscription Error"));
            }
            return true;
        }
        #endregion

        #region IPriceChange Members
        public bool ChangeStockPrice(string symbol, double newprice)
        {
            try
            {
                StockInfo si = new StockInfo();
                si.Price = newprice;
                si.Symbol = symbol;
                si.STime = DateTime.Now;
                // trigger call to the subscribers
                foreach (SubscribeInfo sub in SubscriberList)
                {
                    double triggerPrice = sub.getStockTriggerPrice(symbol);
                    if (triggerPrice != -1 && triggerPrice <= newprice)
                    {
                        if (((ICommunicationObject)sub.ChannelToClient).State == CommunicationState.Opened)
                            sub.ChannelToClient.OnPriceChange(si);
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("Change Price Error"));
            }
            return true;
        }
        #endregion
    }
}

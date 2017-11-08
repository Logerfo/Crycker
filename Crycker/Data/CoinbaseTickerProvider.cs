﻿using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Crycker.Helper;

namespace Crycker.Data
{
    class CoinbaseTickerProvider : BaseTickerProvider, ITickerProvider
    {
        public CoinbaseTickerProvider(string coin, string currency)
        {
            supportedCurrencies = new string[] { "EUR", "USD" };
            supportedCoins = new string[] { "BTC", "LTH", "ETH" };

            Coin = coin;
            Currency = currency;
        }

        public string Provider
        {
            get { return "Coinbase"; }
        }

        protected string BaseUrl
        {
            get { return $"https://api.coinbase.com/v2/prices/{_coin}-{_currency}/spot"; }
        }

        public async Task UpdateData()
        {
            Logger.Info("Getting data from Coinbase.");

            try
            {
                var result = await CallRestApi(BaseUrl);
                var tickerData = ParseJsonResult<CoinbaseTickerData>(result);

                LastUpdated = DateTime.Now;
                LastPrice = tickerData.data.amount;

                Logger.Info($"Coinbase said {this.Coin} = {tickerData.data.amount} {tickerData.data.currency} @ {LastUpdated}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating data.", ex);
            }
        }
    }

    [DataContract]
    public class Data
    {
        [DataMember]
        public decimal amount { get; set; }
        [DataMember]
        public string currency { get; set; }
    }

    [DataContract]
    public class CoinbaseTickerData
    {
        [DataMember]
        public Data data { get; set; }
    }
}

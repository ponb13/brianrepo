using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace StockProvider
{
    public class RandomQuoteSource : IQuoteSource
    {
        DispatcherTimer timer;
        private const int TIMER_INTERVAL = 100;

        private int ind = 0;
        private readonly List<string> _subscribedSymbols = new List<string>();
        private readonly Dictionary<string, double[]> _symbolToPriceEnumerator = new Dictionary<string, double[]>();

        private readonly Object _enumeratorLock = new Object();
        private readonly Object _symbolsLock = new Object();
        private Random rnd = new Random();


        public RandomQuoteSource()
        {
            timer = new DispatcherTimer(DispatcherPriority.DataBind);
            timer.Interval = TimeSpan.FromMilliseconds(TIMER_INTERVAL);
            timer.Tick += delegate
                   {
                       lock (_symbolsLock)
                       {
                           double[] vals;
                           int cnt = _subscribedSymbols.Count;
                           if (cnt == 0)
                               return;
                           int current = ind++ % cnt;
                           {
                               string symbol = _subscribedSymbols[current];
                               if (_symbolToPriceEnumerator.TryGetValue(symbol, out vals))
                               {
                                   double change = rnd.NextDouble() - 0.5;
                                   vals[0] = vals[1]; vals[1] = vals[1] + change;

                                   Quote quote = new Quote();
                                   quote.Symbol = symbol;
                                   quote.Price = vals[1];
                                   quote.Volume = rnd.Next(1, 100);
                                   quote.Tick = (vals[1] > vals[0]) ? Tick.Up : Tick.Down;
                                   QuoteArrived(quote);
                               }
                           }
                       }
                   };
            timer.Start();
        }

        #region IQuoteSource Members

        public event Action<Quote> QuoteArrived;

        public void Subscribe(string symbol)
        {
            lock (_symbolsLock)
            {
                double[] values;
                if (!_symbolToPriceEnumerator.TryGetValue(symbol, out values))
                {
                    double[] vals = new double[2];
                    _symbolToPriceEnumerator.Add(symbol, vals);
                    _subscribedSymbols.Add(symbol);
                }
            }

        }

        #endregion
    }
}

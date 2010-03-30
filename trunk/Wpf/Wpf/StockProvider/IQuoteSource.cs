using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockProvider
{
    public interface IQuoteSource
    {
        void Subscribe(string symbol);

        event Action<Quote> QuoteArrived;
    }
}

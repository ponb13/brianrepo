using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using StockProvider;

namespace Client
{
    public class WatchListViewModel
    {
        ObservableCollection<Quote> quotes = new ObservableCollection<Quote>();
        IQuoteSource source = null;

        public string Symbol
        {
            get;
            set;
        }

        public WatchListViewModel(IQuoteSource source)
        {
            this.source = source;
            this.source.QuoteArrived +=new Action<Quote>(source_QuoteArrived);
        }

        private void source_QuoteArrived(Quote quote)
        {
            Action dispatchAction = () => this.quotes.Add(quote);
            this.Dispatcher.BeginInvoke(dispatchAction);
        }
        
        private void Subscribe()
        {
            //string symbol = this._symbolText.Text;
            quoteSource.Subscribe(this.Symbol);

            //this._lastSymbolText.Text = symbol;
        }
    }
}

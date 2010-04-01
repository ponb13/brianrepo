using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using StockProvider;

namespace Client
{
    public class WatchListViewModel : DependencyObject
    {
        ObservableCollection<Quote> quotes = new ObservableCollection<Quote>();
        private IQuoteSource source;
        private Dispatcher currentDispatcher;

        public string Symbol
        {
            get;
            set;
        }

        public string LastSymbol
        {
            get { return (string)GetValue(LastSymbolProperty); }
            set { SetValue(LastSymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastSymbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastSymbolProperty =
            DependencyProperty.Register("LastSymbol", typeof(string), typeof(WatchListViewModel), new UIPropertyMetadata(0));


        public WatchListViewModel(IQuoteSource source)
        {
            this.currentDispatcher = Dispatcher.CurrentDispatcher;
            this.source = source;
            this.source.QuoteArrived += new Action<Quote>(source_QuoteArrived);
        }

        private void source_QuoteArrived(Quote quote)
        {
            Action dispatchAction = () => this.quotes.Add(quote);
            this.currentDispatcher.BeginInvoke(dispatchAction);
        }
        
        private void Subscribe()
        {
            source.Subscribe(this.Symbol);
            this.LastSymbol = this.Symbol;
        }
    }
}

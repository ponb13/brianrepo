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
        private IQuoteSource source;
        private Dispatcher currentDispatcher;

        public ObservableCollection<Quote> Quotes
        {
            get;
            set;
        }

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
            DependencyProperty.Register("LastSymbol", typeof(string), typeof(WatchListViewModel),new UIPropertyMetadata(""));


        public WatchListViewModel(IQuoteSource source)
        {
            this.Quotes = new ObservableCollection<Quote>();
            this.currentDispatcher = Dispatcher.CurrentDispatcher;
            this.source = source;
            this.source.QuoteArrived += new Action<Quote>(source_QuoteArrived);
        }

        public void Subscribe()
        {
            this.LastSymbol = this.Symbol;            
            source.Subscribe(this.Symbol);
        }

        private void source_QuoteArrived(Quote quote)
        {
            Action dispatchAction = () => this.Quotes.Add(quote);
            this.currentDispatcher.BeginInvoke(dispatchAction);
        }
    }
}

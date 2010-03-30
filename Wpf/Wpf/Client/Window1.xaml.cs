using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StockProvider;
using System.Collections.ObjectModel;

namespace Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        RandomQuoteSource quoteSource = new RandomQuoteSource();
        ObservableCollection<Quote> observableCollection = new ObservableCollection<Quote>();
        
        
        public Window1()
        {
            InitializeComponent();

            quoteSource.QuoteArrived += new Action<Quote>(quoteSource_QuoteArrived);
        }

        private void quoteSource_QuoteArrived(Quote quote)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string symbol = this._symbolText.Text;
            quoteSource.Subscribe(symbol);

            this._lastSymbolText.Text = symbol;
        }
    }
}

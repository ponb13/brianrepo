using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockProvider
{
    public enum Tick
    {
        Up,
        Down,
        NoChange
    }

    public class Quote
    {
        string symbol;
        double price;
        int volume;
        DateTime date;
        Tick tick;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public int Volume
        {
            get { return volume; }
            set { volume = value; }
        }
        public Tick Tick
        {
            get { return tick; }
            set { tick = value; }
        }

    }
}

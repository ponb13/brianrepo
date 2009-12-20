using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class VendingMachine
    {
        private State state;

        public VendingMachine()
        {
            Console.WriteLine("The Vending Machine is now online: product costs 25c");
            state = Start.Instance();
        }

        public void ChangeState(State to)
        {
            state = to;
        }

        public void Vend()
        {
            // deliver merchandise
            Console.WriteLine("Dispensing product...Thank you!");
        }

        public void AddNickel()
        {
            state.AddNickel(this);
        }

        public void AddDime()
        {
            state.AddDime(this);
        }

        public void AddQuarter()
        {
            state.AddQuarter(this);
        }
    }

}

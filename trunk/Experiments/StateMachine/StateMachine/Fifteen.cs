using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class Fifteen : State
    {
        private static State state = new Fifteen();

        private Fifteen()
        {
        }

        public static State Instance()
        {
            Console.WriteLine("Credit: 15c");
            return state;
        }

        public override void AddNickel(VendingMachine vm)
        {
            this.ChangeState(vm,Twenty.Instance());
        }

        public override void AddDime(VendingMachine vm)
        {
            vm.Vend();
            vm.ChangeState(Start.Instance());
        }

        public override void AddQuarter(VendingMachine vm)
        {
            vm.Vend();
            vm.ChangeState(Start.Instance());
        }

        
    }
}

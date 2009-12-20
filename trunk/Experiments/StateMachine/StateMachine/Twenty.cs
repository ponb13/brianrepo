using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class Twenty : State
    {
        private static State state = new Twenty();

        private Twenty()
        {

        }

        public static State Instance()
        {
            Console.WriteLine("Credit: 20c");
            return state;
        }

        public override void AddDime(VendingMachine vm)
        {
            vm.Vend();
            vm.ChangeState(Start.Instance());
        }

        public override void AddNickel(VendingMachine vm)
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

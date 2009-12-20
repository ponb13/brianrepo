using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class Ten : State
    {
        private static State state = new Ten();

        private Ten()
        {

        }

        public static State Instance()
        {
            Console.WriteLine("Credit: 10c");
            return state;
        }

        public override void AddNickel(VendingMachine vm)
        {
            this.ChangeState(vm, Fifteen.Instance());
        }

        public override void AddDime(VendingMachine vm)
        {
            this.ChangeState(vm, Twenty.Instance());
        }

        public override void AddQuarter(VendingMachine vm)
        {
            vm.Vend();
            this.ChangeState(vm, Start.Instance());
        }
    }
}

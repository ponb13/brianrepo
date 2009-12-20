using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class Five : State
    {
        private static State state = new Five();
        private Five()
        {
        }

        public static State Instance()
        {
            // singleton logic
            Console.WriteLine("Credit: 5c");
            return state;
        }

        public override void AddNickel(VendingMachine vm)
        {
            ChangeState(vm, Ten.Instance());
        }

        public override void AddDime(VendingMachine vm)
        {
            ChangeState(vm, Fifteen.Instance());
        }

        public override void AddQuarter(VendingMachine vm)
        {
            vm.Vend();
            ChangeState(vm, Start.Instance());
            // no change returned :-)
        }
    }

}

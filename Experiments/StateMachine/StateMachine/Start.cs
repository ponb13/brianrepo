using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class Start : State
    {
        private static State state = new Start();
        private Start()
        {
        }

        public static State Instance()
        {
            // singleton logic
            Console.WriteLine("Credit: 0c");
            return state;
        }

        public override void AddNickel(VendingMachine vm)
        {
            ChangeState(vm, Five.Instance());
        }

        public override void AddDime(VendingMachine vm)
        {
            ChangeState(vm, Ten.Instance());
        }

        public override void AddQuarter(VendingMachine vm)
        {
            vm.Vend();    // start over
        }
    }

}

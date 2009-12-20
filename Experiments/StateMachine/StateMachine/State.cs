using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    abstract class State
    {
        public virtual void AddNickel(VendingMachine vm)
        { }
        public virtual void AddDime(VendingMachine vm)
        { }
        public virtual void AddQuarter(VendingMachine vm)
        { }

        protected virtual void ChangeState(VendingMachine vm, State s)
        {
            vm.ChangeState(s);
        }
    }
}

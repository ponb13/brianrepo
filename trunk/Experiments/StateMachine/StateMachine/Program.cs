using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            VendingMachine vm = new VendingMachine();
            vm.AddNickel();
            vm.AddDime();
            vm.AddNickel();
            vm.AddDime();
        }
    }
}

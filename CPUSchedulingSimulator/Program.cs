using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    class Program {
        static void Main(string[] args) {
        	SchedulingAlgorithm SA = new FirstComeFirstServe();
            SA.loadProcesses("processDataFile.txt");
        }
    }
}

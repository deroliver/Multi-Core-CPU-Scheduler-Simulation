using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    class Simulator {

        private List<Core> CPUS;
        private SchedulingAlgorithm scheduler;

        public Simulator(int numCores, SchedulingAlgorithm algorithm) {
            for (int i = 0; i < numCores; i++)
                CPUS.Add(new Core());

            scheduler = algorithm;
        }


        public void run() {

        }
    }
}

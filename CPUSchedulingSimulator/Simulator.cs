using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    class Simulator {

        private List<Core> CPUS;
        private SchedulingAlgorithm scheduler;

        /// <summary>
        /// Runs the simulation
        /// </summary>
        /// <param name="numCores">The number of cores to model</param>
        /// <param name="algorithm">Which algorithm to use</param>
        public Simulator(int numCores, SchedulingAlgorithm algorithm) {
            for (int i = 0; i < numCores; i++)
                CPUS.Add(new Core());

            scheduler = algorithm;
        }

        /// <summary>
        /// Run method
        /// </summary>
        public void run() {

        }
    }
}

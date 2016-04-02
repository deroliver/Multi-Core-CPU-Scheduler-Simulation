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
        public Simulator(int numCores) {
            CPUS = new List<Core>();
            for (int i = 0; i < numCores; i++)
                CPUS.Add(new Core());

            //scheduler = new MultiLevelFeedBack();
            scheduler = new RoundRobin(100);
        }

        /// <summary>
        /// Run method
        /// </summary>
        public void run() {
            scheduler.run(CPUS, "processDataFile.txt");
        }
    }
}

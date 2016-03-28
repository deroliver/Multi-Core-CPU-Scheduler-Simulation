using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    /// <summary>
    /// A class to model a CPU Core
    /// </summary>
    public class Core {

        /// <summary>
        /// Core ID static across all cores
        /// Incremented when a new core is added
        /// </summary>
        static protected int CPU_ID = -1;

        /// <summary>
        /// The actual Core ID
        /// </summary>
        public int CORE_ID;

        /// <summary>
        /// The process being processed in the core
        /// </summary>
        public Process process {
            get; set;
        }

        /// <summary>
        /// Constructor that sets the Core ID
        /// </summary>
        public Core() {
            CORE_ID = CPU_ID++;
        }

        /// <summary>
        /// Updates the process in the core
        /// </summary>
        public void update() {
            if (process != null) {
                process.bursts[process.currentBurst].step += 1;
            }
        }
    }
}

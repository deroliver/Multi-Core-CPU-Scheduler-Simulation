using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class Core {

        static protected int CPU_ID = -1;

        public Process process {
            get { return process; }
            set { process = value; }
        }

        public Core() {
            CPU_ID = CPU_ID++;
        }

        public void update() {
            if (process != null) {
                process.bursts[process.currentBurst].step += 1;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class Process {

        public Process(int processid, int numbursts, int arrivaltime, List<Burst> burst) {
            processID = processid;
            bursts = burst;
            arrivalTime = arrivaltime;
            numBursts = numbursts;
        }


        #region Non-Changeable Variables

        // Set in the Constructor
        // Cannot be set again
        public int processID {
            get; set;
        }

        public int arrivalTime {
            get; set;
        }

        public int numBursts {
            get; set;
        }

        #endregion


        #region Changeable Variables

        // Set in the Constructor
        // Can be set again
        public int startTime {
            get; set;
        }

        public int endTime {
            get; set;
        }

        public int waitingTime {
            get; set;
        }

        public int currentBurst {
            get; set;
        }
    
        private int priority {
            get; set;
        }
        // Changed to public to access inside my Round Robin
        public int quantumRemaining {
            get; set;
        }

        private int currentQueue {
            get; set;
        }
        

        #endregion

        public List<Burst> bursts;

        public class Burst {
            public Burst(int length, int step) {
                this.length = length;
                this.step = step;
            }
            public int length;
            public int step;
        }
        
    }
}

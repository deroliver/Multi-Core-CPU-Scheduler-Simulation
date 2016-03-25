using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class Process {

        #region Non-Changeable Variables

        // Set in the Constructor
        // Cannot be set again
        public int processID {
            get { return processID; }
        }

        public int arrivalTime {
            get { return arrivalTime; }
        }

        public int numBursts {
            get { return numBursts; }
        }

        #endregion


        #region Changeable Variables

        // Set in the Constructor
        // Can be set again
        public int startTime {
            get { return startTime; }
            set { startTime = value; }    
        }

        public int endTime {
            get { return endTime; }
            set { endTime = value; }
        }

        public int waitingTime {
            get { return waitingTime; }
            set { waitingTime = value; }
        }

        public int currentBurst {
            get { return currentBurst; }
            set { waitingTime = value; }
        }
    
        private int priority {
            get { return priority; }
            set { priority = value; }
        }
        // Changed to public to access inside my Round Robin
        public int quantumRemaining {
            get { return quantumRemaining; }
            set { quantumRemaining = value; }
        }

        private int currentQueue {
            get { return currentQueue; }
            set { currentQueue = value; }
        }
        

        #endregion

        public List<Burst> bursts;

        public class Burst {
            public int length;
            public int step;
        }
        
    }
}

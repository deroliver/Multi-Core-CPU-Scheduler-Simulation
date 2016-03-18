using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public abstract class SchedulingAlgorithm {
        //protected Process[] processes;
        protected List<Process> preReadyQueue;
        protected List<Process> processes;

        // CPUs
        protected List<Process> CPUS;

        public int ticks {
            get { return ticks; }
            set { ticks = value; }
        }

        public int numberOfProcesses {
            get { return numberOfProcesses; }
        }
        protected int nextProcess;

        // Queues for the ready and waiting processes
        protected Queue<Process> readyQueue {
            get { return readyQueue; }
        }
        protected Queue<Process> waitingQueue {
            get { return readyQueue; }
        }
        
        public int cpuUtilizationTicks {
            get { return cpuUtilizationTicks; }
            set { cpuUtilizationTicks = value; }
        }


        public SchedulingAlgorithm() {
            processes = new List<Process>();
            preReadyQueue = new List<Process>();
            CPUS = new List<Process>();
        }
      

        /// <summary>
        /// Gets the next process to be processed
        /// </summary>
        /// <returns></returns>
        public abstract Process getNextProcess();
        public abstract void addNewProcess();
        public abstract void moveFromWaitingToReady();
        public abstract void moveFromReadyToRunning();
        public abstract void moveFromRunningToWaiting();
        public abstract void updateProcessState();


        #region Process Functions

        /// <summary>
        /// The total number of incoming processes
        /// Processes that haven't arrived yet
        /// </summary>
        /// <returns>The incoming processes</returns>
        protected int numberOfIncProcesses() {
            return numberOfProcesses - nextProcess;
        }

        /// <summary>
        /// Compare the arrival time of two Processes
        /// </summary>
        /// <param name="P1">The first process</param>
        /// <param name="P2">The second process</param>
        /// <returns></returns>
        protected int arrivalTimeComparer(Process P1, Process P2) {
            return P1.arrivalTime - P2.arrivalTime;
        }

        /// <summary>
        /// Compare the ID of two Processes
        /// </summary>
        /// <param name="P1">The first process</param>
        /// <param name="P2">The second process</param>
        /// <returns></returns>
        protected int processIDComparer(Process P1, Process P2) {
            if (P1.processID == P2.processID) {
                throw new Exception("ERROR -- ***Multiples Processes with same ID*** --");
            }

            return P1.processID - P2.processID;
        }

        /// <summary>
        /// Checks all CPUs for running processes
        /// </summary>
        /// <returns>Total number of running processes</returns>
        public int numberOfCurrentRunningProcesses() {
            int totalRunning = 0;
            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i] != null)
                    totalRunning++;
            }
            return totalRunning;
        }

        public int totalExpectedProcesses() {
            return numberOfProcesses - nextProcess;
        }

        #endregion
    }
}

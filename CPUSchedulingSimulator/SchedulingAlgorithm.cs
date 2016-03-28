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

        protected int nextProcess;

        // CPUs
        protected List<Core> CPUS;
        //protected List<Process> CPUS;

        public int ticks {
            get { return ticks; }
            set { ticks = value; }
        }

        public int quantumtime
        {
            get { return quantumtime; }
            set { quantumtime = value; }
        }

        public int contextSwitch
        {
            get { return contextSwitch; }
            set { contextSwitch = value; }
        }

        //public int numberOfProcesses {
            //get { return numberOfProcesses; }
       // }
        

        // Queues for the ready and waiting processes
        protected Queue<Process> readyQueue {
            get { return readyQueue; }
        }

        /// <summary>
        /// Queue for processes that are waiting
        /// </summary>
        protected Queue<Process> waitingQueue {
            get { return readyQueue; }
        }
        
        /// <summary>
        /// Keeps track of CPU utilization ticks
        /// </summary>
        public int cpuUtilizationTicks {
            get { return cpuUtilizationTicks; }
            set { cpuUtilizationTicks = value; }
        }

        /// <summary>
        /// Simple constructor
        /// Initializes member variables
        /// </summary>
        public SchedulingAlgorithm() {
            processes = new List<Process>();
            preReadyQueue = new List<Process>();
            CPUS = new List<Core>();
        }

        #region Abstract Methods

        public abstract Process getNextProcess();
        public abstract void addNewProcess();
        public abstract void moveFromWaitingToReady();
        public abstract void moveFromReadyToRunning();
        public abstract void moveFromRunningToWaiting();
        public abstract void updateProcessState();
        public abstract void run(List<Core> CPUS, string filename);

        #endregion


        #region Process Functions

        /// <summary>
        /// The total number of incoming processes
        /// Processes that haven't arrived yet
        /// </summary>
        /// <returns>The incoming processes</returns>
        protected int numberOfIncProcesses() {
            return processes.Count - nextProcess;
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

        /// <summary>
        /// Calculates how many processes we can expect
        /// </summary>
        /// <returns></returns>
        public int totalExpectedProcesses() {
            return processes.Count - nextProcess;
        }

        /// <summary>
        /// Loads the processes from the created data file
        /// </summary>
        /// <param name="filename">The filename</param>
        public void loadProcesses(String filename) { 
            string line = "";
            Process newProcess;

            System.IO.StreamReader file = new System.IO.StreamReader(filename);

            // Read in each line in the process data files
            while((line = file.ReadLine()) != null) {
                string[] subs = line.Split(null);
                int processID = int.Parse(subs[0]);
                int arrivalTime = int.Parse(subs[1]);
                int numBursts = int.Parse(subs[2]);

                List<Process.Burst> bursts = new List<Process.Burst>();
                Process.Burst newBurst;

                // Create a Burst for every Burst in the line
                for (int i = 3; i < numBursts + 3; i++) {
                    newBurst = new Process.Burst(int.Parse(subs[i]), 0);
                    bursts.Add(newBurst);
                }

                // Create the new process using the parsed variables and the 
                // Burst list, then add the process to the Process list
                newProcess = new Process(processID, numBursts, arrivalTime, bursts);
                processes.Add(newProcess);
            }

            file.Close();

            for(int i = 0; i < processes.Count; i++) {
                Console.Write("PID:" + processes[i].processID + " NumBursts: " + processes[i].numBursts + " Arrival Time: " + processes[i].arrivalTime);
                for(int j = 0; j < processes[i].bursts.Count; j++) {
                    Console.Write(" " + processes[i].bursts[j].length);
                }
                Console.Write("\n");
            }
        }


        #endregion
    }
}

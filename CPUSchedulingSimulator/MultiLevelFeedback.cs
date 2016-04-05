using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class MultiLevelFeedBack : SchedulingAlgorithm {
        List<Process> p = new List<Process>();
        private Queue<Process>[] arrayOfReadyQueues;
        private const int NUMBER_OF_QUEUES = 3;
        private int[] quantums = new int[NUMBER_OF_QUEUES - 1];
        

        public MultiLevelFeedBack() : base() {
            arrayOfReadyQueues = new Queue<Process>[NUMBER_OF_QUEUES];
            for (int i = 0; i < NUMBER_OF_QUEUES; i++)
                arrayOfReadyQueues[i] = new Queue<Process>();

            readyQueue = null;

            quantums[0] = 10;
            quantums[1] = 10; 
         }
   
        /// <summary>
        /// Sort the preReadyQueue, then move the processes from the 
        /// preReadyQueue to the readyQueue, then clear the preReadyQueue
        /// If there is a CPU that does not a process in it, then put the next 
        /// process in the readyQueue in that CPU core
        /// </summary>
        public override void moveFromReadyToRunning() {
            preReadyQueue.Sort(processIDComparer);
            for (int i = 0; i < preReadyQueue.Count; i++) {
                arrayOfReadyQueues[0].Enqueue(preReadyQueue[i]);
            }

            preReadyQueue.Clear();
            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process == null)
                    CPUS[i].process = getNextProcess();
            }
        }

            /// <summary>
            /// Adds a process to the preReady Queue
            /// </summary>
        public override void addNewProcess() { 
            while(nextProcess < processes.Count && processes[nextProcess].arrivalTime <= ticks) {
                preReadyQueue.Add(processes[nextProcess++]);
            }
        }

        /// <summary>
        /// Gets the next process to be run based on their positions
        /// in the multiple queues
        /// </summary>
        public override Process getNextProcess() {
            int rQ1Size = arrayOfReadyQueues[0].Count;
            int rQ2Size = arrayOfReadyQueues[1].Count;
            int rQ3Size = arrayOfReadyQueues[2].Count;

            Process nextProcess = null;
            
            if(rQ1Size != 0) {
                nextProcess = arrayOfReadyQueues[0].Dequeue();
                if (nextProcess.responseTime == -1)
                    nextProcess.responseTime = ticks - nextProcess.arrivalTime;
            }
            else if (rQ2Size != 0) {
                nextProcess = arrayOfReadyQueues[1].Dequeue();
                if (nextProcess.responseTime == -1)
                    nextProcess.responseTime = ticks - nextProcess.arrivalTime;
            }
            else if (rQ3Size != 0) {
                nextProcess = arrayOfReadyQueues[2].Dequeue();
                if (nextProcess.responseTime == -1)
                    nextProcess.responseTime = ticks - nextProcess.arrivalTime;
            }
            return nextProcess;
        }

        public override void moveFromRunningToWaiting() {
            int rQ1Size = arrayOfReadyQueues[0].Count;
            int rQ2Size = arrayOfReadyQueues[1].Count;
            int rQ3Size = arrayOfReadyQueues[2].Count;

            for (int i = 0; i < CPUS.Count; i++) {
                // There is a process in the CPU
                if (CPUS[i].process != null) {
                    int step = CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step;
                    int length = CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].length;
                    int quantum = CPUS[i].process.quantumRemaining;
                    int priority = CPUS[i].process.priority;

                    // The burst is not done, and neither is the quantum time
                    if (step != length && quantum != quantums[0] && priority == 0) {
                        CPUS[i].process.quantumRemaining++;
                        CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step += 1;                       
                    }
                    // The burst is not done, but the quantum is up
                    // But the process back into the readyQueue
                    else if(step != length && quantum == quantums[0] && priority == 0) {
                        CPUS[i].process.quantumRemaining = 0;
                        CPUS[i].process.priority = 1;
                        contextSwitch++;
                        arrayOfReadyQueues[1].Enqueue(CPUS[i].process);
                        CPUS[i].process = null;
                    }

                    // The burst is not done, and the quantum time is not done
                    else if(step != length && quantum != quantums[1] && priority == 1) {
                        if(rQ1Size != 0) {
                            CPUS[i].process.quantumRemaining = 0;
                            contextSwitch++;
                            arrayOfReadyQueues[1].Enqueue(CPUS[i].process);
                            CPUS[i].process = null;
                        }
                        else {
                            CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step++;
                            CPUS[i].process.quantumRemaining += 1;
                        }
                    }

                    // The burst is not done but the quantum time is up
                    else if(step != length && quantum == quantums[1] && priority == 1) {
                        CPUS[i].process.quantumRemaining = 0;
                        CPUS[i].process.priority = 2;
                        contextSwitch++;
                        arrayOfReadyQueues[2].Enqueue(CPUS[i].process);
                        CPUS[i].process = null;
                    }

                    // The burst is not done, but the priority is 2
                    else if(step != length && priority == 2) {
                        if (rQ1Size != 0 || rQ2Size != 0) {
                            CPUS[i].process.quantumRemaining = 0;
                            contextSwitch++;
                            arrayOfReadyQueues[2].Enqueue(CPUS[i].process);
                            CPUS[i].process = null;
                        }
                        else {
                            CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step += 1;
                        }
                    }

                    // FCFS part
                    else if(step == length) {
                        CPUS[i].process.currentBurst += 1;
                        CPUS[i].process.quantumRemaining = 0;
                        CPUS[i].process.priority = 0;
                        if(CPUS[i].process.currentBurst < CPUS[i].process.numBursts) {
                            waitingQueue.Enqueue(CPUS[i].process);
                        }
                        else {
                            CPUS[i].process.endTime = ticks;
                        }
                        CPUS[i].process = null;
                    }
                }

                // Else if the CPU core is empty
                else if(CPUS[i].process == null) {
                    if(rQ1Size != 0) {
                        Process nextProcess = arrayOfReadyQueues[0].Dequeue();
                        CPUS[i].process = nextProcess;
                        CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step += 1;
                        CPUS[i].process.quantumRemaining += 1;
                    }
                    if (rQ2Size != 0) {
                        Process nextProcess = arrayOfReadyQueues[1].Dequeue();
                        CPUS[i].process = nextProcess;
                        CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step += 1;
                        CPUS[i].process.quantumRemaining += 1;
                    }
                    if (rQ3Size != 0) {
                        Process nextProcess = arrayOfReadyQueues[2].Dequeue();
                        CPUS[i].process = nextProcess;
                        CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step += 1;
                        CPUS[i].process.quantumRemaining += 1;
                    }
                }
            }
        }

        public override void moveFromWaitingToReady() {
            int waitingQueueCount = waitingQueue.Count;
            for (int i = 0; i < waitingQueueCount; i++ ) {
                Process nextProcess = waitingQueue.Dequeue();
                nextProcess.priority = 0;
                nextProcess.quantumRemaining = 0;
                if (nextProcess.bursts[nextProcess.currentBurst - 1].step == nextProcess.bursts[nextProcess.currentBurst - 1].length) {
                    nextProcess.currentBurst += 1;
                    preReadyQueue.Add(nextProcess);
                }
                else {
                    waitingQueue.Enqueue(nextProcess);
                }
            }
        }

        public override void updateProcessState() {

            int waitingQueueSize = waitingQueue.Count;
            // Update the waiting Queue
            for (int i = 0; i < waitingQueueSize; i++) {
                Process nextProcess = waitingQueue.Dequeue();
                nextProcess.bursts[nextProcess.currentBurst - 1].step += 1;
                waitingQueue.Enqueue(nextProcess);
            }

            // Update the ready processes
            for (int i = 0; i < NUMBER_OF_QUEUES; i++) {
                for(int j = 0; j < arrayOfReadyQueues[i].Count; j++) {
                    Process nextProcess = arrayOfReadyQueues[i].Dequeue();
                    nextProcess.waitingTime++;
                    arrayOfReadyQueues[i].Enqueue(nextProcess);
                }        
            }
        }

        public override void run(List<Core> CPUS, string filename) {
            this.CPUS = CPUS;

            // Make sure CPUS are empty
            for (int i = 0; i < CPUS.Count; i++) {
                CPUS[i].process = null;
            }

            // Make sure queues are empty
            waitingQueue.Clear();
            preReadyQueue.Clear();

            processes.Clear();

            // Initialize processes
            loadProcesses(filename);

            // Sort the processes by arrival time
            processes.Sort(arrivalTimeComparer);

            while(processes.Count != 0) {
                addNewProcess();
                moveFromRunningToWaiting();
                moveFromReadyToRunning();
                moveFromWaitingToReady();

                updateProcessState();

                cpuUtilizationTicks += numberOfCurrentRunningProcesses();

                if (numberOfCurrentRunningProcesses() == 0 && totalExpectedProcesses() == 0 && waitingQueue.Count == 0) {
                    break;
                }
                ticks += 1;              
            }

            int lastPID = 0;

            for (int i = 0; i < processes.Count; i++) {
                totalTurnaroundTime += processes[i].endTime - processes[i].arrivalTime;
                totalWaitingTime += processes[i].waitingTime;
                totalResponseTime += processes[i].responseTime;

                if (processes[i].endTime == ticks)
                    lastPID = processes[i].processID;
            }

            using (System.IO.StreamWriter writeText = System.IO.File.AppendText("MultiLevel1010.txt")) {
                writeText.WriteLine("Num Cores: " + CPUS.Count);
                writeText.WriteLine("Average Throughput: " + (float)processes.Count / ticks);
                writeText.WriteLine("Average Response Time: " + totalResponseTime / processes.Count);
                writeText.WriteLine("Average Wait Time: " + totalWaitingTime / processes.Count);
                writeText.WriteLine("Average Turnaround Time: " + totalTurnaroundTime / processes.Count);
                writeText.WriteLine("Average Utilization Time: " + (float)cpuUtilizationTicks * 100 / ticks / CPUS.Count + "%");
            }

            // Reset all variables
            ticks = 0;
            totalWaitingTime = 0;
            totalTurnaroundTime = 0;
            totalResponseTime = 0;
            cpuUtilizationTicks = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator
{
    public class RoundRobin : SchedulingAlgorithm
    {


        public RoundRobin(int quantum) : base() {
            quantumtime = quantum;
        }
        // </Summary>
        // Gets the list of Process in order from P1,P2...
        // Gets the created Processes from the PrereadyQueue. Checks to see if there are any open CPU's
        // to run more processes.
        // </summary>
        public override void moveFromReadyToRunning() {
            preReadyQueue.Sort(processIDComparer);
            for (int i = 0; i < preReadyQueue.Count; i++) {
                readyQueue.Enqueue(preReadyQueue[i]);
            }
            preReadyQueue.Clear();
            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process == null) {
                    CPUS[i].process = getNextProcess();
                }
                    
            }
        }

        // </summary>
        // Pretty much the same thing as the FCFS implementation. Checks to see if the the next process interger is
        // less than the processes created as well as to check if the arrival time from the previous
        // is less than the clock. After checking it creates a new process in the preReadyQueue.
        // </summary>
        public override void addNewProcess() {
            while (nextProcess < processes.Count && processes[nextProcess].arrivalTime <= ticks) {
                preReadyQueue.Add(processes[nextProcess]);
                processes[nextProcess].quantumRemaining = quantumtime;
                nextProcess++;     
            }
        }

        /// <summary>
        /// Check the processes in the waiting queue to see if their I/O burst is complete
        /// If it is, then move on to the next burst for that process and add it to the pre-ready queue
        /// </summary>
        public override void moveFromWaitingToReady() {
            int waitingQueueSize = waitingQueue.Count;
            for (int i = 0; i < waitingQueueSize; i++) {
                Process next_Process = waitingQueue.Dequeue();
                int step = next_Process.bursts[next_Process.currentBurst - 1].step;
                int length = next_Process.bursts[next_Process.currentBurst - 1].length;
                if (step == length) {
                    next_Process.currentBurst += 1;
                    next_Process.quantumRemaining = quantumtime;
                    next_Process.endTime = ticks;
                    preReadyQueue.Add(next_Process);
                } else {
                    waitingQueue.Enqueue(next_Process);
                }
            }
        }


        //</Summary>
        // Getting the next process that is ready to be used in burst time and quantun time.
        // </summary>
        public override Process getNextProcess()
        {
            if (readyQueue.Count == 0)
            {
                return null;
            }
            Process nextProcess = readyQueue.Dequeue();
            if (nextProcess.responseTime == -1)
                nextProcess.responseTime = ticks - nextProcess.arrivalTime;
            return nextProcess;
        }

       

        public override void moveFromRunningToWaiting() {
            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process != null) {
                    int step = CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step;
                    int length = CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].length;
                    if (step == length) {
                        CPUS[i].process.currentBurst += 1;
                        if (CPUS[i].process.currentBurst < CPUS[i].process.numBursts) {
                            waitingQueue.Enqueue(CPUS[i].process);
                        }
                        else {
                            CPUS[i].process.endTime = ticks;
                        }
                        CPUS[i].process = null;
                    }
                    // context switch takes longer than time slice
                    else if (CPUS[i].process.quantumRemaining == 0) {
                        Process new_process = CPUS[i].process;
                        new_process.quantumRemaining = quantumtime;
                        readyQueue.Enqueue(new_process);
                        contextSwitch++;
                        CPUS[i].process = null;
                    }
                }
            }
        }


        // </summary>
        // Used to update the state of a process whenever we need to send the processes back to the waiting
        // queue and or readyqueue.
        // </summary>
        public override void updateProcessState() {
            for (int i = 0; i < readyQueue.Count; i++) {
                Process next_process = readyQueue.Dequeue();
                next_process.waitingTime++;
                readyQueue.Enqueue(next_process);
            }

            for (int j = 0; j < waitingQueue.Count; j++) {
                Process next_process = waitingQueue.Dequeue();
                next_process.bursts[next_process.currentBurst - 1].step += 1;
                waitingQueue.Enqueue(next_process);
            }

            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process != null) {
                    CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step += 1;
                    CPUS[i].process.quantumRemaining -= 1;
                }
            }
        }


        public override void run(List<Core> CPUS, string filename) {
            nextProcess = 0;
            this.CPUS = CPUS;

            // Make sure the CPUs are empty
            for (int i = 0; i < CPUS.Count; i++) {
                CPUS[i].process = null;
            }

            // Make sure the queues are empty
            readyQueue.Clear();
            waitingQueue.Clear();
            preReadyQueue.Clear();

            processes.Clear();

            // Load the processes
            loadProcesses(filename);

            // Sort the processes by arrival time
            processes.Sort(arrivalTimeComparer);

            /// **** ERROR FOUND!!!
            /// This is an infinity loop, Processes.Count will always be 100!!! so it will never reach 0!
            /// Do we need to decrement this? 

            while (processes.Count != 0) {
                addNewProcess();
                moveFromRunningToWaiting();
                moveFromReadyToRunning();
                moveFromWaitingToReady();

                updateProcessState();

                cpuUtilizationTicks += numberOfCurrentRunningProcesses();

                // If there are no more running processes and the waiting queue is empty, break
                if (numberOfCurrentRunningProcesses() == 0 && totalExpectedProcesses() == 0 && waitingQueue.Count == 0)
                    break;

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

            using (System.IO.StreamWriter writeText = System.IO.File.AppendText("RoundRobinData.txt")) {
                writeText.WriteLine("Num Cores: " + CPUS.Count);
                writeText.WriteLine("Quantum: " + quantumtime);
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

            // Calculate data stuff
        }
    }
}

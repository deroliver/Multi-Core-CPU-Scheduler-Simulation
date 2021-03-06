﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class FirstComeFirstServe : SchedulingAlgorithm {

        /// <summary>
        /// Sort the processes in the preReadyQueue by process ID
        /// Then add then move all processes from the preReadyQueue
        /// to the readyQueue. Find a CPU that is not utilized, and
        /// run the process on it.
        /// </summary>
        public override void moveFromReadyToRunning() {
            preReadyQueue.Sort(processIDComparer);
            for (int i = 0; i < preReadyQueue.Count; i++) {
                readyQueue.Enqueue(preReadyQueue[i]);
            }
            preReadyQueue.Clear();

            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process == null)
                    CPUS[i].process = getNextProcess();
            }
        }

        /// <summary>
        /// Simply adds a process to the preReady queue
        /// </summary>
        public override void addNewProcess() {
            while (nextProcess < processes.Count && processes[nextProcess].arrivalTime <= ticks) {
                preReadyQueue.Add(processes[nextProcess++]);
            }
        }

        /// <summary>
        /// Gets next process in the ready queue. Return the next process
        /// </summary>
        /// <returns></returns>
        public override Process getNextProcess() {
            if (readyQueue.Count == 0) {
                return null;
            }
            Process nextProcess = readyQueue.Dequeue();
            if (nextProcess.responseTime == -1)
                nextProcess.responseTime = ticks - nextProcess.arrivalTime;
            return nextProcess;
        }

        /// <summary>
        /// Checks to see if any of the processes in the CPU cores have finished their
        /// CPU burst. If they have, move it out of the CPU and start the IO burst.
        /// 
        /// If the CPU has not finished its CPU burst, then move it to the waitingQueue
        /// and free the CPU it was using. 
        /// 
        /// If the process has completed, set its endTime, and terminate it.
        /// </summary>
        public override void moveFromRunningToWaiting() {
            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process != null) {
                    int burstStep = CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step;
                    int burstLength = CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].length;
                    //Console.WriteLine(CPUS[i].process.processID + "   " + burstStep + "    " + burstLength);
                    if (burstStep == burstLength) {
                        CPUS[i].process.currentBurst += 1;                    
                        if (CPUS[i].process.currentBurst - 1 < CPUS[i].process.numBursts) {
                            waitingQueue.Enqueue(CPUS[i].process);
                        } else {
                            CPUS[i].process.endTime = ticks;
                        }
                        CPUS[i].process = null;
                    }
                }
            }
        }

        /// <summary>
        /// Get the first process in the waiting queue and check if it has completed
        /// its IO burst. If it has completed, move it to the preReadyQueue. And
        /// remove it from the waitingQueue.
        /// </summary>
        public override void moveFromWaitingToReady() {
            int waitingQueueSize = waitingQueue.Count;
            for (int i = 0; i < waitingQueueSize; i++) {
                Process nextProcess = waitingQueue.Dequeue();
                int step = nextProcess.bursts[nextProcess.currentBurst - 1].step;
                int length = nextProcess.bursts[nextProcess.currentBurst - 1].length;

                int numBursts = nextProcess.numBursts;
                int currentBurst = nextProcess.currentBurst;
                if (step == length) {
                    nextProcess.currentBurst += 1;
                    preReadyQueue.Add(nextProcess);              
                } else {
                    waitingQueue.Enqueue(nextProcess);
                }
            }
        }

        /// <summary>
        /// Updates waiting processes (IO Burst), ready processes, and running processes (CPU Burst)
        /// </summary>
        public override void updateProcessState() {
            // Update the waiting queue
            int waitingQueueCount = waitingQueue.Count;
            for (int i = 0; i < waitingQueueCount; i++) {
                Process nextProcess = waitingQueue.Dequeue();
                    nextProcess.bursts[nextProcess.currentBurst - 1].step += 1;
                    waitingQueue.Enqueue(nextProcess);               
            }

            // Update the ready queue
            for (int i = 0; i < readyQueue.Count; i++) {
                Process nextProcess = readyQueue.Dequeue();
                nextProcess.waitingTime++;
                readyQueue.Enqueue(nextProcess);
            }

            // Update the running queue (CPUS)
            for (int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process != null) {
                    CPUS[i].process.bursts[CPUS[i].process.currentBurst - 1].step++;
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


            using (System.IO.StreamWriter writeText = System.IO.File.AppendText("FirstComeFirstServeData.txt")) {
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

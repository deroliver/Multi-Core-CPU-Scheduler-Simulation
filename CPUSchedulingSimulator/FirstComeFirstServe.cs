using System;
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
            for(int i = 0; i < preReadyQueue.Count; i++) {
                readyQueue.Enqueue(preReadyQueue[i]);
            }

            for(int i = 0; i < CPUS.Count; i++) {
                if (CPUS[i].process == null)
                    CPUS[i].process = getNextProcess();
            }
        }

        /// <summary>
        /// Simply adds a process to the preReady queue
        /// </summary>
        public override void addNewProcess() {
            while(nextProcess < processes.Count && processes[nextProcess].arrivalTime <= ticks) {
                preReadyQueue.Add(processes[nextProcess++]);
            }
        }

        /// <summary>
        /// Gets next process in the ready queue. Return the next process
        /// </summary>
        /// <returns></returns>
        public override Process getNextProcess() {
            if(readyQueue.Count == 0) {
                return null;
            }
            Process nextProcess = readyQueue.Dequeue();
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
            for(int i = 0; i < CPUS.Count; i++) {
                if(CPUS[i].process != null) {
                    int burstStep = CPUS[i].process.bursts[CPUS[i].process.currentBurst].step;
                    int burstLength = CPUS[i].process.bursts[CPUS[i].process.currentBurst].length;
                    if (burstStep == burstLength) {
                        CPUS[i].process.currentBurst += 1;
                        if(CPUS[i].process.currentBurst < CPUS[i].process.numBursts) {
                            waitingQueue.Enqueue(CPUS[i].process);
                        }
                        else {
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
            for(int i = 0; i < waitingQueue.Count; i++) {
                Process nextProcess = waitingQueue.Dequeue();
                if(nextProcess.bursts[nextProcess.currentBurst].step == nextProcess.bursts[nextProcess.currentBurst].length) {
                    nextProcess.currentBurst += 1;
                    preReadyQueue.Add(nextProcess);
                }
                else {
                    waitingQueue.Enqueue(nextProcess);
                }
            }
        }

        /// <summary>
        /// Updates waiting processes (IO Burst), ready processes, and running processes (CPU Burst)
        /// </summary>
        public override void updateProcessState() {
            // Update the waiting queue
            for(int i = 0; i < waitingQueue.Count; i++) {
                Process nextProcess = waitingQueue.Dequeue();
                nextProcess.bursts[nextProcess.currentBurst].step += 1;
                waitingQueue.Enqueue(nextProcess);
            }

            // Update the ready queue
            for (int i = 0; i < readyQueue.Count; i++) {
                Process nextProcess = readyQueue.Dequeue();
                nextProcess.bursts[nextProcess.currentBurst].step += 1;
                nextProcess.waitingTime++;
                readyQueue.Enqueue(nextProcess);
            }

            // Update the running queue (CPUS)
            for(int i = 0; i < CPUS.Count; i++) {
                if(CPUS[i].process != null) {
                    CPUS[i].update();
                }
            }
        }

        public override void run(List<Core> CPUS, string filename) {
            int status = 0;
            this.CPUS = CPUS;
            
            // Make sure the CPUs are empty
            for(int i = 0; i < CPUS.Count; i++) {
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

            int totalTurnaround = 0;
            int totalWaitingtime = 0;
            int lastPID = 0;

            for(int i = 0; i < processes.Count; i++) {
                totalTurnaround += processes[i].endTime - processes[i].arrivalTime;
                totalWaitingtime += processes[i].waitingTime;

                if (processes[i].endTime == ticks)
                    lastPID = processes[i].processID;
            }

            Console.WriteLine("Average Wait Time: " + totalWaitingtime / processes.Count);
            Console.WriteLine("Average Turnaround Time: " + totalTurnaround / processes.Count);
            Console.WriteLine("Average Utilization Time: " + cpuUtilizationTicks * 100 / ticks);
            //Console.WriteLine("Total Context Switches: " + cpuUtilizationTicks * 100 / ticks);

            // Calculate data stuffs
        }
    }
}

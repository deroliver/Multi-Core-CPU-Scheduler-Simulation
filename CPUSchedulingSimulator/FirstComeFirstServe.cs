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
                if (CPUS[i] == null)
                    CPUS[i] = getNextProcess();
            }
        }

        public override void addNewProcess() {
            while(nextProcess < numberOfProcesses && processes[nextProcess].arrivalTime <= ticks) {
                preReadyQueue.Add(processes[nextProcess++]);
            }
        }

        /// <summary>
        /// Gets next process in th queue. Return the next process
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
        /// Checks if the current process has finished its CPU burst. Then move it to the
        /// waitingQueue. If the process has finished its CPU burst, then terminate it.
        /// Start the next I/O burst. If the process has not finished its CPU burst, move that
        /// process to the waitingQueue and free the CPU it was using. If the current process 
        /// has finished set the endTime to the current time, and terminate it.
        /// </summary>
        public override void moveFromRunningToWaiting() {
            for(int i = 0; i < CPUS.Count; i++) {
                if(CPUS[i] != null) {
                    if(CPUS[i].bursts[CPUS[i].currentBurst].step == CPUS[i].bursts[CPUS[i].currentBurst].length) {
                        CPUS[i].currentBurst += 1;
                        if(CPUS[i].currentBurst < CPUS[i].numBursts) {
                            waitingQueue.Enqueue(CPUS[i]);
                        }
                        else {
                            CPUS[i].endTime = ticks;
                        }
                        CPUS[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Grab the first process that is in the waiting queue and check if its
        /// I/O burst is complete. Then move to the next I/O burst and and add 
        /// to the preReadyQueue. Then remove the process from the waiting queue
        /// and increment the burst's step.
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
        /// Updates waiting processes, ready processes, and running processes
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
                readyQueue.Enqueue(nextProcess);
            }

            // Update the running queue (CPUS)
            for(int i = 0; i < CPUS.Count; i++) {
                if(CPUS[i] != null) {
                    CPUS[i].bursts[CPUS[i].currentBurst].step += 1;
                }
            }
        }

        public void run() {
            int status = 0;
            
            for(int i = 0; i < CPUS.Count; i++) {
                CPUS[i] = null;
            }

            readyQueue.Clear();
            waitingQueue.Clear();

            // Read in process data and add processes to process List

            // Sort the processes by arrival time
            processes.Sort(arrivalTimeComparer);

            while(numberOfProcesses != 0) {
                addNewProcess();
                moveFromRunningToWaiting();
                moveFromReadyToRunning();
                moveFromWaitingToReady();

                updateProcessState();

                cpuUtilizationTicks += runningProcesses();

                // If there are no more running processes and the waiting queue is empty, break
                if (runningProcesses() == 0 && totalExpectedProcesses() == 0 && waitingQueue.Count == 0)
                    break;

                ticks += 1;
            }


            // Calculate data stuffs
        }

        
    }
}

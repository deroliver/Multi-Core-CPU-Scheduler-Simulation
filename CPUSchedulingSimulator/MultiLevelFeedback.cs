using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator
{
    public class MultiLevelFeedBack : SchedulingAlgorithm
    {
        public override void moveFromReadyToRunning()
        {
            preReadyQueue.Sort(processIDComparer);
            for (int i = 0; i < preReadyQueue.Count; i++)
            {
                readyQueue.Enqueue(preReadyQueue[i]);
            }

            for (int i = 0; i < CPUS.Count; i++)
            {
                if (CPUS[i].process == null)
                    CPUS[i].process = getNextProcess();
            }
        }

            /// <summary>
            /// Adds a process to the preReady Queue
            /// </summary>
            public override void addNewProcess()
        {
            while(nextProcess < processes.Count && processes[nextProcess].arrivalTime <= ticks)
            {
                preReadyQueue.Add(processes[nextProcess++]);
            }
        }

        /// <summary>
        /// Gets next process to the preReady Queue
        /// </summary>
        public override Process getNextProcess()
        {
            if (readyQueue.Count == 0)
            {
                return null;
            }
            Process nextProcess = readyQueue.Dequeue();
            return nextProcess;
        }

        public override void moveFromRunningToWaiting()
        {
            for (int i = 0; i < CPUS.Count; i++)
            {
                if (CPUS[i].process != null)
                {
                    int burstStep = CPUS[i].process.bursts[CPUS[i].process.currentBurst].step;
                    int burstLength = CPUS[i].process.bursts[CPUS[i].process.currentBurst].length;

                    if (burstStep == burstLength)
                    {
                        CPUS[i].process.currentBurst += 1;

                        if (CPUS[i].process.currentBurst < CPUS[i].process.numBursts)
                        {
                            waitingQueue.Enqueue(CPUS[i].process);
                        }
                        else
                        {
                            CPUS[i].process.endTime = ticks;
                        }
                        CPUS[i].process = null;
                    }
                }
            }
        }

        public override void moveFromWaitingToReady()
        {
            for (int i = 0; i < waitingQueue.Count; i++ )
            {
                Process nextProcess = waitingQueue.Dequeue();
                if (nextProcess.bursts[nextProcess.currentBurst].step == nextProcess.bursts[nextProcess.currentBurst].length)
                {
                    nextProcess.currentBurst += 1;
                    preReadyQueue.Add(nextProcess);
                }
                else
                {
                    waitingQueue.Enqueue(nextProcess);
                }
            }
        }

        public override void updateProcessState()
        {
            // Update the waiting Queue
            for (int i = 0; i < waitingQueue.Count; i++ )
            {
                Process nextProcess = waitingQueue.Dequeue();
                nextProcess.bursts[nextProcess.currentBurst].step += 1;
            }

            // Update the ready Queue
            for (int i = 0; i < readyQueue.Count; i++)
            {
                Process nextProcess = readyQueue.Dequeue();
                nextProcess.bursts[nextProcess.currentBurst].step += 1;
                readyQueue.Enqueue(nextProcess);
            }

            // Update the running Queue (CPUS)
            for (int i = 0; i < CPUS.Count; i++)
            {
                if (CPUS[i].process != null)
                {
                    CPUS[i].update();
                }
            }
        }

        public override void run(List<Core> CPUS)
        {
            int status = 0;
            this.CPUS = CPUS;

            // Make sure CPUS are empty
            for (int i = 0; i < CPUS.Count; i++)
            {
                CPUS[i].process = null;
            }

            // Make sure queues are empty
            readyQueue.Clear();
            waitingQueue.Clear();
            preReadyQueue.Clear();

            // Initialize processes
            loadProcesses("Some File");

            // Sort the processes by arrival time
            processes.Sort(arrivalTimeComparer);

            while(processes.Count != 0)
            {
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
        }
    }
}

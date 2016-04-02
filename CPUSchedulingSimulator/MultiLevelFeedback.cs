using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator
{
    public class MultiLevelFeedBack : SchedulingAlgorithm
    {
        List<Process> p = new List<Process>();
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
                waitingQueue.Enqueue(nextProcess);
            }

            // Update the ready Queue
            for (int i = 0; i < readyQueue.Count; i++)
            {
                Process nextProcess = readyQueue.Dequeue();
                nextProcess.bursts[nextProcess.currentBurst].step += 1;
                nextProcess.waitingTime++;
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

        public override void run(List<Core> CPUS, string filename)
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
            loadProcesses(filename);

            // Sort the processes by arrival time
            processes.Sort(arrivalTimeComparer);

            /// **** ERROR FOUND!!!
            /// This is an infinity loop, Processes.Count will always be 100!!! so it will never reach 0!
            /// Do we need to decrement this? 

            while(processes.Count != 0)
            {
                addNewProcess();
                moveFromRunningToWaiting();
                moveFromReadyToRunning();
                moveFromWaitingToReady();

                updateProcessState();

                cpuUtilizationTicks += numberOfCurrentRunningProcesses();

                // If there are no more running processes and the waiting queue is empty, break
                Console.WriteLine("I made it here before if statement");
                if (numberOfCurrentRunningProcesses() == 0 && totalExpectedProcesses() == 0 && waitingQueue.Count == 0)
                {
                    Console.WriteLine("I made it into the if statement");
                    break;
                }
                Console.WriteLine("If statement broke, starting ticks");
                ticks += 1;
                
                /// Created a foreach statement that removes each process, but when this is done
                /// At the end it completely removes the processes, so there is a count of 0
                /// Could use some tuning.
                /// Might want to stay away from foreach since it will loop until processes = 0
                /// as a result, the ^^^^ functions/methods above won't be called at all.

                foreach(Process p in processes.ToArray())
                {
                    if (processes.Count != 0)
                    {
                        processes.Remove(p);
                        Console.WriteLine("Removed a process");
                    }
                }
            }
            int totalTurnaround = 0;
            int totalWaitingtime = 0;
            int lastPID = 0;

            for (int i = 0; i < processes.Count; i++)
            {
                totalTurnaround += processes[i].endTime - processes[i].arrivalTime;
                totalWaitingtime += processes[i].waitingTime;

                if (processes[i].endTime == ticks)
                    lastPID = processes[i].processID;
            }

            //Console.WriteLine("Average Wait Time: " + totalWaitingtime / processes.Count);
            //Console.WriteLine("Average Turnaround Time: " + totalTurnaround / processes.Count);
            Console.WriteLine("Average Utilization Time: " + cpuUtilizationTicks * 100 / ticks);
            Console.WriteLine("Total Context Switches: " + cpuUtilizationTicks * 100 / ticks);
        }
    }
}

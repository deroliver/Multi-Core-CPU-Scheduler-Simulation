using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator
{
    public class RoundRobin : SchedulingAlgorithm
    {
        int contextswitch;
        // </Summary>
        // Gets the list of Process in order from P1,P2...
        // Gets the created Processes from the PrereadyQueue. Checks to see if there are any open CPU's
        // to run more processes.
        // </summary>
        public override void moveFromReadyToRunning()
        {
            preReadyQueue.Sort(processIDComparer);
            for (int i = 0; i < preReadyQueue.Count;i++)
            {
                readyQueue.Enqueue(preReadyQueue[i]);
            }

            for (int i = 0; i < CPUS.Count;i++)
            {
                if (CPUS[i].process == null)
                {
                    CPUS[i].process = getNextProcess();
                }
            }
        }

        // </summary>
        // Pretty much the same thing as the FCFS implementation. Checks to see if the the next process interger is
        // less than the processes created as well as to check if the arrival time from the previous
        // is less than the clock. After checking it creates a new process in the preReadyQueue.
        // </summary>
        public override void addNewProcess()
        {
            while (nextProcess < processes.Count && processes[nextProcess].arrivalTime <= ticks)
            {
                preReadyQueue.Add(processes[nextProcess++]);
                processes[nextProcess].quantumRemaining = quantumtime;
                
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
            return nextProcess;
        }

        public override void moveFromWaitingToReady()
        {
            for (int i = 0;i < waitingQueue.Count;i++)
            {
                Process Next_Process = waitingQueue.Dequeue();
                if (Next_Process.bursts[Next_Process.currentBurst].step == Next_Process.bursts[Next_Process.currentBurst].length)
                {  
                    Next_Process.currentBurst += 1;
                    Next_Process.quantumRemaining = quantumtime;
                    Next_Process.endTime = ticks;
                    preReadyQueue.Add(Next_Process);
                }
                else {
                    waitingQueue.Enqueue(Next_Process);
                }

            }
        }

        // </summary>
        // Used to update the state of a process whenever we need to send the processes back to the waiting
        // queue and or readyqueue.
        // </summary>
        public override void updateProcessState()
        {
            for (int i = 0;i < readyQueue.Count;i++)
            {
                Process next_process = readyQueue.Dequeue();
                next_process.bursts[next_process.currentBurst].step += 1;
                readyQueue.Enqueue(next_process);
            }

            for (int j = 0;j < waitingQueue.Count;j++)
            {
                Process next_process = waitingQueue.Dequeue();
                next_process.bursts[next_process.currentBurst].step += 1;
                waitingQueue.Enqueue(next_process);
            }

            for (int i = 0; i < CPUS.Count; i++)
            {
                if (CPUS[i].process != null)
                {
                    CPUS[i].update();
                }
            }
        }

        public override void moveFromRunningToWaiting()
        {

            for (int i = 0; i < CPUS.Count; i++)
            {
                //int num = 0;
                if (CPUS[i] != null)
                {
                    if (CPUS[i].process.bursts[CPUS[i].process.currentBurst].step == CPUS[i].process.bursts[CPUS[i].process.currentBurst].length)
                    {
                        CPUS[i].process.currentBurst += 1;
                        if (CPUS[i].process.currentBurst < CPUS[i].process.numBursts)
                        {
                            waitingQueue.Enqueue(CPUS[i].process);
                        }
                        else {
                            CPUS[i].process.endTime = ticks;
                        }
                        CPUS[i] = null;
                    }
                    // context switch takes longer than time slice
                    else if (CPUS[i].process.quantumRemaining == 0)
                    {
                        Process new_process = CPUS[i].process;
                        new_process.quantumRemaining = quantumtime;
                        readyQueue.Enqueue(new_process);
                        contextSwitch++;
                        CPUS[i] = null;
                    }
                }
            }
        }

        public override void run(List<Core> CPUS)
        {
            throw new NotImplementedException();
        }
    }
}

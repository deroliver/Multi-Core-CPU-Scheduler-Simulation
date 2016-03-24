using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator
{
    public class RoundRobin : SchedulingAlgorithm
    {
        public override void moveFromReadyToRunning()
        {

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
            }
        }

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
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        public override void run(List<Core> CPUS)
        {
            throw new NotImplementedException();
        }
    }
}

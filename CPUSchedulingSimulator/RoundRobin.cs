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
            throw new NotImplementedException();
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

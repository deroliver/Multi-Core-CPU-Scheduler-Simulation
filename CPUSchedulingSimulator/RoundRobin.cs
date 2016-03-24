using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator
{
    public class RoundRobin : SchedulingAlgorithm
    {
        // Feel free to change it to something different
        int timeQuantum = 10;

        public override void moveFromReadyToRunning()
        {
            throw new NotImplementedException();
        }

        public override void addNewProcess()
        {
            throw new NotImplementedException();
        }

        public override Process getNextProcess()
        {
            throw new NotImplementedException();
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

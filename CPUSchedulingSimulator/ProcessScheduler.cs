using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class ProcessScheduler {

        private SchedulingAlgorithm algorithm;

        #region Data Average Calculators

        /// <summary>
        /// Calculates the average wait time for all processes
        /// </summary>
        /// <param name="totalWait">The total wait time for all processes</param>
        /// <returns></returns>
        double waitTimeAverage(int totalWait) {
            double result = totalWait / (double)algorithm.numberOfProcesses;
            return result;
        }

        /// <summary>
        /// Calculates the average turnaround time for all processes
        /// </summary>
        /// <param name="totalTurnAround">The total turnaround time for all processes</param>
        /// <returns></returns>
        double turnaroundTimeAverage(int totalTurnAround) {
            double result = totalTurnAround / (double)algorithm.numberOfProcesses;
            return result;
        }

        /// <summary>
        /// Calculates the average CPU utilization
        /// </summary>
        /// <param name="cpuUtilization">The time of CPU utilization</param>
        /// <returns></returns>
        double cpuUtilizationTime(int cpuUtilization) {
            double result = (cpuUtilization * 100) / algorithm.ticks;
            return result;
        }

        #endregion


        

        
    }
}

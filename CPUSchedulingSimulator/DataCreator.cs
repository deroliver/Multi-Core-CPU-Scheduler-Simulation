using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class DataCreator {
        public void Test() {
            int numCores = 0;
            int quantum = 0;
            SchedulingAlgorithm RoundRobin;
            SchedulingAlgorithm FirstComeFirst;
            Simulator S;

            /*
            for (int i = 0; i < 5; i++) {
                RoundRobin = new RoundRobin(quantum += 10);
                for(int j = 0; j < 4; j++) {
                    S = new Simulator(numCores += 2, RoundRobin);
                    S.run();
                }
                numCores = 0;
            }
            */

            for(int i = 0; i < 4; i++) {
                FirstComeFirst = new MultiLevelFeedBack();
                S = new Simulator(numCores += 2, FirstComeFirst);
                S.run();
            }
        }
    }
}

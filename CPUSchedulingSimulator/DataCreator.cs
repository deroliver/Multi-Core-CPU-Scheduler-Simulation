﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSchedulingSimulator {
    public class DataCreator {
        public void Test() {
            int numCores = 2;
            int quantum = 5;
            SchedulingAlgorithm RoundRobin;
            Simulator S;

            for (int i = 0; i < 20; i++) {
                RoundRobin = new RoundRobin(quantum += 5);
                for(int j = 0; j < 4; j++) {
                   // S = new Simulator(numCores += 2, RoundRobin);
                    //S.run();
                }
            }
        }
    }
}

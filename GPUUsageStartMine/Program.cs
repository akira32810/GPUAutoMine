using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPUUsageStartMine
{
    class Program
    {
        static void Main(string[] args)
        {

            int countX_StartProcess = 0;
   
          
           
            while (true)
            {
                try
                {
                    Process[] myMiner = Process.GetProcessesByName("miner");
                    Process[] myP = Process.GetProcesses();
              
                    var myPList = myP.ToList().Where(x => x.PrivateMemorySize64 / 1024 / 1024 > 2000);
                    var PListExeceptMiner = myPList.Where(x => !x.ProcessName.Contains("miner")).Count();
                   // var PListExeceptMiner = myPList.Except(PListMiner).First().ProcessName;
                    //var gpuCounters = GetGPUCounters();
                    //var gpuUsage = GetGPUUsage(gpuCounters);
                  
                    Console.WriteLine(PListExeceptMiner);

                    if (PListExeceptMiner==0)
                    {
                        countX_StartProcess++;
                    }

                    if (PListExeceptMiner>0)
                    {
                        countX_StartProcess = 0;
                        foreach (var process in  myMiner)
                        {
                           
                            process.Kill();
                        }

                    }
                              

                    if (countX_StartProcess >= 3 && myMiner.Length==0)
                    {


                        string strCmdText = Properties.Resources.minerStr;
                        System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                       

                    }

                   
                    Thread.Sleep(4000);
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                    continue;
                }


            }
        }

        public static List<PerformanceCounter> GetGPUCounters()
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            var counterNames = category.GetInstanceNames();

            var gpuCounters = counterNames
                                .Where(counterName => counterName.EndsWith("engtype_3D"))
                                .SelectMany(counterName => category.GetCounters(counterName))
                                .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
                                .ToList();

            return gpuCounters;
        }

        public static float GetGPUUsage(List<PerformanceCounter> gpuCounters)
        {
            gpuCounters.ForEach(x => x.NextValue());

            Thread.Sleep(1000);

            var result = gpuCounters.Sum(x => x.NextValue());

            return result;
        }
    }

}

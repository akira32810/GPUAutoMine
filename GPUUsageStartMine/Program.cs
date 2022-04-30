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
            int countY_StopProcess = 0;

            while (true)
            {
                try
                {
                    Process[] pname = Process.GetProcessesByName("miner");
                    var gpuCounters = GetGPUCounters();
                    var gpuUsage = GetGPUUsage(gpuCounters);
                    Console.WriteLine(gpuUsage);

                    if (gpuUsage < 70 && pname.Length == 0)
                    {
                        countX_StartProcess++;
                        countY_StopProcess = 0;
                    }

                    if (countX_StartProcess >= 3)
                    {


                        string strCmdText = Properties.Resources.minerStr;
                        System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                        countX_StartProcess = 0;

                    }

                    if (gpuUsage > 70 && pname.Length > 0)
                    {
                        countY_StopProcess++;
                        countX_StartProcess = 0;

                    }

                    if (countY_StopProcess >= 3)
                    {
                        foreach (var process in pname)
                        {
                            process.Kill();
                        }
                        countY_StopProcess = 0;
                    }

                    Thread.Sleep(4000);
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                }

                Thread.Sleep(4000);
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

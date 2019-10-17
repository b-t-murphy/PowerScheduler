using System;
using Topshelf;

namespace PowerScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure the Windows service using Topshelf
            TopshelfExitCode ranService = HostFactory.Run(serviceConfig =>
            {
                serviceConfig.SetServiceName("PowerPosition");
                serviceConfig.SetDisplayName("Power Position");
                serviceConfig.SetDescription("Service to aggregate power trades and save day ahead position");

                serviceConfig.Service<PositionService>(serviceInstance =>
                {
                    serviceInstance.ConstructUsing(() => new PositionService());
                    serviceInstance.WhenStarted(execute => execute.Start());
                    serviceInstance.WhenStopped(execute => execute.Stop());
                });

                serviceConfig.UseNLog();
                serviceConfig.RunAsNetworkService();
                serviceConfig.StartAutomatically();
            });
        }
    }
}

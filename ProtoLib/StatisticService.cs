
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProtoLib
{
    public static class ServiceInjector
    {
        public static void Inject(IServiceCollection serviceCollection)
        {
            StatisticService ss = new StatisticService();
            serviceCollection.AddSingleton(ss);
          
            
        }
    }
    public class StatisticService
    {
        public StatisticService()
        {
            BaseContext.PostWorkStatusChanged += BaseContextOnPostWorkStatusChanged;
        }

        private void BaseContextOnPostWorkStatusChanged()
        {
            Task.Run(async () =>
            {
                // Debug.WriteLine("START ");
                await Task.Delay(50);
                //Debug.WriteLine("END");
                PostStatisticManager psm = new PostStatisticManager();
                psm.SaveStat(psm.GetStat());
            });
        }
    }
}
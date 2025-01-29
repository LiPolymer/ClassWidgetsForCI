using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassWidgetsForCI;

[PluginEntrance]
public class Plugin : PluginBase {
    public override void Initialize(HostBuilderContext context,IServiceCollection services) {
        Guide guide = new Guide();
        new Thread(() => {
            Thread.Sleep(5000);
            guide.Dispatcher.BeginInvoke(() => {
                guide.Show();
            });
        }).Start();
    }
}
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwitchWallpaperSettings.Controls.ActionSettingsControls;
using SwitchWallpaperSettings.Actions;
namespace SwitchWallpaperSettings;
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // 注册自定义行动
        services.AddAction<SwitchWallpaperAction, SwitchWallpaperActionSettingsControl>();
        // 好耶,是女装
    }
}
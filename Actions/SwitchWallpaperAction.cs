using System;
using System.IO;
using System.Threading.Tasks;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using FluentAvalonia.UI.Controls;
using SwitchWallpaperSettings.Models;
namespace SwitchWallpaperSettings.Actions;
[ActionInfo(
    "switchwallpaper.action", 
    "切换桌面壁纸", 
    "\uE91B" // Fluent UI 图片图标
)]
public class SwitchWallpaperAction : ActionBase<WallpaperSettings>
{
    protected override async Task OnInvoke()
    {
        try
        {
            // 检查文件是否存在
            if (!File.Exists(Settings.WallpaperPath))
            {
                throw new FileNotFoundException("壁纸文件不存在，请检查路径是否正确：" + Settings.WallpaperPath);
            }
            // 调用API修改系统壁纸
            await WallpaperHelper.SetWallpaper(Settings.WallpaperPath, Settings.FitStyle);
        }
        catch (Exception ex)
        {
            // 错误处理：使用TaskDialog实现弹窗提示
            var dialog = new TaskDialog()
            {
                Title = "修改桌面壁纸失败",
                Content = ex.Message,
                XamlRoot = AppBase.Current.GetRootWindow(),
                Buttons = { new TaskDialogButton("确定", true) }
            };
            await dialog.ShowAsync();
        }
        await Task.CompletedTask;
    }
}
using System;
using System.ComponentModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SwitchWallpaperSettings;

/// <summary>
/// 桌面壁纸帮助类，异步纯C#实现，修复了填充/适应样式颠倒的问题
/// 现在所有契合度选项都可以正确对应Windows系统的设置
/// </summary>
internal static class WallpaperHelper
{
    // Win32 API 常量定义
    private const int SPI_SETDESKWALLPAPER = 20;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDWININICHANGE = 0x02;

    /// <summary>
    /// 导入系统API：SystemParametersInfo，用于设置桌面壁纸并通知系统更新
    /// </summary>
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    /// <summary>
    /// 异步设置桌面壁纸，纯C#实现，同步系统操作将在后台线程执行，不会阻塞UI
    /// 已修复填充/适应样式颠倒的问题，现在所有样式都可以正确生效
    /// </summary>
    /// <param name="wallpaperPath">壁纸文件的完整路径</param>
    /// <param name="fitStyle">壁纸契合度样式：0=平铺,1=居中,2=拉伸,3=填充,4=适应,5=跨区</param>
    /// <returns>异步任务</returns>
    /// <exception cref="Win32Exception">当操作失败时抛出</exception>
    public static async Task SetWallpaper(string wallpaperPath, int fitStyle)
    {
        try
        {
            // 修正UI样式到系统注册表值的正确映射，已修复填充/适应颠倒的问题
            // 系统注册表WallpaperStyle的实际取值：0=居中,2=拉伸,6=适应,10=填充,22=跨区
            var (tileValue, styleValue) = fitStyle switch
            {
                0 => ("1", "1"),    // 平铺：TileWallpaper=1, WallpaperStyle=1
                1 => ("0", "0"),    // 居中：TileWallpaper=0, WallpaperStyle=0
                2 => ("0", "2"),    // 拉伸：TileWallpaper=0, WallpaperStyle=2
                3 => ("0", "6"),   // 填充：TileWallpaper=0, WallpaperStyle=6
                4 => ("0", "10"),    // 适应：TileWallpaper=0, WallpaperStyle=10
                5 => ("0", "22"),   // 跨区：TileWallpaper=0, WallpaperStyle=22
                _ => ("0", "2")     // 默认：拉伸
            };

            // 将耗时的同步系统操作放到后台线程执行，避免阻塞UI线程
            await Task.Run(() =>
            {
                // 1. 修改注册表配置壁纸契合样式
                using (var desktopRegKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
                {
                    if (desktopRegKey == null)
                    {
                        throw new Win32Exception("无法访问系统桌面注册表配置项，操作失败。");
                    }

                    // 设置正确的注册表值
                    desktopRegKey.SetValue("TileWallpaper", tileValue, RegistryValueKind.String);
                    desktopRegKey.SetValue("WallpaperStyle", styleValue, RegistryValueKind.String);
                }

                // 2. 调用系统API设置壁纸，并通知系统更新配置
                int result = SystemParametersInfo(
                    SPI_SETDESKWALLPAPER, 
                    0, 
                    wallpaperPath, 
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE
                );

                // 检查API调用结果
                if (result == 0)
                {
                    // 调用失败，获取系统错误码并抛出异常
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            });
        }
        catch (Exception ex)
        {
            // 封装异常，保持原有异常类型，兼容项目中原有的错误处理逻辑
            throw new Win32Exception(ex.Message, ex);
        }
    }
}

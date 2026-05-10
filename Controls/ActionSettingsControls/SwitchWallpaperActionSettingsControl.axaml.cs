using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Controls;
using FluentAvalonia.UI.Controls;
using SwitchWallpaperSettings.Models;
namespace SwitchWallpaperSettings.Controls.ActionSettingsControls;
public partial class SwitchWallpaperActionSettingsControl : ActionSettingsControlBase<WallpaperSettings>
{
    public SwitchWallpaperActionSettingsControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 选择文件按钮点击事件
    /// 使用Avalonia推荐的新StorageProvider API，消除过时警告
    /// 兼容Avalonia 0.10+所有版本，适配ClassIsland SDK
    /// </summary>
    public async void OnSelectFileClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            // 获取当前窗口的存储服务提供者，使用官方推荐的非过时API
            var topLevel = TopLevel.GetTopLevel(this);
            // 空检查，消除可空引用警告
            if (topLevel == null)
            {
                var dialog = new TaskDialog()
                {
                    Title = "选择文件失败",
                    Content = "无法获取窗口信息，无法打开文件选择器",
                    XamlRoot = AppBase.Current.GetRootWindow(),
                    Buttons = { new TaskDialogButton("确定", true) }
                };
                await dialog.ShowAsync();
                return;
            }
            var storageProvider = topLevel.StorageProvider;

            // 配置文件选择选项
            // 适配Avalonia 0.10版本的FilePickerFileType API
            var options = new FilePickerOpenOptions
            {
                Title = "选择壁纸文件",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("图片文件")
                    {
                        Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.tiff", "*.webp" }
                    },
                    new FilePickerFileType("所有文件")
                    {
                        Patterns = new[] { "*.*" }
                    }
                }
            };

            // 显示对话框并等待结果
            var result = await storageProvider.OpenFilePickerAsync(options);
            
            // 如果用户选择了文件，自动填充路径
            if (result != null && result.Count > 0)
            {
                Settings.WallpaperPath = result[0].Path.AbsolutePath;
            }
        }
        catch (Exception ex)
        {
            // 错误处理，避免对话框异常导致崩溃
            var dialog = new TaskDialog()
            {
                Title = "选择文件失败",
                Content = $"无法打开文件选择器：{ex.Message}",
                XamlRoot = AppBase.Current.GetRootWindow(),
                Buttons = { new TaskDialogButton("确定", true) }
            };
            await dialog.ShowAsync();
        }
    }
}
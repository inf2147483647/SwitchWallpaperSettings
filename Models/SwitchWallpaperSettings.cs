using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace SwitchWallpaperSettings.Models;
public class WallpaperSettings : INotifyPropertyChanged
{
    private string _wallpaperPath = "";
    /// <summary>
    /// 壁纸文件的完整路径
    /// </summary>
    public string WallpaperPath
    {
        get => _wallpaperPath;
        set
        {
            _wallpaperPath = value;
            OnPropertyChanged();
        }
    }
    
    private int _fitStyle = 3;
    /// <summary>
    /// 壁纸契合度样式
    /// 0: 平铺, 1: 居中, 2: 拉伸, 3: 填充, 4: 适应, 5: 跨区
    /// </summary>
    public int FitStyle
    {
        get => _fitStyle;
        set
        {
            _fitStyle = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using Helper.Wpf.Embed;
using Helper.Wpf.Image;
using Helper.Wpf.Text;

namespace Helper.Wpf.Main;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private TextViewModel _tvm;
    [ObservableProperty] private ImageViewModel _ivm;
    [ObservableProperty] private EmbedViewModel _evm;

    public MainWindowViewModel(TextViewModel tvm, ImageViewModel ivm, EmbedViewModel evm)
    {
        _tvm = tvm;
        _ivm = ivm;
        _evm = evm;
    }
}
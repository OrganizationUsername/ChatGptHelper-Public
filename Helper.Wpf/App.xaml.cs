using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Helper.ServiceGateways;
using Helper.Wpf.Embed;
using Helper.Wpf.General;
using Helper.Wpf.Image;
using Helper.Wpf.Text;
using Helper.Wpf.Main;

namespace Helper.Wpf;

public partial class App
{
    private ServiceProvider Services { get; }

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<DbStuff>();
        services.AddSingleton<GptContext>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IHttpService, HttpService>();
        services.AddSingleton<IImageSaver, ImageSaver>();
        services.AddSingleton<IFileIo, FileIo>();
        services.AddSingleton<TextViewModel>();
        services.AddSingleton<ImageViewModel>();
        services.AddSingleton<EmbedViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindowView>();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var wnd = Services.GetService<MainWindowView>();
        try { wnd?.Show(); }
        catch (System.Exception) { wnd!.Show(); }
    }
}
using Engine.Application.Commanding;
using Engine.Infrastructure.Config;
using Engine.Infrastructure.Contracts;
using Engine.Infrastructure.CPU;
using Engine.Infrastructure.Decoders;
using Engine.Infrastructure.Resolvers;
using Engine.RenderGraph;
using Engine.RenderGraph.Abstractions;
using Engine.RenderGraph.Contracts;
using Engine.RenderGraph.Effects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EESample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public static Window? Window { get; private set; }
        public IHost HostApp { get; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddSingleton<EffectRenderNodeFactoryRegistry>();
            builder.Services.AddScoped<RenderGraphBuilder>();
            builder.Services.AddSingleton<IEffectRenderNodeFactory, BrightnessRenderNodeFactory>();

            builder.Services.AddSingleton<IRenderBackend, CpuRenderBackend>();
            builder.Services.AddSingleton<IRenderNodeProcessor, CpuAssetNodeProcessor>();
            builder.Services.AddSingleton<IRenderNodeProcessor, CpuBrightnessNodeProcessor>();
            builder.Services.AddSingleton<IRenderNodeProcessor, CpuCompositeNodeProcessor>();
            builder.Services.AddSingleton<IRenderNodeProcessor, CpuTransformNodeProcessor>();

            builder.Services.AddSingleton<CommandDispatcher>();
            builder.Services.AddSingleton<IRenderCache, InMemoryRenderCache>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<RenderGraphExecutor>();
            builder.Services.AddSingleton<IAssetResolver, FileAssetResolver>();
            builder.Services.AddSingleton<IImageDecoder, ImageDecoder>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
            });

            builder.Services.AddEditorEngine();
            HostApp = builder.Build();

            ServiceLocator.Init(HostApp.Services);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            Window = _window;
            _window.Activate();
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Engine.Abstractions;
using Engine.Application.Commanding;
using Engine.Application.Commands;
using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.Infrastructure.CPU;
using Engine.RenderGraph;
using Engine.RenderGraph.Abstractions;
using Engine.Runtime;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.Storage.Pickers;
using WinRT.Interop;

namespace EESample;

internal partial class MainPageViewModel : ObservableObject
{
    private readonly RenderGraphBuilder _renderGraphBuilder;
    private readonly CommandDispatcher _commandDispatcher;
    private readonly RenderGraphExecutor _renderGraphExecutor;
    private readonly IRenderCache _renderCache;

#pragma warning disable MVVMTK0045
    [ObservableProperty]
    private double _brightness;
#pragma warning restore MVVMTK0045

    private ImageSource? _source;
    public ImageSource? Source
    {
        get { return _source; }
        set => SetProperty(ref _source, value);
    }
    private string? _renderStatusText = "Idle";
    public string? RenderStatusText
    {
        get => _renderStatusText;
        set => SetProperty(ref _renderStatusText, value);
    }
    private string? _nodeCount = "Nodes: 0";
    public string? NodeCount
    {
        get => _nodeCount;
        set => SetProperty(ref _nodeCount, value);
    }

    private InfoBarSeverity _severity = InfoBarSeverity.Informational;
    public InfoBarSeverity Severity
    {
        get => _severity;
        set => SetProperty(ref _severity, value);
    }

    private string? _statusBarMessage = "Aguardando renderização...";
    public string? StatusBarMessage
    {
        get => _statusBarMessage;
        set => SetProperty(ref _statusBarMessage, value);
    }


    private EditorSession? _session;

    public MainPageViewModel(RenderGraphBuilder renderGraphBuilder,
                             CommandDispatcher commandDispatcher,
                             RenderGraphExecutor renderGraphExecutor,
                             IRenderCache renderCache)
    {
        _renderGraphBuilder = renderGraphBuilder;
        _commandDispatcher = commandDispatcher;
        _renderGraphExecutor = renderGraphExecutor;
        _renderCache = renderCache;
    }
    [RelayCommand]
    public async Task Teste(CancellationToken cancellationToken)
    {
        try
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(App.Window);

            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var picker = new FileOpenPicker(windowId);


            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");

            var pickerResult = await picker.PickSingleFileAsync();

            if (pickerResult is null)
            {
                return;
            }

            var document = new EditorDocument(DocumentId.New(), new CanvasSize(999, 699));
            _session = new EditorSession(document, _commandDispatcher, _renderGraphBuilder, _renderGraphExecutor, _renderCache);
            _session.PreviewUpdated += Session_PreviewUpdated;

            var cmd = new AddImageLayerCommand("image", new AssetReference(AssetId.New(), pickerResult.Path));
            var context = new CommandContext(document);
            await _session.DispatchAndRenderAsync(cmd);
            UpdateDebugInfo();
        }
        catch (Exception ex)
        {
            Severity = InfoBarSeverity.Error;
            StatusBarMessage = ex.Message;
        }
    }

    private async void Session_PreviewUpdated(object? sender, PreviewUpdated e)
    {
        if (e.Preview.Surface is not CpuRenderSurface surface)
            return;

        StatusBarMessage = "Finalizado";
        await RenderToPreviewAsync(surface);
        UpdateDebugInfo();
    }

    private void UpdateDebugInfo()
    {
        if (_session is null)
        {
            return;
        }

        RenderStatusText = _session.RenderState.Status.ToString();

        NodeCount = $"Nodes: {_session.CurrentGraph.Nodes.Count}";
    }
    partial void OnBrightnessChanged(double value)
    {
        var selectedNodeId = _session?.Document.Selection.ActiveNodeId;
        if (selectedNodeId is null)
            return;

        var cmd = new AddOrChangeBrightnessCommand(selectedNodeId.Value, (float)value);
        _session?.DispatchAndRenderAsync(cmd);
    }
    [RelayCommand]
    public async Task RotateAsync(string degress, CancellationToken cancellationToken)
    {
        if (_session is null)
            return;

        var selectedNodeId = _session.Document.Selection.ActiveNodeId;
        if (selectedNodeId is null)
            return;

        var cmd = new RotateLayerCommand(selectedNodeId.Value, double.Parse(degress));
        await _session.DispatchAndRenderAsync(cmd);
    }

    private async Task RenderToPreviewAsync(IRenderSurface surface)
    {
        if (surface is CpuRenderSurface cpuSurface)
        {
            using var bitmap = cpuSurface.ToSoftwareBitmap();
            var bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(bitmap);
            if (Source is IDisposable disposable)
                disposable.Dispose();

            Source = null;
            Source = bitmapSource;
        }
    }
}

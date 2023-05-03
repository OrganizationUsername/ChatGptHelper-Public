using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helper.ServiceGateways;
using Helper.ServiceGateways.Models;
using Helper.Web;
using Helper.Wpf.General;

namespace Helper.Wpf.Image;

public partial class ImageViewModel : ObservableObject
{
    private readonly DbStuff _sg;
    private readonly IDialogService _dialogService;
    private readonly IHttpService _httpService;
    private readonly IImageSaver _imageSaver;
    [ObservableProperty] private string _currentQuery = "";

    public int ImageCount
    {
        get => _imageCount;
        set => SetProperty(ref _imageCount, Math.Max(Math.Min(10, value), 1));
    }

    private string? _apiKey;
    private int _imageCount = 1;

    public ObservableCollection<ImageSearchThing> ImageSearchThings { get; set; } = new();
    [ObservableProperty] private ImageSearchThing? _selectedImageSearchThing;

    public ImageViewModel(DbStuff sg, IDialogService dialogService, IHttpService httpService, IImageSaver imageSaver)
    {
        _sg = sg;
        _dialogService = dialogService;
        _httpService = httpService;
        _imageSaver = imageSaver;
    }

    private string GetApiKey()
    {
        if (_sg.HasApiKey()) return _sg.GetApiKey();
        var apiKey = _dialogService.GetApiKey();
        if (apiKey is null)
        {
            System.Windows.Application.Current.Shutdown();
            return "";
        }

        _sg.SetApiKey(apiKey);
        return apiKey;
    }

    [RelayCommand]
    private async void UserControlLoaded()
    {
        _apiKey ??= GetApiKey();

        if (ImageSearchThings.Any()) { return; }

        var imageQueries = await _sg.GetImageQueries();
        foreach (var iq in imageQueries) { ImageSearchThings.Add(ImageSearchThing.Get(iq)); }
    }

    [RelayCommand]
    private async void DownloadImages()
    {
        if (SelectedImageSearchThing is null) { return; }

        var directory = _dialogService.ShowFolderBrowserDialog();
        if (directory is null) { return; }

        await _imageSaver.SaveImagesAsync(SelectedImageSearchThing, directory);
    }

    private bool _blockQuerying;

    [RelayCommand]
    private async void Query()
    {
        if (_blockQuerying)
        {
            _dialogService.ShowMessageBox("Please wait until the previous query is finished.");
            return;
        }

        _blockQuerying = true;
        if (_apiKey == null) { return; }

        var imageResult = await ChatGptCaller.ImageQuery(_apiKey, CurrentQuery, ImageCount);
        if (imageResult is null)
        {
            _dialogService.ShowMessageBox("An error occured!");
            _blockQuerying = false;
            return;
        }

        var imageQuery = new ImageQuery() { ImageQueryText = CurrentQuery };
        var images = await DownloadImagesAsync(imageResult.data.Select(d => d.url));
        if (!images.Any())
        {
            _dialogService.ShowMessageBox("An error occured!");
            _blockQuerying = false;
            return;
        }

        foreach (var i in images) { imageQuery.ImageResults.Add(new() { ImageQuery = imageQuery, ImageBlob = i }); }

        var iq = await _sg.StoreStuff(imageQuery);
        ImageSearchThings.Add(ImageSearchThing.Get(iq));

        _blockQuerying = false;
        CurrentQuery = "";
    }

    private async Task<List<byte[]>> DownloadImagesAsync(IEnumerable<string> urls)
    {
        var images = new List<byte[]>();
        foreach (var url in urls)
        {
            try { images.Add(await _httpService.GetByteArrayAsync(url)); }
            catch (Exception ex) { _dialogService.ShowMessageBox($"Error downloading image from {url}: {ex.Message}"); }
        }

        return images;
    }
}
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helper.ServiceGateways;
using Helper.ServiceGateways.Models;
using Helper.Web;
using Helper.Wpf.General;

namespace Helper.Wpf.Embed;

public partial class EmbedViewModel : ObservableObject
{
    private readonly DbStuff _sg;
    private readonly IDialogService _dialogService;
    private readonly IFileIo _fileIo;
    [ObservableProperty] private string _currentQuery = "";
    private string? _apiKey;

    public ObservableCollection<EmbedPieceViewModel> EmbedPieceViewModels { get; set; } = new();
    [ObservableProperty] private EmbedPieceViewModel? _selectedEmbedPieceViewModel;

    public EmbedViewModel(DbStuff sg, IDialogService dialogService, IFileIo fileIo)
    {
        _sg = sg;
        _dialogService = dialogService;
        _fileIo = fileIo;
    }

    [RelayCommand]
    private async void UserControlLoaded()
    {
        _apiKey ??= GetApiKey();

        if (EmbedPieceViewModels.Any()) { return; }

        var eps = await _sg.GetSomeEmbeds();
        foreach (var ep in eps) { EmbedPieceViewModels.Add(EmbedPieceViewModel.Get(ep)); }

        SelectedEmbedPieceViewModel = EmbedPieceViewModels.LastOrDefault();
    }

    [RelayCommand]
    private void CalculateEmbedScores()
    {
        if (SelectedEmbedPieceViewModel is null)
        {
            _dialogService.ShowMessageBox("Need more EmbedPieces! or Select One!");
            return;
        }

        var e = SelectedEmbedPieceViewModel;

        if (_sg.GetEmbedCount() > 10)
        {
            var eps = _sg.GetBestEmbeds(e.Vector);
            EmbedPieceViewModels.Clear();
            foreach (var ep in eps) { EmbedPieceViewModels.Add(EmbedPieceViewModel.Get(ep)); }
        }

        foreach (var ep in EmbedPieceViewModels) { ep.TempScore = EmbedPieceViewModel.DotProduct(e.Vector, ep.Vector); }
    }

    private bool _blockQuerying;

    [RelayCommand]
    private async void GetTokenCount()
    {
        if (_blockQuerying)
        {
            _dialogService.ShowMessageBox("Please wait until the previous query is finished.");
            return;
        }

        if (CurrentQuery.Length < 10)
        {
            _dialogService.ShowMessageBox("Provide a meaningful query.");
            return;
        }

        _blockQuerying = true;
        if (_apiKey == null) { return; }

        var embed = await ChatGptCaller.GetEmbed(_apiKey, CurrentQuery);
        if (embed is null)
        {
            _dialogService.ShowMessageBox("Error getting embedding.");
            _blockQuerying = false;
            CurrentQuery = "";
            return;
        }

        var embedResult = new EmbedResult() { Text = CurrentQuery, Vector = _fileIo.SerializeVector(embed.embedding) };
        await _sg.StoreEmbed(embedResult);
        EmbedPieceViewModels.Add(new() { Text = CurrentQuery, Vector = embed.embedding.ToArray() });

        SelectedEmbedPieceViewModel = EmbedPieceViewModels.Last();
        _blockQuerying = false;
        CurrentQuery = "";
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
}
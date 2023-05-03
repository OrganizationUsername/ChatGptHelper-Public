using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helper.ServiceGateways;
using Helper.ServiceGateways.Models;
using Helper.Web;
using Helper.Wpf.General;

namespace Helper.Wpf.Text;

public partial class TextViewModel : ObservableObject
{
    private readonly DbStuff _sg;
    private readonly IDialogService _dialogService;
#pragma warning disable CS0414
    [ObservableProperty] private string _currentQuery = "";
#pragma warning restore CS0414
#pragma warning disable CS0169
    [ObservableProperty] private int _currentQueryTokenCount;
#pragma warning restore CS0169
    [ObservableProperty] private int _currentResponseTokenCount;
    [ObservableProperty] private string _currentResponse = "";
    [ObservableProperty] private bool _useGoodModel;
    [ObservableProperty] private bool _isDeterministic = true;
    private string? _apiKey;

    public ObservableCollection<QuestionAnswer> QuestionAnswers { get; set; } = new();

    public TextViewModel(DbStuff sg, IDialogService dialogService)
    {
        _sg = sg;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async void UserControlLoaded()
    {
        _apiKey ??= GetApiKey();

        if (QuestionAnswers.Any()) { return; }

        var stuffs = await _sg.GetQueries();
        foreach (var q in stuffs) { QuestionAnswers.Add(QuestionAnswer.Get(q)); }
    }

    [RelayCommand]
    private async void GetTokenCount()
    {
        if (_apiKey != null) CurrentQueryTokenCount = await ChatGptCaller.GetEmbedCost(_apiKey, CurrentQuery);
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

        CurrentResponse = "";
        _blockQuerying = true;
        var modelToUse = UseGoodModel ? "text-davinci-003" : "text-curie-001";
        if (_apiKey == null) return;
        var response = await ChatGptCaller.Query(_apiKey, CurrentQuery, modelToUse, IsDeterministic);
        if (response is null || response.error is not null)
        {
            _dialogService.ShowMessageBox($"an Error occurred!{response?.error?.message ?? ""}");
            _blockQuerying = false;
        }
        else
        {
            CurrentResponseTokenCount = response.usage.completion_tokens;
            CurrentQueryTokenCount = response.usage.prompt_tokens;
            CurrentResponse = string.Join(Environment.NewLine, response.choices.Select(c => c.text)).Trim();
            var newQ = await _sg.StoreQuery(CurrentQuery, CurrentQueryTokenCount, response, modelToUse);
            QuestionAnswers.Add(QuestionAnswer.Get(newQ));
            _blockQuerying = false;
        }
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
    private void Stuff() => _dialogService.ShowMessageBox($"Hi! {_sg.GptContext.Answers.ToList().Count}");
}

public class QuestionAnswer
{
    public string Prompt { get; set; } = null!;
    public int PromptTokens { get; set; }
    public string ModelUsed { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public int AnswerTokens { get; set; }

    public static QuestionAnswer Get(Query q)
    {
        return new()
        {
            Prompt = q.Text,
            PromptTokens = q.TokenCount,
            Answer = string.Join(Environment.NewLine, q.Response.Answers.Select(a => a.Text)),
            AnswerTokens = q.Response.TokenCount,
            ModelUsed = q.Response.ModelUsed
        };
    }
}
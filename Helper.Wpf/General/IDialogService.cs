namespace Helper.Wpf.General;

public interface IDialogService
{
    string? ShowFolderBrowserDialog();
    void ShowMessageBox(string message);
    string? GetApiKey();
}
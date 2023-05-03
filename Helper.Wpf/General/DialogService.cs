using System.Windows.Forms;
using Ookii.Dialogs.WinForms;

namespace Helper.Wpf.General;

internal class DialogService : IDialogService
{
    public string? ShowFolderBrowserDialog()
    {
        var folderBrowserDialog = new VistaFolderBrowserDialog
        {
            Description = "Select a directory",
            UseDescriptionForTitle = true
        };
        var dialogResult = folderBrowserDialog.ShowDialog();
        return dialogResult == DialogResult.OK ? folderBrowserDialog.SelectedPath : null;
    }

    public void ShowMessageBox(string message) => MessageBox.Show(message);

    public string? GetApiKey()
    {
        var window = new InputDialog() { Content = "Provide API string" };
        var result = window.ShowDialog();
        return result == DialogResult.OK ? window.Input : null;
    }
}
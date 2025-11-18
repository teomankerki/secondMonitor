using System.Collections.ObjectModel;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class NotesViewModel : ObservableObject
{
    private readonly RelayCommand _addNoteCommand;
    private readonly RelayCommand _clearNotesCommand;
    private readonly RelayCommand _deleteNoteCommand;
    private string _draft = string.Empty;

    public NotesViewModel()
    {
        Notes = new ObservableCollection<string>();
        _addNoteCommand = new RelayCommand(_ => AddNote());
        _clearNotesCommand = new RelayCommand(_ => Notes.Clear());
        _deleteNoteCommand = new RelayCommand(note =>
        {
            if (note is string text && Notes.Contains(text))
            {
                Notes.Remove(text);
            }
        });
    }

    public ObservableCollection<string> Notes { get; }

    public string Draft
    {
        get => _draft;
        set => SetProperty(ref _draft, value);
    }

    public RelayCommand AddNoteCommand => _addNoteCommand;
    public RelayCommand ClearNotesCommand => _clearNotesCommand;
    public RelayCommand DeleteNoteCommand => _deleteNoteCommand;

    private void AddNote()
    {
        var text = Draft?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        Notes.Insert(0, text);
        Draft = string.Empty;
    }
}

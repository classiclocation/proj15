const string FileName = "notes.txt";
var manager = new NoteManager();

try
{
    manager.LoadFromFile(FileName);
}
catch (InvalidDataException error)
{
    Console.WriteLine($"Error: {error.Message}");
}

while (true)
{
    Console.WriteLine("\n1 - list, 2 - add, 3 - edit, 4 - delete, 0 - exit");
    Console.Write("Choose: ");

    switch (Console.ReadLine())
    {
        case "1":
            ShowNotes(manager);
            break;
        case "2":
            AddNote(manager);
            break;
        case "3":
            EditNote(manager);
            break;
        case "4":
            DeleteNote(manager);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Unknown command.");
            break;
    }
}

static void ShowNotes(NoteManager manager)
{
    if (manager.Notes.Count == 0)
    {
        Console.WriteLine("No notes.");
        return;
    }

    for (var i = 0; i < manager.Notes.Count; i++)
    {
        var note = manager.Notes[i];
        Console.WriteLine($"{i + 1}. {note.Title} ({note.CreatedAt:dd.MM.yyyy HH:mm})");
        Console.WriteLine($"   {note.Text}");
    }
}

static void AddNote(NoteManager manager)
{
    Console.Write("Title: ");
    var title = Console.ReadLine() ?? string.Empty;
    Console.Write("Text: ");
    var text = Console.ReadLine() ?? string.Empty;

    try
    {
        manager.AddNote(title, text);
        manager.SaveToFile(FileName);
        Console.WriteLine("Note added.");
    }
    catch (ArgumentException error)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}

static void EditNote(NoteManager manager)
{
    if (!TryReadIndex(manager, out var index)) return;

    Console.Write("New title: ");
    var title = Console.ReadLine() ?? string.Empty;
    Console.Write("New text: ");
    var text = Console.ReadLine() ?? string.Empty;

    try
    {
        manager.EditNote(index, title, text);
        manager.SaveToFile(FileName);
        Console.WriteLine("Note updated.");
    }
    catch (ArgumentException error)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}

static void DeleteNote(NoteManager manager)
{
    if (!TryReadIndex(manager, out var index)) return;

    manager.DeleteNote(index);
    manager.SaveToFile(FileName);
    Console.WriteLine("Note deleted.");
}

static bool TryReadIndex(NoteManager manager, out int index)
{
    Console.Write("Note number: ");
    if (!int.TryParse(Console.ReadLine(), out var number) || number < 1 || number > manager.Notes.Count)
    {
        Console.WriteLine("Note not found.");
        index = -1;
        return false;
    }

    index = number - 1;
    return true;
}

class Note
{
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

    public Note(string title, string text)
    {
        Title = title;
        Text = text;
        CreatedAt = DateTime.Now;
    }
}

class NoteManager
{
    public List<Note> Notes { get; private set; } = [];

    public void AddNote(string title, string text)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is empty.");
        }

        Notes.Add(new Note(title, text));
    }

    public void EditNote(int index, string title, string text)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is empty.");
        }

        Notes[index].Title = title;
        Notes[index].Text = text;
    }

    public void DeleteNote(int index)
    {
        Notes.RemoveAt(index);
    }

    public void SaveToFile(string fileName)
    {
        using var writer = new StreamWriter(fileName);

        foreach (var note in Notes)
        {
            writer.WriteLine(note.Title);
            writer.WriteLine(note.Text);
            writer.WriteLine(note.CreatedAt.ToString("O"));
            writer.WriteLine("---");
        }
    }

    public void LoadFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return;
        }

        var lines = File.ReadAllLines(fileName);

        for (var i = 0; i + 3 < lines.Length; i += 4)
        {
            var note = new Note(lines[i], lines[i + 1]);
            note.CreatedAt = DateTime.Parse(lines[i + 2]);
            Notes.Add(note);
        }
    }
}
using System.Diagnostics;
using System.IO;
using System;
using Godot;

public enum PromptType
{
    Babble,
    ThrowLeft,
    ThrowRight,
    ThrowOppositeSide
}

public partial class TTSSystem : Node
{
    [Export]
    public string InstallationPath { get; set; } = "../tts/";

    [Export]
    public string PythonExecutable { get; set; } = "./venv/Scripts/python.exe";

    [Export]
    public string Script { get; set; } = "main.py";

    [Export]
    public string OutputAudioFile { get; set; } = "output.mp3";

    private byte[]? Execute(PromptType type)
    {
        // Record the start time before generating the audio to check if the audio file has been generated
        var start = DateTime.Now;

        var prompt_argument = type switch
        {
            PromptType.Babble => "babble",
            PromptType.ThrowLeft => "throw_left",
            PromptType.ThrowRight => "throw_right",
            PromptType.ThrowOppositeSide => "throw_opposite_side",
            _ => throw new ArgumentException("Invalid prompt type")
        };

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = PythonExecutable,
                Arguments = string.Join(" ", Script, prompt_argument),
                WorkingDirectory = InstallationPath,
                UseShellExecute = true,
                CreateNoWindow = true
            }
        };

        // Wait for the process to generate the audio
        process.Start();
        process.WaitForExit();
        process.Close();

        // Make sure the process exited with correct exit code
        if (process.ExitCode != 0)
        {
            return null;
        }

        // Check if the output file has been modified since the process started
        var audio_path = Path.Combine(InstallationPath, OutputAudioFile);

        if (!File.Exists(audio_path) || File.GetLastWriteTime(audio_path) <= start)
        {
            return null;
        }

        try
        {
            return File.ReadAllBytes(audio_path);
        }
        catch
        {
            return null;
        }
    }

    public AudioStream? GenerateAudio(PromptType type)
    {
        var audio_data = Execute(type);

        if (audio_data == null)
        {
            return null;
        }

        return new AudioStreamMP3 { Data = audio_data };
    }
}
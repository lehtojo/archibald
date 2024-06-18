using Godot;

public partial class ArchibaldSpeech : AudioStreamPlayer3D
{
    [Export]
    public TTSSystem? TTS { get; set; }

    public bool IsSpeaking => Playing;

    public void OnFinishedSpeaking()
    {
        GD.Print("Finished speaking");
    }

    public void Speak(AudioStream audio)
    {
        GD.Print("Speaking...");
        Stream = audio;
        Play();
    }

    public void SpeakAsync(PromptType type)
    {
        if (TTS == null)
        {
            GD.PushError("TTS is not set");
            return;
        }

        GD.Print("Generating audio...");
        var audio = TTS.GenerateAudio(type);

        if (audio == null)
        {
            GD.PushError("Failed to generate speech");
            return;
        }

        CallDeferred(MethodName.Speak, audio);
    }

    public void Speak(PromptType type)
    {
        WorkerThreadPool.AddTask(Callable.From(() => SpeakAsync(type)));
    }

    public void Babble()
    {
        // If we're speaking already, do nothing
        if (IsSpeaking)
        {
            return;
        }

        Speak(PromptType.Babble);
    }

    public override void _Ready()
    {
        Finished += OnFinishedSpeaking;
    }
}
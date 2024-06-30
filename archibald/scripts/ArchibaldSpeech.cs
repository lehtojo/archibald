using System;
using Godot;

public partial class ArchibaldSpeech : AudioStreamPlayer3D
{
    [Export]
    public TTSSystem? TTS { get; set; }

    [Export]
    public Player? Player { get; set; }

    [Export]
    public FishingSystem? FishingSystem { get; set; }

    public PromptType LastPrompt { get; private set; } = PromptType.Babble;
    public bool IsSpeaking { get; private set; } = false;

    public void Advice(int sector, int correct, int sectors)
    {
        // If the float is in the correct sector, do nothing for now and start babbling later
        if (sector == correct)
        {
            return;
        }

        // We use terms "left" and "right" here that correspond to "counter-clockwise" and "clockwise" respectively.
        // Now, we'll find out which way the player should turn the rod to align the float with the correct sector.
        const float throw_opposite_side_min_degrees = 100.0f;

        var direct_distance = Math.Abs(sector - correct);
        var circular_distance = sectors - direct_distance;
        var min_distance = Math.Min(direct_distance, circular_distance);
        var degrees_per_sector = 360.0f / sectors;

        // If the difference is large enough in degrees, recommend throwing the float to the opposite side.
        if (min_distance * degrees_per_sector >= throw_opposite_side_min_degrees)
        {
            Speak(PromptType.ThrowOppositeSide);
            return;
        }

        var type = direct_distance <= circular_distance ? 
            (sector > correct ? PromptType.ThrowLeft : PromptType.ThrowRight) : 
            (sector > correct ? PromptType.ThrowRight : PromptType.ThrowLeft);

        Speak(type);
    }

    public void OnFinishedSpeaking()
    {
        GD.Print("Finished speaking");
        IsSpeaking = false;

        if (Player == null)
        {
            GD.PrintErr("Player is not set");
            return;
        }

        if (FishingSystem == null)
        {
            GD.PrintErr("Fishing system is not set");
            return;
        }

        // If the player hasn't thrown the float yet, we can't advice them
        var sector = Player.FloatSector;

        if (sector == null)
        {
            GD.Print("Player has not thrown the float yet, not advicing");
            return;
        }

        // If we babbled for a while, advice the player
        if (LastPrompt == PromptType.Babble)
        {
            Advice(sector.Value, FishingSystem.CorrectSector, FishingSystem.SectorCount);
        }
    }

    public void Speak(AudioStream audio)
    {
        GD.Print("Speaking...");
        Stream = audio;
        Play();
    }

    public void SpeakAsync(PromptType type)
    {
        IsSpeaking = true;

        if (TTS == null)
        {
            GD.PushError("TTS is not set");
            return;
        }

        GD.Print("Generating audio...");
        LastPrompt = type;
        var audio = TTS.GenerateAudio(type);

        if (audio == null)
        {
            GD.PushError("Failed to generate speech");
            IsSpeaking = false;
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
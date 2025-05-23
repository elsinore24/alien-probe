using UnityEngine;

public enum Speaker { Xylar, Zorp, Narrator, HumanInternalMonologue }

[CreateAssetMenu(fileName = "NewDialogueLine", menuName = "Alien Probe/Dialogue Line Data")]
public class DialogueLineData : ScriptableObject
{
    public string dialogueKey; // Unique key, e.g., "ZORP_PUZZLE1_WRONG"
    public Speaker characterSpeaking;
    [TextArea(3, 10)]
    public string dialogueText;
    public AudioClip audioClip; // The voice-over audio
    public string live2DAnimationTrigger; // Trigger for Live2D (e.g., "Surprise", "Ponder")
    // Optional: public float duration; // If you want to manually specify line duration
}
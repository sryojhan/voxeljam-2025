using UnityEngine;


[CreateAssetMenu(fileName = "New dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [System.Serializable]
    public class SpeechBubble
    {
        public bool isSpeaker1 = false;
        public Speaker speaker;

        [TextArea]
        public string text;
    }

    public SpeechBubble[] completeDialogue;
    public int sceneIndexToLoadWhenComplete = 0;

    public Sprite background;

    public Speaker firstSpeakerLeft;
    public Speaker firstSpeakerRight;

}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Dialogue defaultDialogueToLoad;

    public static Dialogue dialogueToLoad;

    public static DialogueManager instance;


    public float charactersPerSecond = 5;
    public Interpolation characterRevealInterpolation;

    public float timeBeforeCanSkipDialogue = 0.3f;

    public TextMeshProUGUI speaker1;
    public TextMeshProUGUI speaker1Outline;
    public TextMeshProUGUI speaker2;
    public TextMeshProUGUI speaker2Outline;

    public TextMeshProUGUI text;

    public CoroutineAnimation displayCharacter;

    public Image background;

    public Image speaker1Sprite;
    public Image speaker2Sprite;


    public float notTalkingOffset = 200;
    public Color notTalingColor = Color.gray;

    private Vector2 speakerLeftOrigin;
    private Vector2 speakerRightOrigin;

    private void Awake()
    {
        instance = this;

        Dialogue dialogue = dialogueToLoad;

        if (!dialogue)
        {
            dialogue = defaultDialogueToLoad;
        }

        background.sprite = dialogue.background;
        text.text = "";
        speaker1.text = dialogue.firstSpeakerLeft.name;
        speaker2.text = dialogue.firstSpeakerRight.name;
        speaker1Outline.text = dialogue.firstSpeakerLeft.name;
        speaker2Outline.text = dialogue.firstSpeakerRight.name;

        speaker1Sprite.sprite = dialogue.firstSpeakerLeft.defaultSprite;
        speaker2Sprite.sprite = dialogue.firstSpeakerRight.defaultSprite;

        speakerLeftOrigin = speaker1Sprite.rectTransform.anchoredPosition;
        speakerRightOrigin = speaker2Sprite.rectTransform.anchoredPosition;

        speaker1Sprite.color = notTalingColor;
        speaker2Sprite.color = notTalingColor;

        StartCoroutine(DisplayDialogue(dialogue));
    }



    bool spacebarPressed = false;

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            spacebarPressed = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            spacebarPressed = true;
        }
    }

    IEnumerator DisplayDialogue(Dialogue dialogue)
    {
        int currentSpeechIdx = 0;
        Dialogue.SpeechBubble speech = dialogue.completeDialogue[currentSpeechIdx];

        string previousSpeaker = "";

        while (speech != null)
        {

            if (previousSpeaker != speech.speaker.name)
            {
                //Reveal new speaker

                Vector2 offsetLeftSpeaker = speakerLeftOrigin + new Vector2(-notTalkingOffset, -50);


                Vector2 offsetRightSpeaker = speakerRightOrigin + new Vector2(notTalkingOffset, -50);


                void OnBegin()
                {
                    (speech.isSpeaker1 ? speaker1Sprite : speaker2Sprite).sprite = speech.speaker.defaultSprite;
                }

                void OnUpdate(float i)
                {
                    (speech.isSpeaker1 ? speaker1Sprite : speaker2Sprite).color = Color.Lerp(notTalingColor, Color.white, i);
                    (!speech.isSpeaker1 ? speaker1Sprite : speaker2Sprite).color = Color.Lerp(Color.white, notTalingColor, i);



                    speaker1Sprite.rectTransform.anchoredPosition =

                        speech.isSpeaker1 ?
                        Vector2.Lerp(offsetLeftSpeaker, speakerLeftOrigin, i) :
                        Vector2.Lerp(speakerLeftOrigin , offsetLeftSpeaker, i);


                    speaker2Sprite.rectTransform.anchoredPosition =

                        !speech.isSpeaker1 ?
                        Vector2.Lerp(offsetRightSpeaker, speakerRightOrigin, i) :
                        Vector2.Lerp(speakerRightOrigin, offsetRightSpeaker, i);

                }

                void OnEnd()
                {
                    (speech.isSpeaker1 ? speaker1 : speaker2).text = speech.speaker.name;
                    (speech.isSpeaker1 ? speaker1Outline : speaker2Outline).text = speech.speaker.name;

                    (speech.isSpeaker1 ? speaker1Sprite : speaker2Sprite).sprite = speech.speaker.defaultSprite;

                    (speech.isSpeaker1 ? speaker1Sprite : speaker2Sprite).color =Color.white;
                    (!speech.isSpeaker1 ? speaker1Sprite : speaker2Sprite).color =notTalingColor;

                    speaker1Sprite.rectTransform.anchoredPosition =
                    (speech.isSpeaker1 ? speakerLeftOrigin : offsetLeftSpeaker);


                    speaker2Sprite.rectTransform.anchoredPosition =
                    (!speech.isSpeaker1 ? speakerRightOrigin : offsetRightSpeaker);

                }


                displayCharacter.Play(this, OnUpdate, OnBegin, OnEnd);

            }

            yield return new WaitForSeconds(displayCharacter.duration);

            previousSpeaker = speech.speaker.name;


            float speechDuration = speech.text.Length / charactersPerSecond;

            for (float i = 0; i < 1; i += Time.deltaTime / speechDuration)
            {
                if (spacebarPressed)
                {
                    spacebarPressed = false;
                    break;
                }

                string currentText = speech.text.Substring(0, Mathf.FloorToInt(characterRevealInterpolation.LerpWithInterpolation(i, 0, speech.text.Length)));
                text.text = currentText;
                yield return null;
            }

            text.text = speech.text;

            yield return new WaitForSeconds(timeBeforeCanSkipDialogue);

            while (!spacebarPressed) yield return null;
            spacebarPressed = false;

            currentSpeechIdx++;

            if (currentSpeechIdx >= dialogue.completeDialogue.Length)
                speech = null;
            else
                speech = dialogue.completeDialogue[currentSpeechIdx];
        }


        SceneManager.LoadScene(dialogue.sceneIndexToLoadWhenComplete);

    }

}

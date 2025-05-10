using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurtainManager : Singleton<CurtainManager>
{

    public CoroutineAnimation curtainMovement;
    public Sound curtainSound;

    public float leftOpenedPosition = 0;
    private float leftClosedPosition = 0;
    public float rightOpenedPosition = 0;
    private float rightClosedPosition = 0;

    private void Start()
    {
        leftClosedPosition = leftCurtain.rectTransform.anchoredPosition.x;
        rightClosedPosition = rightCurtain.rectTransform.anchoredPosition.x;

        OpenCurtains();
    }


    public Image leftCurtain;
    public Image rightCurtain;


    [EasyButtons.Button]
    void OpenCurtains()
    {
        Vector2 leftInitialPosition = leftCurtain.rectTransform.anchoredPosition;
        Vector2 leftDestination = new Vector2(leftOpenedPosition, leftInitialPosition.y);

        Vector2 rightInitialPosition = rightCurtain.rectTransform.anchoredPosition;
        Vector2 rightDestination = new Vector2(rightOpenedPosition, leftInitialPosition.y);


        void Movement(float i)
        {
            leftCurtain.rectTransform.anchoredPosition =
                Vector2.Lerp(leftInitialPosition, leftDestination, i);
            rightCurtain.rectTransform.anchoredPosition =
                Vector2.Lerp(rightInitialPosition, rightDestination, i);
        }

        void End()
        {
            //Curtain completely opens
        }

        curtainMovement.Play(this, Movement, null, End);
        AudioManager.instance.PlaySound(curtainSound);

    }

    [EasyButtons.Button]
    public void CloseCurtains(string sceneToChange)
    {
        if (!curtainMovement.IsFinished()) return;

        Vector2 leftInitialPosition = leftCurtain.rectTransform.anchoredPosition;
        Vector2 leftDestination = new Vector2(leftClosedPosition, leftInitialPosition.y);


        Vector2 rightInitialPosition = rightCurtain.rectTransform.anchoredPosition;
        Vector2 rightDestination = new Vector2(rightClosedPosition, leftInitialPosition.y);


        void Movement(float i)
        {
            leftCurtain.rectTransform.anchoredPosition =
                Vector2.Lerp(leftInitialPosition, leftDestination, i);
            rightCurtain.rectTransform.anchoredPosition =
                Vector2.Lerp(rightInitialPosition, rightDestination, i);
        }

        void End()
        {
            SceneManager.LoadScene(sceneToChange);
        }

        curtainMovement.Play(this, Movement, null, End);

    }

}

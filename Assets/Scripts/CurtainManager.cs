using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurtainManager : Singleton<CurtainManager>
{

    public CoroutineAnimation curtainMovement;

    public float leftOpenedPosition = 0;
    public float rightOpenedPosition = 0;

    private void Start()
    {
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

    }

    [EasyButtons.Button]
    public void CloseCurtains(string sceneToChange)
    {
        Vector2 leftInitialPosition = leftCurtain.rectTransform.anchoredPosition;
        Vector2 leftDestination = new Vector2(0, leftInitialPosition.y);


        Vector2 rightInitialPosition = rightCurtain.rectTransform.anchoredPosition;
        Vector2 rightDestination = new Vector2(0, leftInitialPosition.y);


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

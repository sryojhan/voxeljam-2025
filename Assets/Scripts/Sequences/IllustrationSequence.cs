using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IllustrationSequence : MonoBehaviour
{
    public Morph[] illustrations;

    IEnumerator Start()
    {
        SceneTransition.SceneTransitionManager.instance.useCustomFunction = true;

        yield return new WaitForSeconds(1);

        foreach(Morph m in illustrations)
        {
            bool isMorphing = true;
            void WaitToEnd()
            {
                isMorphing = false;
            }

            m.onMorphEnd = WaitToEnd;
            m.BeginMorph();

            while (isMorphing) yield return null;

            void RemoveIllustration()
            {
                m.GetComponent<Image>().enabled = false;
            }

            SceneTransition.SceneTransitionManager.instance.customFunction = RemoveIllustration;
            SceneTransition.SceneTransitionManager.instance.ChangeScene("");
        }

        yield return new WaitForSeconds(1);

        SceneTransition.SceneTransitionManager.instance.useCustomFunction = false;

        CurtainManager.instance.CloseCurtains("Sandbox");
    }

}

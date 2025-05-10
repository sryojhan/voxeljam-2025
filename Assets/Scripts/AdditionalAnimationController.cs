using UnityEngine;

public class AdditionalAnimationController : MonoBehaviour
{
    public void OnRollEnd()
    {
        StyleMeter.instance.RollComplete();
    }
}

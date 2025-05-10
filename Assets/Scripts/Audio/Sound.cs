using UnityEngine;

[CreateAssetMenu(fileName = "New Sound", menuName = "Sound")]
public class Sound : ScriptableObject
{
    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 1;

    [Range(0, 3)]
    public float pitch = 1;

    [Range(-1, 1)]
    public float stereoPan;

    [Range(0, 1)]
    public float spatialBlend;
}

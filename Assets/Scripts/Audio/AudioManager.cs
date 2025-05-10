using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    protected override bool DestroyOnLoad => false;

    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound sound)
    {
        m_AudioSource.clip = sound.clip;    
        m_AudioSource.volume = sound.volume;
        m_AudioSource.pitch = sound.pitch;
        m_AudioSource.panStereo = sound.stereoPan;
        m_AudioSource.spatialBlend = sound.spatialBlend;

        m_AudioSource.Play();
    }


}

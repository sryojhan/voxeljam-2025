using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoroutineAnimation
{
    public delegate void OnValueUpdate(float value);
    public delegate void Callback();

    public float duration = 1;
    public float delay = 0;

    public Interpolation interpolation;

    private float progress = 0;
    private Coroutine coroutine = null;

    public virtual void Play(MonoBehaviour behaviour, OnValueUpdate update = null, Callback begin = null, Callback end = null)
    {
        if(coroutine != null)
        {
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = behaviour.StartCoroutine(Coroutine(update, begin, end));
    }

    public IEnumerator Coroutine(OnValueUpdate update, Callback begin, Callback onFinish)
    {
        yield return new WaitForSeconds(delay);
        
        begin?.Invoke();
        update?.Invoke(interpolation.Interpolate(0));

        progress = 0;
        float invDuration = 1.0f / duration;

        for(float i = 0; i < 1; i += invDuration * Time.deltaTime)
        {
            progress = i;
            update?.Invoke(interpolation.Interpolate(i));
            yield return null;
        }

        update?.Invoke(interpolation.Interpolate(1.0f));
        onFinish?.Invoke();

        progress = 1;
        coroutine = null;
    }

    public bool IsFinished()
    {
        return coroutine == null;
    }

    public float GetProgess()
    {
        return progress;
    }

    public void Stop(MonoBehaviour behaviour)
    {
        if(coroutine != null)
        {
            Debug.Log("Detenido el movimiento");

            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
    }






    public void Play(MonoBehaviour obj, RectTransform tr, Vector2 finalPosition, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null)
    {
        Vector2 initialPosition = tr.anchoredPosition;

        void OnUpdate(float i)
        {
            tr.anchoredPosition = Vector2.Lerp(initialPosition, finalPosition, i);

            onUpdate?.Invoke(i);
        }

        Play(obj, OnUpdate, onBegin, onEnd);
    }


    public void Play(MonoBehaviour obj, Transform tr, Vector3 finalPosition, OnValueUpdate onUpdate = null, Callback onBegin = null, Callback onEnd = null, bool local = true)
    {
        Vector3 initialPosition = local ? tr.localPosition : tr.position;

        void OnUpdate(float i)
        {
            if(local)
                tr.localPosition = Vector3.Lerp(initialPosition, finalPosition, i);
            else
                tr.position = Vector3.Lerp(initialPosition, finalPosition, i);

            onUpdate?.Invoke(i);
        }

        Play(obj, OnUpdate, onBegin, onEnd);
    }

}


using UnityEngine;
using UnityEngine.UI;

public class StyleMeter : Singleton<StyleMeter>
{

    public enum StyleAction
    {
        Jump, Airborne, Roll
    }

    [Header("Style score")]
    public float jumpScore = 10f;

    private float style = 0;

    const float maxStyle = 100f;
    public float positiveGain = 10f;
    public float negativeGain = 10f;

    public float styleGainMinMultiplier = 3f;
    public float styleGainMaxMultiplier = .3f;

    public float styleLoseMinMultiplier = .3f;
    public float styleLoseMaxMultiplier = 3f;


    public Slider styleMeter;

    private void Start()
    {
        styleMeter.minValue = -maxStyle;
        styleMeter.maxValue = maxStyle;

        styleMeter.value = 0;
    }

    public float MapStyleGainMultiplier()
    {
        return Map(style, -maxStyle, maxStyle, styleGainMinMultiplier, styleGainMaxMultiplier);
    }

    public float MapStyleLoseMultiplier()
    {
        return Map(style, -maxStyle, maxStyle, styleLoseMinMultiplier, styleLoseMaxMultiplier);
    }

    public float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
    }

    public void GainStyle(float value)
    {
        style += MapStyleGainMultiplier() * value;


        if(style >= maxStyle)
        {
            //On max style gained
            style = maxStyle;
        }

        OnStyleValueChanged();
    }

    public void LoseStyle(float value)
    {
        style -= MapStyleLoseMultiplier() * value;

        if(style <= -maxStyle)
        {
            //On min style gained
            style = -maxStyle;
        }

        OnStyleValueChanged();
    }


    private void OnStyleValueChanged()
    {
        styleMeter.value = style;
    }

}

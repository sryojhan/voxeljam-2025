using UnityEngine;
using UnityEngine.UI;

public class StyleMeter : Singleton<StyleMeter>
{

    //TODO: This is just informative, delete later
    public enum StyleAction
    {
        Jump, Airborne, Roll, Slide, SuperJump, ForceDownwards
    }

    public enum StyleSubtracters
    {
        Still, GroundStuck, Knockback
    }

    public Slider styleMeter;

    [Header("Style information")]
    const float maxStyle = 100f;
    public float positiveGain = 10f;
    public float negativeGain = 10f;

    public float styleGainMinMultiplier = 3f;
    public float styleGainMaxMultiplier = .3f;

    public float styleLoseMinMultiplier = .3f;
    public float styleLoseMaxMultiplier = 3f;

    [Header("Style score")]
    public float jumpScore = 10f;
    public float airborneScorePower = 1f;
    public float completeRollScore = 5f;
    public float slideScoreMultiplier = 1f;
    public float superJumpScore = 20f;
    public float forceDownwardsMultiplier = 5f;

    [Header("Style subtracters")]
    public float beingStill = 5f;
    public float beingGroundStuck = 2f;
    public float knockBack = 1f;


    private float airborneBegin = -1;


    private float style = 0;



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


    public void BeginGrounded()
    {
        float airborneTime = Time.time - airborneBegin;

        GainStyle(Mathf.Exp(airborneTime * airborneScorePower));
    }

    public void BeginAirborne()
    {
        airborneBegin = Time.time;
    }

    public void Jump()
    {
        GainStyle(jumpScore);
    }

    public void SuperJump()
    {
        GainStyle(superJumpScore);
    }

    public void Airborne(float duration)
    {
        GainStyle(duration * airborneBegin);
    }

    public void Slide(float slideSpeed)
    {
        GainStyle(slideSpeed * slideScoreMultiplier);
    }

    public void RollComplete()
    {
        GainStyle(completeRollScore);
    }

    public void ForceDownFall(float time)
    {
        GainStyle(time * forceDownwardsMultiplier);
    }


    private void Update()
    {
        
    }
}

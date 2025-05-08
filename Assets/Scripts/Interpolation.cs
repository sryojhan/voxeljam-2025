using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Interpolation
{

    public enum Type
    {
        Linear, Sine, Quadratic, Cubic, Quartic, Quintic, Exponencial, Circular, BounceBack
    }

    public enum Mode
    {
        InAndOut, In, Out
    }


    public Type type = Type.Cubic;
    public Mode mode;


    public static float LerpWithInterpolation(float x, float a, float b, Type type = Type.Cubic, Mode mode = Mode.Out)
    {
        return Mathf.Lerp(a, b, Interpolate(x, type, mode));
    }

    public float LerpWithInterpolation(float x, float a, float b)
    {
        return Mathf.Lerp(a, b, Interpolate(x));
    }

    public Vector2 LerpWithInterpolation(float t, Vector2 a, Vector2 b)
    {
        return new Vector2(LerpWithInterpolation(t, a.x, b.x), LerpWithInterpolation(t, a.y, b.y));
    }


    public static float Interpolate(float x, Type type, Mode mode)
    {
        return type switch
        {
            Type.Linear => LinearInterpolation(x, mode),
            Type.Sine => SineInterpolation(x, mode),
            Type.Quadratic => QuadraticInterpolation(x, mode),
            Type.Cubic => CubicInterpolation(x, mode),
            Type.Quartic => QuadraticInterpolation(x, mode),
            Type.Quintic => QuinticInterpolation(x, mode),
            Type.Exponencial => ExponencialInterpolation(x, mode),
            Type.Circular => CircularInterpolation(x, mode),
            Type.BounceBack => BounceBackInterpolation(x, mode),
            _ => LinearInterpolation(x, mode)
        };
    }

    public float Interpolate(float x)
    {

        return type switch
        {
            Type.Linear => LinearInterpolation(x),
            Type.Sine => SineInterpolation(x),
            Type.Quadratic => QuadraticInterpolation(x),
            Type.Cubic => CubicInterpolation(x),
            Type.Quartic => QuadraticInterpolation(x),
            Type.Quintic => QuinticInterpolation(x),
            Type.Exponencial => ExponencialInterpolation(x),
            Type.Circular => CircularInterpolation(x),
            Type.BounceBack => BounceBackInterpolation(x),
            _ => LinearInterpolation(x)
        };

    }

    public static float LinearInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return x;
    }

    public static float SineInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => 1 - Mathf.Cos(x * Mathf.PI * 0.5f),
            Mode.Out => Mathf.Sin(x * Mathf.PI * 0.5f),
            _ => -(Mathf.Cos(Mathf.PI * x) - 1) / 2
        };
    }


    public static float QuadraticInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => x * x,
            Mode.Out => 1 - (1 - x) * (1 - x),
            _ => x < 0.5 
                    ? 2 * x * x 
                    : 1 - Mathf.Pow(-2 * x + 2, 2) / 2
        };
    }

    public static float CubicInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => x * x * x,
            Mode.Out => 1 - Mathf.Pow(1 - x, 3),
            _ => x < 0.5 
                    ? 4 * x * x * x 
                    : 1 - Mathf.Pow(-2 * x + 2, 3) / 2
        };
    }

    public static float QuarticInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => x * x * x * x,
            Mode.Out => 1 - Mathf.Pow(1 - x, 4),
            _ => x < 0.5 ?
                    8 * x * x * x * x 
                    : 1 - Mathf.Pow(-2 * x + 2, 4) / 2
        };
    }

    public static float QuinticInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => x * x * x * x * x,
            Mode.Out => 1 - Mathf.Pow(1 - x, 5),
            _ => x < 0.5 
                    ? 16 * x * x * x * x * x 
                    : 1 - Mathf.Pow(-2 * x + 2, 5) / 2
        };
    }

    public static float ExponencialInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10),
            Mode.Out => x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x),
            _ =>
                x == 0
                    ? 0
                    : x == 1
                        ? 1
                        : x < 0.5
                            ? Mathf.Pow(2, 20 * x - 10) / 2
                            : (2 - Mathf.Pow(2, -20 * x + 10)) / 2
        };
    }

    public static float CircularInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        return mode switch
        {
            Mode.In => 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2)),
            Mode.Out => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2)),
            _ => x < 0.5
                    ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
                    : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2
        };
    }

    public static float BounceBackInterpolation(float x, Mode mode = Mode.InAndOut)
    {
        switch (mode)
        {

            case Mode.In:
                {
                    float c1 = 1.70158f;
                    float c3 = c1 + 1;

                    return c3 * x * x * x - c1 * x * x;
                }
            case Mode.Out:
                {
                    float c1 = 1.70158f;
                    float c3 = c1 + 1;

                    return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
                }
            default:
                {

                    float c1 = 1.70158f;
                    float c2 = c1 * 1.525f;

                    return x < 0.5
                      ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
                      : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;

                }


        }
    }
}

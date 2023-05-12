using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class FuzzyLogic
{

    public static float And(float a, float b)
    {
        return Mathf.Min(a, b);
    }

    public static float Or(float a, float b)
    {
        return Mathf.Max(a, b);
    }

    public static float Not(float a)
    {
        return 1.0f - a;
    }

    public static float Gradient(float fValue, float fLow, float fHigh)
    {

        if (fValue <= fLow)
        {
            return 0.0f;
        }

        else if (fValue >= fHigh)
        {
            return 1.0f;
        }

        else
        {
            float fDifference = fHigh - fLow;
            if (fDifference == 0.0f)
            {
                return 0.0f;
            }

            else
            {
                return ((fValue - fLow) / fDifference);
            }
        }
        

    }

    public static float ReverseGradient(float fValue, float fLow, float fHigh)
    {
        if (fValue <= fHigh)
        {
            return 1.0f;
        }

        else if (fValue >= fLow)
        {
            return 0.0f;
        }

        else
        {
            float fDifference = fLow - fHigh;
            if (fDifference == 0.0f)
            {
                return 0.0f;
            }

            else
            {
                return 1.0f - ((fValue - fHigh) / fDifference);
            }
        }
    }

    public static float Trapezoid(float fValue, float fLowStart, float fHighStart, float fHighEnd, float fLowEnd)
    {
        if (fValue <= fLowStart)
        {
            return 0.0f;
        }

        else if ((fValue >= fHighStart) && (fValue <= fHighEnd))
        {
            return 1.0f;
        }

        else if (fValue >= fLowEnd)
        {
            return 0.0f;
        }

        //Left gradient
        else if ((fValue > fLowStart) && (fValue < fHighStart))
        {
            float fDifference = fHighStart - fLowStart;
            if (fDifference == 0.0f)
            {
                return 0.0f;
            }

            else
            {
                return ((fValue - fLowStart) / fDifference);
            }
        }
        
        //Right gradient
        else
        {
            float fDifference = fLowEnd - fHighEnd;
            if (fDifference == 0.0f)
            {
                return 0.0f;
            }

            else
            {
                return 1.0f - ((fValue - fHighEnd) / fDifference);
            }
        }

    }










}

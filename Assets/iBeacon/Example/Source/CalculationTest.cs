using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static Calculations;

public class CalculationTest : MonoBehaviour
{
    public Text latText;
    public Text longText;

    public void Calculate()
    {
        var b0 = Tuple.Create(32.813129, -96.795350);
        var b1 = Tuple.Create(32.813083, -96.795375);
        var b2 = Tuple.Create(32.813089, -96.795333);
        var ipad = Tuple.Create(32.813040, -96.795404);
        var dA = DistanceKm(b0, ipad);
        var dB = DistanceKm(b1, ipad);
        var dC = DistanceKm(b2, ipad);
        var result = Trilateration(dA, dB, dC);
        latText.text = result.Item1.ToString();
        longText.text = result.Item2.ToString();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3D
{
    static public double[] Mul(double[] a, double [] b)
    {
        return new double[] { a[0]*b[0], a[1]*b[1], a[2] * b[2]};
    }
    static public double[] Mul(double[] a, double b)
    {
        return new double[] { a[0] * b, a[1] * b, a[2] * b };
    }
    static public double[] Div(double[] a, double b)
    {
        return new double[] { a[0] / b, a[1] / b, a[2] / b };
    }
    static public double[] Diff(double[] a, double[] b)
    {
        return new double[] { a[0] - b[0], a[1] - b[1], a[2] - b[2] };
    }
    static public double[] Add(double[] a, double[] b)
    {
        return new double[] { a[0] + b[0], a[1] + b[1], a[2] + b[2] };
    }
    static public double Dot(double[] a, double[] b)
    {
        return (a[0] * b[0]) + (a[1] * b[1]) + (a[2] * b[2]);
    }
    static public double[] Cross(double[] a, double[] b)
    {
        return new double[] { a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0] };
    }
    static public double Magnitude(double[] a)
    {
        return Math.Sqrt(Dot(a, a));
    }
    static public double[] Normalize(double[] a)
    {
        var m = Magnitude(a);
        return new double[] { a[0] / m, a[1] / m, a[2] / m };
    }
}

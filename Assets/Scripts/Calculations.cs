using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Vector3D;
using static System.Math;
using static UnityEngine.Debug;

public class Calculations
{
    static public double ToKm(double meters)
    {
        return meters * 0.001;
    }
    static public double ToM(double kilometers)
    {
        return kilometers * 1000;
    }
    static public double ToRadians(double degrees)
    {
        return (PI / 180) * degrees;
    }

    static public double ToDegrees(double radians)
    {
        return (180 / PI) * radians;
    }

    static public string Str(double[] a)
    {
        return string.Join(", ", a);
    }

    static public double Sq(double a)
    {
        return a * a;
    }
    /// <summary>
    /// Returns distance between two long/lat points, in kilometers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns> 
    static private double _Distance(Tuple<double, double> a, Tuple<double, double> b)
    {
        var lat1 = a.Item1;
        var lon1 = a.Item2;
        var lat2 = b.Item1;
        var lon2 = b.Item2;
        double dist = Sin(ToRadians(lat1))
                        * Sin(ToRadians(lat2))
                        + Cos(ToRadians(lat1))
                        * Cos(ToRadians(lat2))
                        * Cos(ToRadians(lon1 - lon2));
        dist = Acos(dist);
        dist = ToDegrees(dist);
        return dist * 60 * 1.1515 * 1.609344;
    }
    static public double DistanceKm(Tuple<double, double> a, Tuple<double, double> b)
    {
        return _Distance(a, b);
    }
    static public double DistanceM(Tuple<double, double> a, Tuple<double, double> b)
    {
        return ToM(_Distance(a, b));
    }

    // https://gis.stackexchange.com/questions/66/trilateration-using-3-latitude-longitude-points-and-3-distances
    static public Tuple<double, double> Trilateration(double DistA, double DistB, double DistC)
    {
        var earthR = 6371;
        // A
        var LatA = 32.813129;
        var LonA = -96.795350;
        // B
        var LatB = 32.813083;
        var LonB = -96.795375;
        // C
        var LatC = 32.813089;
        var LonC = -96.795333;
        // using authalic sphere
        // Convert geodetic Lat/Long to ECEF xyz
        //    1. Convert Lat/Long to radians
        //    2. Convert Lat/Long(radians) to ECEF

        var xA = earthR * (Cos(ToRadians(LatA)) * Cos(ToRadians(LonA)));
        var yA = earthR * (Cos(ToRadians(LatA)) * Sin(ToRadians(LonA)));
        var zA = earthR * Sin(ToRadians(LatA));

        var xB = earthR * (Cos(ToRadians(LatB)) * Cos(ToRadians(LonB)));
        var yB = earthR * (Cos(ToRadians(LatB)) * Sin(ToRadians(LonB)));
        var zB = earthR * Sin(ToRadians(LatB));

        var xC = earthR * (Cos(ToRadians(LatC)) * Cos(ToRadians(LonC)));
        var yC = earthR * (Cos(ToRadians(LatC)) * Sin(ToRadians(LonC)));
        var zC = earthR * Sin(ToRadians(LatC));

        var p1 = new double[] { xA, yA, zA };
        var p2 = new double[] { xB, yB, zB };
        var p3 = new double[] { xC, yC, zC };
        Log($"Sean - p1: ${Str(p1)}");
        Log($"Sean - p2: ${Str(p2)}");
        Log($"Sean - p3: ${Str(p3)}");

        var ex = Div(Diff(p2, p1), Magnitude(Diff(p2, p1)));
        Log($"Sean - ex: ${Str(ex)}");

        var i = Dot(ex, Diff(p3, p1));
        var diff = Diff(Diff(p3, p1), Mul(ex, i));
        var ey = Normalize(diff);
        Log($"Sean - ey: ${Str(ey)}");
        var ez = Cross(ex, ey);
        Log($"Sean - ez: ${Str(ez)}");
        var d = Magnitude(Diff(p2, p1));
        var j = Dot(ey, Diff(p3, p1));

        var DistA2 = Sq(DistA);
        var DistB2 = Sq(DistB);
        var DistC2 = Sq(DistC);
        var x = (DistA2 - DistB2 + Sq(d)) / (2 * d);
        var y = ((DistA2 - DistC2 + Sq(i) + Sq(j)) / (2 * j)) - ((i / j) * x);
        Log($"Sean - x: ${x}");
        Log($"Sean - y: ${y}");

        var z = Sqrt(DistA2 - Sq(x) - Sq(y));
        Log($"Sean - z: ${z}");
        var triPt = Add(Add(p1, Mul(ex, x)), Add(Mul(ey, y), Mul(ez, z)));
        Log($"Sean - triPt: ${Str(triPt)}");
        var lat = ToDegrees(Asin(triPt[2] / earthR));
        var lon = ToDegrees(Atan2(triPt[1], triPt[0]));
        return Tuple.Create(lat, lon);
    }
}

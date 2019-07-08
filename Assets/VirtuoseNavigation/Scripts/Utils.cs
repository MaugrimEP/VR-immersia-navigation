using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Utils
{
    public static string ArrayToString(float[] array)
    {
        string str = "[";
        foreach (float val in array)
        {
            str += $" {val},";
        }
        str += "]";
        return str;
    }

    public static List<Transform> getAllChilds(Transform me)
    {
        List<Transform> childs = new List<Transform>();
        foreach (Transform child in me)
        {
            childs.AddRange(getAllChilds(child));
        }

        return childs;
    }

    public static Vector3 ClampDisplacement(Vector3 displacementVector, float clampDistance)
    {
        float travelDistance = displacementVector.magnitude;

        if (travelDistance > clampDistance)
        {// need to size down the distance
            displacementVector = Vector3.Lerp(Vector3.zero, displacementVector, clampDistance / travelDistance);
            //displacementVector /= (travelDistance / clampDistance);
        }
        return displacementVector;
    }

    public static Vector3 ClampDisplacement(Vector3 oldPosition, Vector3 newPosition, float clampDistance)
    {
        return oldPosition + ClampDisplacement(newPosition - oldPosition, clampDistance);
    }

    public static Vector3 ClampVector3(Vector3 v, float max)
    {
        return new Vector3(Mathf.Clamp(v.x, -max, max), Mathf.Clamp(v.y, -max, max), Mathf.Clamp(v.z, -max, max));
    }

    public static Color RandomColor()
    {
        return Random.ColorHSV();
    }

    public static string Vector3Text(Vector3 v, int decimalNumber)
    {
        string format = $"F{decimalNumber}";
        return $"({v.x.ToString(format)}, {v.y.ToString(format)}, {v.z.ToString(format)})";
    }

    public static bool Equals(Collision c1, Collision c2)
    {
        if (c1.contactCount != c2.contactCount) return false;
        for (int i = 0; i < c1.contactCount; ++i)
        {
            ContactPoint c1Point = c1.GetContact(i);
            ContactPoint c2Point = c2.GetContact(i);

            if (c1Point.separation != c2Point.separation) return false;
            if (c1Point.normal != c2Point.normal) return false;
        }
        return true;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        sVector = sVector.Replace("−", "-");
        var fmt = new NumberFormatInfo { NegativeSign = "-" };

        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0], fmt),
            float.Parse(sArray[1], fmt),
            float.Parse(sArray[2], fmt));

        return result;
    }

    public static Vector3 Treshold(this Vector3 v, float treshold, float defaultValue)
    {
        Vector3 res = Vector3.zero;
        res.x = Mathf.Abs(v.x) > treshold ? v.x : defaultValue;
        res.y = Mathf.Abs(v.y) > treshold ? v.y : defaultValue;
        res.z = Mathf.Abs(v.z) > treshold ? v.z : defaultValue;

        return res;
    }

    public static Quaternion ScalarMul(this Quaternion q, float scalar)
    {
        return new Quaternion(q.x * scalar, q.y * scalar, q.z * scalar, q.w * scalar);
    }

    /// <summary>
    /// Get a point in the Sphere or projectected on the sphere
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="sphereRadius"></param>
    /// <returns></returns>
    public static Vector3 GetPointInSphere(this Vector3 pos, float sphereRadius)
    {
        if (pos.magnitude < sphereRadius) return pos;
        return pos * sphereRadius / pos.magnitude;
    }

    public static Vector3 Clamp(this Vector3 v3, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(v3.x, -1f, 1f),
            Mathf.Clamp(v3.y, -1f, 1f),
            Mathf.Clamp(v3.z, -1f, 1f));
    }

    public static float OrientedAngle(this float angle)
    {
        if (angle > 180) return angle - 360;
        else return angle;
    }

    public static float TresholdLower(this float value, float treshold, float defaultValue)
    {
        if (Mathf.Abs(value) < treshold) return defaultValue;
        else return value;
    }

    public static float Signe(this float value)
    {
        return value < 0? -1 : 1;
    }
}


/// <summary>
/// X toward us, y right, z up
/// </summary>
public class InertiaMatrix
{
    private float[,] tab;

    public float[,] GetMatrix2D()
    {
        return tab;
    }

    public float[] GetMatrix1D()
    {
        float[] res = new float[9];
        for (int line = 0; line < 3; ++line)
            for (int column = 0; column < 3; ++column)
                res[line * 3 + column] = tab[line, column];
        return res;
    }

    /// <summary>
    /// Compute the inertia matrix for the raquette
    /// </summary>
    /// <param name="density">the density in kg.m^-3</param>
    /// <returns>
    /// the inertia matrix
    /// mass in kg
    /// </returns>
    public static (InertiaMatrix inertiaMatrix, float mass) GetRaquette(float density)
    {
        //old > this would need a axis changement
        //InertiaMatrix PaveExterieur = GetPave(mass: 1f, length: 0.4f, width: 0.4f, heigh: 0.02f);
        //InertiaMatrix PaveInterieur = GetPave(mass: 1f, length: 0.33f, width: 0.33f, heigh: 0.02f);

        //this new call take into account the change of axis, we just swap the height and the lenght
        (InertiaMatrix PaveExterieur, float massPaveExterieur) = GetPaveDensity(density: density, length: 0.02f, width: 0.4f, heigh: 0.4f);
        (InertiaMatrix PaveInterieur, float massPaveInterieur) = GetPaveDensity(density: density, length: 0.02f, width: 0.33f, heigh: 0.33f);

        InertiaMatrix CerceauCentered = (PaveExterieur - PaveInterieur);
        // translation from the center to the handle
        Vector3 translation = new Vector3(0f, 0f, 0.2f + 0.1f);
        InertiaMatrix Cerceau = Translated(im: CerceauCentered, t: translation, mass: massPaveExterieur - massPaveInterieur);

        (InertiaMatrix Handle, float massHandle) = GetCylinderDensity(density: density, radius: 0.05f / 2, heigh: 0.2f);

        InertiaMatrix Raquette = Handle + Cerceau;

        float totalMass = massPaveExterieur - massPaveInterieur + massHandle;

        return (Raquette, totalMass);
    }

    /// <summary>
    /// With the application point at the center
    /// </summary>
    /// <param name="mass"></param>
    /// <param name="length"></param>
    /// <param name="width"></param>
    /// <param name="heigh"></param>
    /// <returns></returns>
    public static InertiaMatrix GetPave(float mass, float length, float width, float heigh)
    {
        return new InertiaMatrix(new float[3, 3] {
            { mass*(width*width+heigh*heigh) / 12f, 0                                      , 0                                        },
            { 0                                   , mass*(length*length+heigh*heigh) / 12f , 0                                        },
            { 0                                   , 0                                      , mass*(length*length+width*width) / 12f } });
    }

    public static (InertiaMatrix inertiaMatrix, float mass) GetPaveDensity(float density, float length, float width, float heigh)
    {
        float volume = length * width * heigh;
        float mass = volume * density;

        return (GetPave(mass, length, width, heigh), mass);
    }

    /// <summary>
    /// With the application point a the center
    /// </summary>
    /// <param name="mass"></param>
    /// <param name="radius"></param>
    /// <param name="heigh"></param>
    /// <returns></returns>
    public static InertiaMatrix GetCylinder(float mass, float radius, float heigh)
    {

        return new InertiaMatrix(new float[3, 3] {
            { mass*(radius*radius/4f + heigh*heigh/12f), 0                                         , 0                            },
            { 0                                        , mass*(radius*radius/4f + heigh*heigh/12f) , 0                            },
            { 0                                        , 0                                         , mass * radius * radius / 2f} });
    }
    public static (InertiaMatrix inertiaMatrix, float mass) GetCylinderDensity(float density, float radius, float heigh)
    {
        float volume = Mathf.PI * Mathf.Pow(radius, 2) * heigh;
        float mass = volume * density;
        return (GetCylinder(mass, radius, heigh), mass);
    }

    private InertiaMatrix(float[,] tab)
    {
        this.tab = tab;
    }

    private InertiaMatrix()
    {
        tab = new float[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    }

    public static InertiaMatrix operator +(InertiaMatrix im1, InertiaMatrix im2)
    {
        InertiaMatrix res = new InertiaMatrix();
        for (int line = 0; line < 3; ++line)
            for (int column = 0; column < 3; ++column)
                res.tab[line, column] = im1.tab[line, column] + im2.tab[line, column];

        return res;
    }
    public static InertiaMatrix operator *(float scalar, InertiaMatrix im2)
    {
        InertiaMatrix res = new InertiaMatrix();
        for (int line = 0; line < 3; ++line)
            for (int column = 0; column < 3; ++column)
                res.tab[line, column] = scalar * im2.tab[line, column];
        return res;
    }

    public static InertiaMatrix operator -(InertiaMatrix im1, InertiaMatrix im2)
    {
        return im1 + (-1 * im2);
    }
    /// <summary>
    /// Translate the inertia matrice from the previous application point, to the previous application point + the translation vector t
    /// </summary>
    /// <param name="t"></param>
    /// <param name="mass"></param>
    public static InertiaMatrix Translated(InertiaMatrix im, Vector3 t, float mass)
    {
        float[,] translationMatrix = new float[3, 3] {
                                                        { t.y*t.y + t.z*t.z, -t.x*t.y         , -t.x*t.z            },
                                                        { -t.x*t.y         , t.x*t.x + t.z*t.z, -t.y*t.z            },
                                                        { -t.x*t.z         , -t.y*t.z         , t.x*t.x + t.y*t.y } };

        InertiaMatrix temp = mass * new InertiaMatrix(translationMatrix);
        InertiaMatrix res = im + temp;
        return res;
    }

    public override string ToString()
    {
        string res = "";

        for (int line = 0; line < 3; ++line)
        {
            res += "[";
            for (int column = 0; column < 3; ++column)
            {
                res += $"{tab[line, column]}, ";
            }
            res += "\n";

        }
        res += "]";

        return res;
    }
}
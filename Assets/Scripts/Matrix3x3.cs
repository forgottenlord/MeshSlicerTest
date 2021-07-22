using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Matrix3x3
{
    float a11; float a12; float a13;
    float a21; float a22; float a23;
    float a31; float a32; float a33;
    public Matrix3x3(float[] a)
    {
        float a11 = a[0]; float a12 = a[1]; float a13 = a[2];
        float a21 = a[3]; float a22 = a[4]; float a23 = a[5];
        float a31 = a[6]; float a32 = a[7]; float a33 = a[8];
    }
    public float GetDeterminant()
    {
        return a11 * a22 * a33 +
            a12 * a23 * a31 +
            a13 * a21 * a32 -

            a13 * a22 * a31 -
            a11 * a23 * a32 -
            a12 * a21 * a33;
    }
}
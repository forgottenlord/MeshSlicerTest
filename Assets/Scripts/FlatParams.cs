using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Параметры ABCD из уравнения прямой.
/// a * x + b * y + c * z + d = 0
/// </summary>
public struct FlatParams
{
    public float a;
    public float b;
    public float c;
    public float d;
    public override string ToString()
    {
        return string.Format("a={0}; b={1}; c={2}; d={3}", a, b, c, d);
    }
}
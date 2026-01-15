using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace _3D_SFML.models;
public struct Vector3(float x, float y, float z)
{
    public float X { get; set; } = x;
    public float Y { get; set; } = y;
    public float Z { get; set; } = z;
    
}

public struct Triangle
{
    public Vector3[] p;

    public Triangle()
    {
        p = new Vector3[3];
        
    }
    public  Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        p = new Vector3[3];
        p[0] = a;
        p[1] = b;
        p[2] = c;
    }

    public Color Color;
}

public struct Mat4
{
    public float[][] m;
    public  Mat4()
    {
        m = new float[4][];
        for (int i = 0; i < 4; i++)
        {
            m[i] = new float[4];
        }
    }
}
public class Mesh(List<Triangle> triangles)
{
    public List<Triangle> Triangles { get; set; } = triangles;
}
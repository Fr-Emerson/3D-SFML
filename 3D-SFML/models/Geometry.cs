using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using SFML.Graphics;
using SFML.Graphics.Glsl;

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

    public bool LoadFromObjectFile(string sFilename)
    {
        try
        {
            List<Vector3> verts = new List<Vector3>();
            using FileStream fs = new FileStream(
                sFilename,
                FileMode.Open,
                FileAccess.Read
            );
            using StreamReader f = new StreamReader(sFilename);
            string? line;
            while ((line = f.ReadLine()) != null)
            {
                StringReader s = new StringReader(line);
                char trash;
                if (line.StartsWith('v'))
                {
                    string[] parts = line.Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    Vector3 v = new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    );
                    verts.Add(v);
                }

                if (line.StartsWith('f'))
                {
                    string[] p = line.Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries
                    );
                    int i0 = int.Parse(p[1].Split('/')[0]) - 1;
                    int i1 = int.Parse(p[2].Split('/')[0]) - 1;
                    int i2 = int.Parse(p[3].Split('/')[0]) - 1;

                    Triangles.Add(new Triangle(
                        verts[i0],
                        verts[i1],
                        verts[i2]
                    ));
                    
                }
            }
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
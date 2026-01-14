using System;
using System.Collections.Generic;
using _3D_SFML.models;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace _3D_SFML;

public class Render : RenderWindow
{
    public Mesh Cube;
    public Mat4 Projection;
    
    #region ||-- Overrides --||
    public Render(VideoMode mode, string title) : base(mode, title)
    {
        Closed += (sender,e ) => Close();
    }

    public Render(VideoMode mode, string title, Styles style) : base(mode, title, style)
    {
        Closed += (sender,e ) => Close();
    }

    public Render(VideoMode mode, string title, Styles style, ContextSettings settings) : base(mode, title, style, settings)
    {
        Closed += (sender,e ) => Close();
    }

    public Render(IntPtr handle) : base(handle)
    {
        Closed += (sender,e ) => Close();
    }

    public Render(IntPtr handle, ContextSettings settings) : base(handle, settings)
    {
        Closed += (sender,e ) => Close();
    }

    public void Create(VideoMode mode, string title)
    {
        if (IsOpen)
        {
            Close();
        }
        
        Create(mode, title, Styles.Default);
    }

    public void Create(VideoMode mode, string title, Styles style)
    {
        base.Close();
    }
    #endregion

    public bool OnCreate()
    {
        InitializeCube();
        float fNear = 0.1f;
        float fFar = 1000f;
        float fFov = 90f;
        float fAspectRatio = (float)Size.Y / (float)Size.X;
        float fFovRad = 1f / (float)Math.Tan(fFov * 0.5f / 180f * 3.14159f);
        Projection = new Mat4();
        
        Projection.m[0][0] = fAspectRatio * fFovRad;
        Projection.m[1][1] = fFovRad;
        Projection.m[2][2] = fFar / (fFar - fNear);
        Projection.m[3][2] = (-fFar * fNear) / (fFar - fNear);
        Projection.m[2][3] = 1f;
        Projection.m[3][3] = 0f;
        return true;
    }

    public void Run()
    {
        OnCreate();
        
        while (IsOpen)
        {
            DispatchEvents();
            OnUpdate();
        }
    }
    
    Vector3 MultiplyMatrixVector(Vector3 i, ref Mat4 m)
    {
        Vector3 o = new Vector3();
        o.X = i.X * m.m[0][0] + i.Y * m.m[1][0] + i.Z * m.m[2][0] + m.m[3][0];
        o.Y = i.X * m.m[0][1] + i.Y * m.m[1][1] + i.Z * m.m[2][1] + m.m[3][1];
        o.Z = i.X * m.m[0][2] + i.Y * m.m[1][2] + i.Z * m.m[2][2] + m.m[3][2];
        float w = i.X * m.m[0][3] + i.Y * m.m[1][3] + i.Z * m.m[2][3] + m.m[3][3];

        if (w != 0.0f)
        {
            o.X /= w; o.Y /= w; o.Z /= w;
        }
        return o;
    }

    public bool OnUpdate()
    {
        Clear(Color.Black);
    
        for (int i = 0; i < Cube.Triangles.Count; i++)
        {
            Triangle tri = Cube.Triangles[i];
            Triangle projected = new Triangle();
            Triangle translated = new Triangle();
            
            translated.p[0] = new Vector3(tri.p[0].X, tri.p[0].Y, tri.p[0].Z + 3.0f);
            translated.p[1] = new Vector3(tri.p[1].X, tri.p[1].Y, tri.p[1].Z + 3.0f);
            translated.p[2] = new Vector3(tri.p[2].X, tri.p[2].Y, tri.p[2].Z + 3.0f);
            
            projected.p[0] = MultiplyMatrixVector(translated.p[0], ref Projection);
            projected.p[1] = MultiplyMatrixVector(translated.p[1], ref Projection);
            projected.p[2] = MultiplyMatrixVector(translated.p[2], ref Projection);
        
            DrawTriangle(projected);
        }
    
        Display();
        return true;
    }

    private void DrawTriangle(Triangle tri)
    {
        float offsetX = Size.X / 2f;
        float offsetY = Size.Y / 2f;
        float scale = 500f;
    
        Vector2f p0 = new Vector2f(
            tri.p[0].X * scale + offsetX, 
            tri.p[0].Y * scale + offsetY
        );
        Vector2f p1 = new Vector2f(
            tri.p[1].X * scale + offsetX, 
            tri.p[1].Y * scale + offsetY
        );
        Vector2f p2 = new Vector2f(
            tri.p[2].X * scale + offsetX, 
            tri.p[2].Y * scale + offsetY
        );
    
        ConvexShape triangle = new ConvexShape(3);
        triangle.SetPoint(0, p0);
        triangle.SetPoint(1, p1);
        triangle.SetPoint(2, p2);
        triangle.FillColor = Color.Transparent;
        triangle.OutlineColor = Color.White;
        triangle.OutlineThickness = 1f;
    
        Draw(triangle);
    }

    public void InitializeCube()
    {
        Cube = new Mesh(new List<Triangle>
        {
            // SOUTH
            new Triangle(
                new Vector3(0,0,0),
                new Vector3(0,1,0),
                new Vector3(1,1,0)
            ),
            new Triangle(
                new Vector3(0,0,0),
                new Vector3(1,1,0),
                new Vector3(1,0,0)
            ),

            // EAST
            new Triangle(
                new Vector3(1,0,0),
                new Vector3(1,1,0),
                new Vector3(1,1,1)
            ),
            new Triangle(
                new Vector3(1,0,0),
                new Vector3(1,1,1),
                new Vector3(1,0,1)
            ),

            // NORTH
            new Triangle(
                new Vector3(1,0,1),
                new Vector3(1,1,1),
                new Vector3(0,1,1)
            ),
            new Triangle(
                new Vector3(1,0,1),
                new Vector3(0,1,1),
                new Vector3(0,0,1)
            ),

            // WEST
            new Triangle(
                new Vector3(0,0,1),
                new Vector3(0,1,1),
                new Vector3(0,1,0)
            ),
            new Triangle(
                new Vector3(0,0,1),
                new Vector3(0,1,0),
                new Vector3(0,0,0)
            ),

            // TOP
            new Triangle(
                new Vector3(0,1,0),
                new Vector3(0,1,1),
                new Vector3(1,1,1)
            ),
            new Triangle(
                new Vector3(0,1,0),
                new Vector3(1,1,1),
                new Vector3(1,1,0)
            ),

            // BOTTOM
            new Triangle(
                new Vector3(1,0,1),
                new Vector3(0,0,1),
                new Vector3(0,0,0)
            ),
            new Triangle(
                new Vector3(1,0,1),
                new Vector3(0,0,0),
                new Vector3(1,0,0)
            ),
        });
    }
}
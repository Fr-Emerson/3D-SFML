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
    private Vector3 _cameraPosition = new Vector3();
    
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
        float fFov = 60f;
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

    private float fTheta = 0f;
    public bool OnUpdate()
    {
        Clear(Color.Black);

        Mat4 matRotZ = new Mat4();
        Mat4 matRotX = new Mat4();
        fTheta += 0.0008f ;

        
        // Rotacionar eixo Z
        matRotZ.m[0][0] = MathF.Cos(fTheta);
        matRotZ.m[0][1] = MathF.Sin(fTheta);
        matRotZ.m[1][0] = -MathF.Sin(fTheta);
        matRotZ.m[1][1] = MathF.Cos(fTheta);
        matRotZ.m[2][2] = 1f;
        matRotZ.m[3][3] = 1f;
        
        //Rotacionar eixo X
        matRotX.m[0][0] = 1f;
        matRotX.m[1][1] = MathF.Cos(fTheta*0.5f);
        matRotX.m[1][2] = MathF.Sin(fTheta*0.5f);
        matRotX.m[2][1] = -MathF.Sin(fTheta*0.5f);
        matRotX.m[2][2] = MathF.Cos(fTheta*0.5f);
        matRotX.m[3][3] = 1f;
    
        for (int i = 0; i < Cube.Triangles.Count; i++)
        {
            Triangle tri = Cube.Triangles[i];
            Triangle projected = new Triangle();
            Triangle triRotatedZ = new Triangle();
            Triangle triRotatedZx = new Triangle();
            
            triRotatedZ.p[0] = MultiplyMatrixVector(tri.p[0], ref matRotZ);
            triRotatedZ.p[1] = MultiplyMatrixVector(tri.p[1], ref matRotZ);
            triRotatedZ.p[2] = MultiplyMatrixVector(tri.p[2], ref matRotZ);
            
            triRotatedZx.p[0] = MultiplyMatrixVector(triRotatedZ.p[0], ref matRotX);
            triRotatedZx.p[1] = MultiplyMatrixVector(triRotatedZ.p[1], ref matRotX);
            triRotatedZx.p[2] = MultiplyMatrixVector(triRotatedZ.p[2], ref matRotX);
            
            Triangle translated = new Triangle() ;
            translated.p[0] = new Vector3(triRotatedZx.p[0].X, triRotatedZx.p[0].Y, triRotatedZx.p[0].Z + 3.0f);
            translated.p[1] = new Vector3(triRotatedZx.p[1].X, triRotatedZx.p[1].Y, triRotatedZx.p[1].Z + 3.0f);
            translated.p[2] = new Vector3(triRotatedZx.p[2].X, triRotatedZx.p[2].Y, triRotatedZx.p[2].Z + 3.0f);

            Vector3 normal = new Vector3();
            Vector3 line1 = new Vector3();
            Vector3 line2 = new Vector3();
            
            line1.X = translated.p[1].X  - translated.p[0].X;
            line1.Y = translated.p[1].Y  - translated.p[0].Y;
            line1.Z = translated.p[1].Z  - translated.p[0].Z;
            
            line2.X = translated.p[2].X  - translated.p[0].X;
            line2.Y = translated.p[2].Y  - translated.p[0].Y;
            line2.Z = translated.p[2].Z  - translated.p[0].Z;
            
            normal.X = line1.Y * line2.Z - line1.Z * line2.Y;
            normal.Y = line1.Z * line2.X - line1.X * line2.Z;
            normal.Z = line1.X * line2.Y - line1.Y * line2.X;
            
            float l = MathF.Sqrt(normal.X*normal.X + normal.Y*normal.Y + normal.Z*normal.Z);
            normal.X /= l;
            normal.Y /= l;
            normal.Z /= l;
            if (
                normal.X *(translated.p[0].X - _cameraPosition.X) + 
                normal.Y *(translated.p[0].Y - _cameraPosition.Y) +
                normal.Z *(translated.p[0].Z - _cameraPosition.Z) < 0
                
                )
            {

                Vector3 lightDirection = new Vector3(0.0f, 0.0f, -1.0f);
                l = MathF.Sqrt(lightDirection.X*lightDirection.X + lightDirection.Y*lightDirection.Y + lightDirection.Z*lightDirection.Z);

                lightDirection.X /= l;
                lightDirection.Y /= l;
                lightDirection.Z /= l;

                float dp = normal.X * lightDirection.X + normal.Y * lightDirection.Y + normal.Z * lightDirection.Z;
                Color baseColor = Color.Blue;

                projected.Color = new Color(
                    (byte)(baseColor.R * dp),
                    (byte)(baseColor.G * dp),
                    (byte)(baseColor.B * dp)
                );
                projected.p[0] = MultiplyMatrixVector(translated.p[0], ref Projection);
                projected.p[1] = MultiplyMatrixVector(translated.p[1], ref Projection);
                projected.p[2] = MultiplyMatrixVector(translated.p[2], ref Projection);
                
                DrawTriangle(projected);
            }
        
        }
    
        Display();
        return true;
    }

    
    private void DrawTriangle(Triangle tri)
    {
        float offsetX = Size.X / 2f;
        float offsetY = Size.Y / 2f;
        float scale = 250f;
    
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
        triangle.FillColor = tri.Color;
        triangle.OutlineColor = Color.Transparent;
        triangle.OutlineThickness = 1.5f;
    
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
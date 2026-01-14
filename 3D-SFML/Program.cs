using System;
using SFML.Window;
using SFML.Graphics;
namespace _3D_SFML;

public class Program
{
    
    private static void Main()
    {
        
        Render window = new Render(new VideoMode(800, 600), "3D SFML Window");
        window.Run();
        Console.WriteLine("Fim");
        
    }
}
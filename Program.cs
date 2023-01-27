using Raylib_cs;

namespace Gnutella
{
    static class Program
    {
        private static Listener listener = new Listener();
        private static PingSender pingSender = new PingSender();

        public static void Main()
        {
            Raylib.InitWindow(640, 480, "Gnutella");

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                pingSender.Main();
                //Console.WriteLine("sup");

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
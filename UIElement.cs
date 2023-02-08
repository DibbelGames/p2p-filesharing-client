using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Gnutella
{
    public class TextBox
    {
        private Vector2 position;
        private Vector2 size;

        private string placeholder = string.Empty;
        private string initialPlaceholder = string.Empty;
        private int fontSize;

        private bool selected = false;
        private List<char> input = new List<char>();
        public string input_string = string.Empty;
        private int input_size = 0;

        public TextBox(Vector2 position, Vector2 size, string placeholder, int fontSize)
        {
            this.position = position;
            this.size = size;
            this.placeholder = placeholder;
            this.initialPlaceholder = placeholder;
            this.fontSize = fontSize;
        }

        public void Main()
        {
            Rectangle box = new Rectangle(position.X, position.Y, size.X, size.Y);
            Color boxColor = Color.LIGHTGRAY;

            if (CheckCollisionPointRec(GetMousePosition(), box))
            {
                boxColor = Color.GRAY;
                if (IsMouseButtonPressed(0))
                {
                    selected = true;
                    input = new List<char>();
                }
            }
            else
            {
                boxColor = Color.LIGHTGRAY;
                if (IsMouseButtonPressed(0))
                {
                    selected = false;
                }
            }

            if (selected)
            {
                if (input_size <= 0)
                {
                    placeholder = "|";
                }

                int key = GetCharPressed();
                if (key >= 32 && key <= 125)
                {
                    input.Add((char)key);
                    input_size++;
                }

                if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                {
                    if (input_size > 0)
                    {
                        input_size--;
                        input = new List<char>();
                    }
                }
                input_string = new string(input.ToArray());
            }
            else
            {
                if (input_size <= 0)
                {
                    placeholder = initialPlaceholder;
                }
            }

            DrawRectangleRec(box, boxColor);
            if (input_size == 0)
            {
                DrawText(placeholder, (int)position.X + 4, (int)position.Y + GetFontDefault().baseSize / 2, fontSize, Color.BLACK);
            }
            else
            {
                DrawText(input_string, (int)position.X + 4, (int)position.Y + GetFontDefault().baseSize / 2, fontSize, Color.BLACK);
            }
        }
    }

    public class Button
    {
        private Vector2 position;
        private Vector2 size;

        private string placeholder = string.Empty;
        private int fontSize;

        private Action pressAction;

        public Button(Vector2 position, Vector2 size, string placeholder, int fontSize, Action pressAction)
        {
            this.position = position;
            this.size = size;
            this.placeholder = placeholder;
            this.fontSize = fontSize;
            this.pressAction = pressAction;
        }

        public void Main()
        {
            Rectangle box = new Rectangle(position.X, position.Y, size.X, size.Y);
            Color boxColor = Color.LIGHTGRAY;

            if (CheckCollisionPointRec(GetMousePosition(), box))
            {
                boxColor = Color.GRAY;
                if (IsMouseButtonPressed(0))
                {
                    pressAction();
                }
            }
            else
            {
                boxColor = Color.LIGHTGRAY;
            }

            DrawRectangleRec(box, boxColor);
            DrawText(placeholder, (int)position.X + 4, (int)position.Y + GetFontDefault().baseSize / 2, fontSize, Color.BLACK);
        }
    }

    public class InformationBox
    {
        private Vector2 position;

        private string text = string.Empty;
        private int fontSize;

        int frameCounter = -100000;

        public InformationBox(Vector2 position, string text, int fontSize)
        {
            this.position = position;
            this.text = text;
            this.fontSize = fontSize;
        }

        public void ShowInfo(string text)
        {
            frameCounter = 0;
            this.text = text;
        }

        public void Main()
        {
            frameCounter++;
            if (frameCounter >= 60 * 5)
            {
                text = "";
                frameCounter = 0;
            }

            DrawText(text, (int)position.X, (int)position.Y, fontSize, Color.RED);
        }
    }
}
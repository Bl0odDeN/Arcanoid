using System;
using System.IO;
using System.Threading;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using System.Reflection;

class Program
{
    static RenderWindow window;

    static Texture ballTexture;
    static Texture stickTexture;
    static Texture blockTexture1;

    static Sprite stick;
    static Sprite[] blocks;
    static Random rnd = new Random();
    static int[] blockLive;
    static int health = 3;

    static int gamePlay = 0;
    static int level = 1;
    static int total_blocks = 0;

    static Ball ball;

    static Text text = new Text();
    static Font textFont = new Font("arial.ttf");

    public static void initializeWindow()
    {
        window.Clear();
        window.DispatchEvents();
    }
    public static void SetStartPosition(int level)
    {
        for (int i = 0; i < blockLive.Length; i++) blocks[i].Position = new Vector2f(1000, 1000);
        int index = 0;
        for (int y = 0; y < level * 3; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                blocks[index] = new Sprite(blockTexture1);
                blocks[index].Position = new Vector2f(x * (blocks[index].TextureRect.Width + 15) + 75, 
                                                      y * (blocks[index].TextureRect.Height + 5) + 50);
                blockLive[index] = rnd.Next(0, level + 2);
                index++;
            }
        }
        total_blocks = level * 10 * 3;
        stick.Position = new Vector2f(400, 500);
        ball.sprite.Position = new Vector2f(375, 400);
    }
    
    static void Main(string[] args)
    {
        window = new RenderWindow(new VideoMode(800, 600), "Arcanoid");
        window.SetFramerateLimit(75);
        window.Closed += Window_Closed;

        //Textures
        ballTexture = new Texture("Ball.png");
        stickTexture = new Texture("Stick.png");
        blockTexture1 = new Texture("Block.png");

        ball = new Ball(ballTexture);
        stick = new Sprite(stickTexture);
        blocks = new Sprite[100];
        blockLive = new int[100];
        rnd = new Random();

        for (int i = 0; i < blocks.Length; i++) blocks[i] = new Sprite(blockTexture1);

        SetStartPosition(level);

        while (window.IsOpen == true)
        {
            while (gamePlay == 0 || gamePlay == 2 || gamePlay == 3 || gamePlay == 4)
            {
                initializeWindow();
                if (gamePlay == 2)
                {
                    text = new Text($"Next level {level}", textFont);
                    text.Position = new Vector2f(290, 250);
                    text.FillColor = Color.Red;//Цвет текста
                    text.CharacterSize = 38;//Размер текста
                    window.Draw(text);
                    window.Display();
                    System.Threading.Thread.Sleep(2000);
                    gamePlay = 1;
                    ball.SetStartSpeedBall(0);
                    break;
                }
                if (gamePlay == 3)
                {
                    text = new Text("   YOU WIN!\n" +
                                    "Restart game - R\n" +
                                    "Exit game - Esc", textFont);
                    text.Position = new Vector2f(265, 220);
                    text.FillColor = Color.Green;//Цвет текста
                    text.CharacterSize = 38;//Размер текста
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) == true)
                    {
                        window.Close();
                        break;
                    }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.R) == true)
                    {
                        level = 1;
                        health = 3;
                        gamePlay = 1;
                        SetStartPosition(level);
                        ball.SetStartSpeedBall(0);
                    }
                    window.Draw(text);
                    window.Display();
                    break;

                }
                if (gamePlay == 4)
                {
                    text = new Text("          PAUSE\n" +
                                    "Return to the game - P", textFont);
                    text.Position = new Vector2f(225, 220);
                    text.FillColor = Color.Red;//Цвет текста
                    text.CharacterSize = 36;//Размер текста
                    window.Draw(text);
                    if (Keyboard.IsKeyPressed(Keyboard.Key.P) == true)
                    {
                        gamePlay = 1;
                        System.Threading.Thread.Sleep(200);
                    }
                    window.Display();
                    break;
                }
                text = new Text("Start game - R\n" +
                                "Exit game - Esc", textFont); 
                text.Position = new Vector2f(265, 220);
                text.FillColor = Color.Red;//Цвет текста
                text.CharacterSize = 36;//Размер текста
                window.Draw(text);
                if(Keyboard.IsKeyPressed(Keyboard.Key.R) == true)
                {
                    gamePlay = 1;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) == true)
                {
                    window.Close();
                    break;
                }
                window.Display();
            }
            while(gamePlay == 1)
            {
                initializeWindow();

                if (Mouse.IsButtonPressed(Mouse.Button.Left) == true) ball.Start(5, new Vector2f(0, -1));

                if (ball.Move(new Vector2i(0, 0), new Vector2i(800, 600)))
                {
                    if (ball.sprite.Position.Y == 600) //Условие выхода мяча за нижнюю границу
                    {
                        health -= 1;
                    }
                    if (health < 1) //Конец игры
                    {
                        text = new Text("GAME OVER", textFont);
                        text.Position = new Vector2f(275, 250);
                        text.FillColor = Color.Red;//Цвет текста
                        text.CharacterSize = 38;//Размер текста
                        level = 1;
                        gamePlay = 0;
                        window.Draw(text);
                        window.Display();
                        health = 3;
                        System.Threading.Thread.Sleep(5000);
                    }
                    ball.sprite.Position = new Vector2f(Mouse.GetPosition(window).X - 10, stick.Position.Y - ball.sprite.TextureRect.Height); //Установка мяча по стику
                }
                //Проверка на столкновение
                ball.CheckCollision(stick, "Stick");
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (ball.CheckCollision(blocks[i], "Block") == true)
                    {
                        blockLive[i] -= 1;
                        if (blockLive[i] <= 0)
                        {
                            blocks[i].Position = new Vector2f(1000, 1000);
                            total_blocks--;
                            break;
                        }
                        break; //Если не срабатывает условие
                    }
                    if(total_blocks == 0)
                    {
                        text = new Text("Level completed", textFont);
                        text.Position = new Vector2f(265, 250);
                        text.FillColor = Color.Red;//Цвет текста
                        text.CharacterSize = 36;//Размер текста
                        window.Draw(text);
                        window.Display();
                        System.Threading.Thread.Sleep(1800);
                        level++;
                        gamePlay = 2;
                        SetStartPosition(level);
                        ball.sprite.Position = new Vector2f(375, 400);
                        window.Clear();
                    } 
                }
                if (level >= 4) //Ограничение по левелу на WIN
                {
                    gamePlay = 3;
                }
                //Позиция платформы
                stick.Position = new Vector2f(Mouse.GetPosition(window).X - stick.TextureRect.Width * 0.5f, stick.Position.Y);
                //Отрисовка
                window.Draw(ball.sprite);
                window.Draw(stick);
                //Текст
                text.Font = textFont;//Положить в переменную
                text = new Text($"Осталось мячей: {health} | Level: {level} |                                    | Pause - P | Main menu - E | Exit game - Esc", textFont);
                text.CharacterSize = 18;//Размер текста
                text.Position = new Vector2f(5, 576);//Положение текста
                text.FillColor = Color.Green;//Цвет текста
                window.Draw(text);
                if (Keyboard.IsKeyPressed(Keyboard.Key.E) == true)
                {
                    gamePlay = 0;
                    level = 1;
                    health = 3;
                    SetStartPosition(level);
                    ball.SetStartSpeedBall(0);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) == true)
                {
                    window.Close();
                    break;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.P) == true)
                {
                    gamePlay = 4;
                    System.Threading.Thread.Sleep(200);
                }
                for (int i = 0; i < blocks.Length; ++i)
                {
                    window.Draw(blocks[i]);
                }
                window.Display();
            }
        }
    }
    private static void Window_Closed(object sender, EventArgs e) { window.Close(); }
}

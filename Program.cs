using R = Raylib_cs.Raylib;
using Raylib_cs;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

class GameWindow
{
    public static void Main()
    {
        int gameWindowX = 800;
        int gameWindowY = 600;

        R.InitWindow(gameWindowX, gameWindowY, "Space Invaders");
         
        int FPS = 60;
        R.SetTargetFPS(FPS);
        int frameCounter = 0;
        Random random = new Random();
        int spawnInterval = random.Next(2, 6) * FPS;



        Player player = new Player(gameWindowX, gameWindowY);
        List<Rocket> rockets = new List<Rocket>();
        List<Enemy> enemies = new List<Enemy>();

        while (!R.WindowShouldClose())
        {
            R.BeginDrawing();
            R.ClearBackground(Color.Black);
            
            player.Move();
            player.Draw();
            if (R.IsKeyPressed(KeyboardKey.Space) || R.IsMouseButtonPressed(MouseButton.Left))
            {
                rockets.Add(player.TryShoot());
            }

            
            foreach (var rocket in rockets)
            {
                rocket.Move();
                rocket.Draw();
            }

            rockets.RemoveAll(r => r.IsOutOfBounds(gameWindowY));

            frameCounter++;
            if (frameCounter >= spawnInterval)
            {
                enemies.Add(new Enemy(gameWindowX));

                frameCounter = 0;
                spawnInterval = random.Next(3, 11) * FPS;
            }

            foreach (var enemy in enemies)
            {
                enemy.Move();
                enemy.Draw();
            }
            
            foreach (var rocket in rockets.ToList())
            {
                foreach (var enemy in enemies.ToList())
                {
                    if (R.CheckCollisionRecs(rocket.GetRectangle(), enemy.GetRectangle()))
                    {
                        rockets.Remove(rocket);
                        enemies.Remove(enemy);
                    }
                }
            }


            R.EndDrawing();
        }
        
        R.CloseWindow();
    }
}

public class Player
{
    private int x;
    private int y;
    private int width = 50;
    private int height = 30;
    private int speed = 5;
    private int windowWidth;
    private int windowHeight;

    public Player(int gameWindowX, int gameWindowY)
    {
        x = gameWindowX / 2 - width / 2;
        y = gameWindowY - 2 * height;
        windowWidth = gameWindowX;
        windowHeight = gameWindowY;
    }

    public void Move()
    {
            if ((R.IsKeyDown(KeyboardKey.Right) || R.IsKeyDown(KeyboardKey.D)) && x < windowWidth - width)
            {
                x += speed;
            }

            if ((R.IsKeyDown(KeyboardKey.Left) || R.IsKeyDown(KeyboardKey.A)) && x > 0)
            {
                x -= speed;
            }

            if ((R.IsKeyDown(KeyboardKey.Up) || R.IsKeyDown(KeyboardKey.W)) && y > 0)
            {
                y -= speed;
            }

            if ((R.IsKeyDown(KeyboardKey.Down) || R.IsKeyDown(KeyboardKey.S)) && y < windowHeight - height)
            {
                y += speed;
            }
    }

    public void Draw()
    {
        R.DrawRectangle(x, y, width, height, Color.Green);
    }

    public Rocket TryShoot()
    {
            return new Rocket(this);
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public int GetWidth()
    {
        return width;
    }
}


public class Rocket
{
    private int x;
    private int y;
    private int width = 5;
    private int height = 10;
    private int speed = 10;

    // Konstruktor – nastaví počáteční souřadnice při vytvoření střely
    public Rocket(int startX, int startY)
    {
        x = startX;
        y = startY;
    }

    public Rocket(Player player)
    {
        x = player.GetX() + (player.GetWidth() / 2) - (width / 2); // Střela se objeví uprostřed hráče
        y = player.GetY();
    }

    // Pohyb střely směrem vzhůru
    public void Move()
    {
        y -= speed;
    }

    // Vykreslení střely na obrazovku
    public void Draw()
    {
        R.DrawRectangle(x, y, width, height, Color.Red);
    }

    public bool IsOutOfBounds(int gameWindowY)
    {
        return y < -height;
    }

    public Rectangle GetRectangle()
    {
        return new Rectangle(x, y, width, height);
    }
}

public class Enemy
{
    private int x;
    private int y;
    private int width = 20;
    private int height = 30;
    private int speed = 2;

    public Enemy(int gameWindowX)
    {
        Random random = new Random();
        x = random.Next(0, gameWindowX - width);
        y = -height;
    }

    public void Move()
    {
        y += speed;
    }

    public void Draw()
    {
        R.DrawRectangle(x, y, width, height, Color.Red);
    }

    public Rectangle GetRectangle()
    {
        return new Rectangle(x, y, width, height);
    }

}
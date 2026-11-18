using R = Raylib_cs.Raylib;
using Raylib_cs;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata.Ecma335;

class GameWindow
{
    public static int FPS = 60;
    public static void Main()
    {
        int gameWindowX = 800;
        int gameWindowY = 600;

        R.InitWindow(gameWindowX, gameWindowY, "Space Invaders");
         
        R.SetTargetFPS(FPS);
        int spawnTimer = 0;
        Random random = new Random();
        int spawnInterval = random.Next(2, 6) * FPS;

        Player player = new Player(gameWindowX, gameWindowY);
        List<Rocket> rockets = new List<Rocket>();
        List<Enemy> enemies = new List<Enemy>();
        List<Rocket> enemyRockets = new List<Rocket>();

        while (!R.WindowShouldClose())
        {
            R.BeginDrawing();
            R.ClearBackground(Color.Black);
            
            player.Move();
            player.Draw();
            if (R.IsKeyPressed(KeyboardKey.Space) || R.IsMouseButtonPressed(MouseButton.Left))
            {
                rockets.Add(player.Shoot());
            }

            spawnTimer++;
            if (spawnTimer >= spawnInterval)
            {
                enemies.Add(new Enemy(gameWindowX));

                spawnTimer = 0;
                spawnInterval = random.Next(3, 11) * FPS;
            }

            foreach (var enemy in enemies)
            {
                enemy.Move();
                enemy.Draw();
                Rocket? newRocket = enemy.TryShoot();
                if (newRocket != null)
                {
                    enemyRockets.Add(newRocket);
                }
            }
            foreach (var rocket in enemyRockets)
            {
                rocket.Move();
                rocket.Draw();
            }

            foreach (var rocket in rockets)
            {
                rocket.Move();
                rocket.Draw();
            }

            rockets.RemoveAll(r => r.IsUpOutOfBounds(gameWindowY));
            enemyRockets.RemoveAll(r => r.IsDownOutOfBounds(gameWindowY));
            
            foreach (var enemyRocket in enemyRockets.ToList())
            {
                foreach (var playerRocket in rockets.ToList())
                {
                    if (R.CheckCollisionRecs(playerRocket.GetRectangle(), enemyRocket.GetRectangle()))
                    {
                        enemyRockets.Remove(enemyRocket);
                        rockets.Remove(playerRocket);
                    }                    
                }
            }

            foreach (var playerRocket in rockets.ToList())
            {
                foreach (var enemy in enemies.ToList())
                {
                    if (R.CheckCollisionRecs(playerRocket.GetRectangle(), enemy.GetRectangle()))
                    {
                        rockets.Remove(playerRocket);
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

    public Rocket Shoot()
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
    private int speed;

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
        speed = -10;
    }

    public Rocket(Enemy enemy)
    {
        x = enemy.GetX() + (enemy.GetWidth() / 2) - (width / 2); // Střela se objeví uprostřed nepřítele
        y = enemy.GetY();
        speed = 7;
    }

    // Pohyb střely směrem vzhůru
    public void Move()
    {
        y += speed;
    }

    // Vykreslení střely na obrazovku
    public void Draw()
    {
        R.DrawRectangle(x, y, width, height, Color.Red);
    }

    public bool IsUpOutOfBounds(int gameWindowY)
    {
        return y < -height;
    }

    public bool IsDownOutOfBounds(int gameWindowY)
    {
        return y > gameWindowY;
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
    private int shootTimer = 0;
    private float shootInterval;
    Random random = new Random();

    public Enemy(int gameWindowX)
    {
        x = random.Next(0, gameWindowX - width);
        y = -height;
        shootInterval = random.Next(1, 3) * GameWindow.FPS;
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

    public Rocket? TryShoot()
    {
        shootTimer ++;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0;
            shootInterval = random.Next(1, 3) * GameWindow.FPS;
            return new Rocket(this);
        }
        return null;
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
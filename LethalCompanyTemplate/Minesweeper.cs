using System;
using UnityEngine;
using Random = System.Random;

namespace LCTerminalGames;

public class Minesweeper
{
    private static int width = 15;
    private static int height = 15;
    private static int numMines = 25;
    private static string[,] board = new string[width, height];
    private static bool[,] revealed = new bool[width, height];
    private static bool[,] flags = new bool[width, height];
    private static int currentX = 0;
    private static int currentY = 0;
    private static int flagsPlaced = 0;
    private static bool gameOver = false;

    public static void RunGame(Action<string> setScreen)
    {
        RenderGameState(setScreen);
    }

    public static void StartGame()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                board[x, y] = " ";
                revealed[x, y] = false;
                flags[x, y] = false;
            }
        }

        // Place mines randomly
        Random random = new Random();
        int minesPlaced = 0;
        while (minesPlaced < numMines)
        {
            int x = random.Next(width);
            int y = random.Next(height);
            if (board[x, y] != "*")
            {
                board[x, y] = "*";
                minesPlaced++;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int near = 0;
                for (int xi = -1; xi <= 1; xi++)
                {
                    for (int yi = -1; yi <= 1; yi++)
                    {
                        if (x + xi >= 0 && x + xi < width && y + yi >= 0 && y + yi < height)
                        {
                            if (board[x + xi, y + yi] == "*")
                            {
                                near++;
                            }
                        }
                    }
                }

                if (board[x, y] != "*")
                {
                    if (near != 0)
                    {
                        board[x, y] = $"{near}";
                    }
                }
            }
        }
    }

    private static void RenderGameState(Action<string> setScreen)
    {
        flagsPlaced = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (flags[x, y]) flagsPlaced++;
            }
        }

        string display =
            $"Total Mines: {numMines}\nFlags Remaining: {numMines - flagsPlaced}\nWASD to Select | Space to select | F to flag\n";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == currentX && y == currentY)
                {
                    display += ">";
                }
                else
                {
                    if (revealed[x, y])
                    {
                        int near = 0;
                        for (int xi = -1; xi <= 1; xi++)
                        {
                            for (int yi = -1; yi <= 1; yi++)
                            {
                                int neighborX = x + xi;
                                int neighborY = y + yi;

                                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                                {
                                    if (board[neighborX, neighborY] == "*")
                                    {
                                        near++;
                                    }
                                }
                            }
                        }

                        display += near > 0 ? near.ToString() : " ";
                    }
                    else if (flags[x, y])
                    {
                        display += 'F';
                    }
                    else
                    {
                        display += '-';
                    }
                }

                display += " ";
            }

            display += "\n";
        }

        if (gameOver)
        {
            display += "\nGame Over! Press R to Restart.";
        }
        else if (CheckWin())
        {
            display += "\nYou Win! Press R to Restart.";
        }

        setScreen(display);
    }

    private static bool CheckWin()
    {
        // Win condition: all non-mine cells are revealed
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] != "*" && !revealed[x, y])
                    return false;
            }
        }

        return true;
    }

    private static void FloodFill(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && !revealed[x, y] && !flags[x, y])
        {
            revealed[x, y] = true;
            // If the cell is empty, recursively reveal its neighbors
            if (board[x, y] == " ")
            {
                for (int xi = -1; xi <= 1; xi++)
                {
                    for (int yi = -1; yi <= 1; yi++)
                    {
                        FloodFill(x + xi, y + yi);
                    }
                }
            }
        }
    }

    public static void HandleKey(KeyCode keyCode)
    {
        if (gameOver || CheckWin())
        {
            if (keyCode == KeyCode.R)
            {
                StartGame();
                gameOver = false;
            }

            return;
        }

        switch (keyCode)
        {
            case KeyCode.W:
            case KeyCode.UpArrow:
                currentY = Mathf.Max(0, currentY - 1);
                break;
            case KeyCode.S:
            case KeyCode.DownArrow:
                currentY = Mathf.Min(height - 1, currentY + 1);
                break;
            case KeyCode.A:
            case KeyCode.LeftArrow:
                currentX = Mathf.Max(0, currentX - 1);
                break;
            case KeyCode.D:
            case KeyCode.RightArrow:
                currentX = Mathf.Min(width - 1, currentX + 1);
                break;
            case KeyCode.F:
                if (!revealed[currentX, currentY] && flagsPlaced <= numMines)
                {
                    flags[currentX, currentY] = !flags[currentX, currentY];
                }

                break;
            case KeyCode.Return:
                if (!flags[currentX, currentY])
                {
                    if (board[currentX, currentY] == "*")
                    {
                        gameOver = true;
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                if (board[x, y] == "*")
                                {
                                    revealed[x, y] = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        FloodFill(currentX, currentY);
                    }
                }

                break;
        }
    }
}
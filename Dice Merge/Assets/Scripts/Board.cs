using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{
    public static Board instance;

    public int height;
    public int width;
    public float borderSize;
    public UI ui;
    public GameObject tilePrefab;
    public DiceSpwaner diceSpawaner;
    private Transform currDicePos;
    private Tile currTile;
    [HideInInspector] public Tile[,] tiles;

    private void Awake()
    {
        instance = this;
        tiles = new Tile[width, height];
        SetupTiles();
        SetCamera();
    }

    private void OnEnable()
    {
        Dice.OnItemPlaced += ItemAdded;
    }

    private void OnDesable()
    {
        Dice.OnItemPlaced -= ItemAdded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) ClearAllDice();
    }

    private void ItemAdded(Dice dice)
    {
        currDicePos = dice.destinationPos;
        currTile = dice.tile;
        CheckMatches();
        CheckGameOver();
    }

    private void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject currTile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                currTile.name = "Tile (" + i + "," + j + ")";
                currTile.transform.SetParent(this.transform);
                tiles[i, j] = currTile.GetComponent<Tile>();
                tiles[i, j].Init(i, j, this);
            }
        }

        diceSpawaner.transform.position = tiles[2, 0].transform.position - new Vector3(0, 2, 0);
    }

    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3((float)((width - 1) / 2), (float)((height - 1) / 2), -10);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)(height / 2) + borderSize;
        float horrizontalSize = ((float)(width / 2) + borderSize) / aspectRatio;

        Camera.main.orthographicSize = verticalSize > horrizontalSize ? verticalSize : horrizontalSize;
    }

    private bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private List<Dice> FindMatches(int startX, int startY, Vector2 searchDirection)
    {
        List<Dice> matches = new List<Dice>();
        Dice startDice = null;

        if (IsWithinBounds(startX, startY))
        {
            startDice = tiles[startX, startY].dice;
        }

        if (startDice)
        {
            matches.Add(startDice);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = width > height ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            Dice newDice = tiles[nextX, nextY].dice;

            if (newDice && newDice.diceNo != 7 && newDice.diceNo == startDice.diceNo && !matches.Contains(newDice))
            {
                matches.Add(newDice);
            }
            else
            {
                break;
            }
        }

        return matches.Count >= 3 ? matches : null;
    }

    private List<Dice> FindVerticalMatches(int startX, int startY)
    {
        List<Dice> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1));
        List<Dice> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1));

        if (upwardMatches == null) upwardMatches = new List<Dice>();
        if (downwardMatches == null) downwardMatches = new List<Dice>();

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        return combinedMatches.Count >= 3 ? combinedMatches : null;
    }

    private List<Dice> FindHorrizontalMatches(int startX, int startY)
    {
        List<Dice> rightMatches = FindMatches(startX, startY, new Vector2(1, 0));
        List<Dice> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0));

        if (rightMatches == null) rightMatches = new List<Dice>();
        if (leftMatches == null) leftMatches = new List<Dice>();

        var combinedMatches = rightMatches.Union(leftMatches).ToList();
        return combinedMatches.Count >= 3 ? combinedMatches : null;
    }

    private List<Dice> FindMatchesAt(int x, int y)
    {
        List<Dice> horrizontalMatches = FindVerticalMatches(x, y);
        List<Dice> verticalMatches = FindHorrizontalMatches(x, y);

        if (horrizontalMatches == null) horrizontalMatches = new List<Dice>();
        if (verticalMatches == null) verticalMatches = new List<Dice>();

        var combinedMatches = horrizontalMatches.Union(verticalMatches).ToList();
        return combinedMatches;
    }

    private void CheckMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var combinedMatches = FindMatchesAt(i, j);

                if (combinedMatches.Count > 0)
                {
                    // foreach (Dice dice in combinedMatches)
                    // {
                    //     SpriteRenderer spriteRenderer = dice.spriteRenderer;
                    //     spriteRenderer.color = Color.black;
                    // }
                    ClearDiceAt(combinedMatches);
                    ui.UpdateScore(combinedMatches[0].diceNo);

                    Dice newDice = Instantiate(diceSpawaner.dices[combinedMatches[0].diceNo], transform.position, Quaternion.identity).GetComponent<Dice>();
                    newDice.tile = currTile;
                    newDice.transform.SetParent(diceSpawaner.transform);
                    newDice.destinationPos = currDicePos;
                    newDice.PlaceDice();
                    newDice.isItemPlaced = true;
                }
            }
        }
    }

    private void ClearDiceAt(int x, int y)
    {
        Dice dice = tiles[x, y].dice;

        if (dice)
        {
            tiles[x, y].dice = null;
            Destroy(dice.gameObject);
        }
    }

    private void ClearDiceAt(List<Dice> dices)
    {
        foreach (Dice dice in dices)
        {
            ClearDiceAt(dice.x, dice.y);
        }
    }

    private void CheckGameOver()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(tiles[i,j].dice == null)
                {
                    return;
                }
            }
        }

        ui.GameOver();
    }

    private void ClearAllDice()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearDiceAt(i, j);
            }
        }
    }
}
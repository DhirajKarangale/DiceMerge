using System;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public static Action<Dice> OnItemPlaced;
    public static Action<Dice> OnItemRemoved;
    public int diceNo;
    private BoxCollider2D boxCollider;
    [HideInInspector] public Transform destinationPos;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    [HideInInspector] public int x;
    [HideInInspector] public int y;

    [HideInInspector] public Tile tile;
    private Board board;
    private Transform diceSpwaner;
    private Vector3 resetPos;
    private float startPosX;
    private float startPosY;
    private bool isMoving;
    public bool isItemPlaced;

    private void Start()
    {
        diceSpwaner = DiceSpwaner.instance.transform;
        board = Board.instance;
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        resetPos = transform.position;
    }

    private void Update()
    {
        if (!isItemPlaced && isMoving)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.gameObject.transform.position = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, this.gameObject.transform.position.z);
        }
    }

    private void OnMouseDown()
    {
        if (!isItemPlaced && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startPosX = mousePos.x - this.transform.position.x;
            startPosY = mousePos.y - this.transform.position.y;
            OnItemRemoved?.Invoke(this);
            isMoving = true;

            ItemHold();
        }
    }

    private void OnMouseUp()
    {
        if (isItemPlaced) return;
        isMoving = false;
        ItemRelease();
        destinationPos = FindDestination();
        PlaceDice();
    }

    public void PlaceDice()
    {
        if (destinationPos && tile)
        {
            this.transform.position = destinationPos.position;
            if (destinationPos != diceSpwaner)
            {
                tile.PlaceDice(this);
                isItemPlaced = true;
                OnItemPlaced?.Invoke(this);
            }
        }
        else
        {
            this.transform.position = resetPos;
            OnItemRemoved?.Invoke(this);
        }
    }

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private Transform FindDestination()
    {
        float dist = float.MaxValue;
        Transform pos = this.transform;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.tiles[i, j].dice) continue;
                float currDist = Vector2.Distance(transform.position, board.tiles[i, j].transform.position);
                if (dist > currDist)
                {
                    tile = board.tiles[i, j];
                    dist = currDist;
                    pos = board.tiles[i, j].transform;
                }
            }
        }

        if (Vector2.Distance(transform.position, diceSpwaner.transform.position) < 1.5f) return diceSpwaner;

        return pos;
    }

    private void ItemHold()
    {
        transform.localScale = (1.3f) * Vector3.one;
        Color holdColor = spriteRenderer.color;
        holdColor.a = 0.8f;
        spriteRenderer.color = holdColor;
    }

    private void ItemRelease()
    {
        transform.localScale = (1f) * Vector3.one;
        Color holdColor = spriteRenderer.color;
        holdColor.a = 1f;
        spriteRenderer.color = holdColor;
    }
}

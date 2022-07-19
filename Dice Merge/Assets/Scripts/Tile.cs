using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public Dice dice;
    private Board board;

    private void OnEnable()
    {
        Dice.OnItemPlaced += ItemAdded;
        Dice.OnItemRemoved += ItemRemoved;
    }

    private void OnDesable()
    {
        Dice.OnItemPlaced -= ItemAdded;
        Dice.OnItemRemoved -= ItemRemoved;
    }

    public void Init(int x, int y, Board board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
    }

    public void PlaceDice(Dice dice)
    {
        this.dice = dice;
        dice.Init(x, y);
    }

    private void ItemAdded(Dice item)
    {

    }

    private void ItemRemoved(Dice item)
    {

    }
}

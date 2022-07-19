using UnityEngine;

public class DiceSpwaner : MonoBehaviour
{
    public static DiceSpwaner instance;

    public GameObject[] dices;
    private GameObject currDice;
    private bool isDiceRemove;

    private void Awake()
    {
        instance = this;
        SpwanDice();
    }

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

    private void ItemAdded(Dice dice)
    {
        if (dice.transform.name == currDice.transform.name && isDiceRemove)
        {
            SpwanDice();
        }
    }

    private void ItemRemoved(Dice dice)
    {
        isDiceRemove = dice.transform.name == currDice.transform.name;
    }

    private void SpwanDice()
    {
        currDice = Instantiate(dices[Random.Range(0, dices.Length - 1)], transform.position, Quaternion.identity);
        currDice.transform.SetParent(this.transform);
    }
}
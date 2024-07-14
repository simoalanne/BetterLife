using UnityEngine;

[CreateAssetMenu(fileName = "NewLoan", menuName = "ScriptableObjects/Loan", order = 1)]
public class Loan : InventoryItem
{
    [Header("Loan settings")]
    public int loanAmount;
    public int daysToRepay;
    [Range(0, 100)]
    public int interestRate;
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewLoan", menuName = "ScriptableObjects/Loan", order = 1)]
public class Loan : ScriptableObject
{
    [Header("Loan settings")]
    public int loanAmount;
    public int daysToRepay;
    [Range(0, 100)]
    public int interestRate;
    [HideInInspector]
    public int ActualAmount => (int)(loanAmount * (1 + ((float)interestRate / 100)));
}

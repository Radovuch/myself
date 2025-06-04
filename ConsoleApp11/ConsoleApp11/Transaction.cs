namespace BudgetApp.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public TransactionType Type { get; set; }

        public override string ToString()
        {
            return $"{Date.ToShortDateString()} | {Type} | {Category} | {Amount} грн";
        }
    }
}
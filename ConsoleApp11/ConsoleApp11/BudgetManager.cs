using BudgetApp.Models;

namespace BudgetApp.Services
{
    public class BudgetManager
    {
        public List<Transaction> Transactions { get; set; } = new();

        public void AddTransaction(Transaction t)
        {
            Transactions.Add(t);
        }

        public decimal GetBalance()
        {
            return Transactions.Sum(t => t.Type == TransactionType.Доход ? t.Amount : -t.Amount);
        }

        public void PrintAllTransactions()
        {
            foreach (var t in Transactions)
                Console.WriteLine(t);
        }

        public void PrintMonthlyReport(int month)
        {
            var filtered = Transactions.Where(t => t.Date.Month == month).ToList();
            if (filtered.Count == 0)
            {
                Console.WriteLine("Немає транзакцій за цей місяць.");
                return;
            }

            foreach (var t in filtered)
                Console.WriteLine(t);

            var total = filtered.Sum(t => t.Type == TransactionType.Доход ? t.Amount : -t.Amount);
            Console.WriteLine($"\nБаланс за місяць: {total} грн");
        }

        public void ChangeCategory(int transactionIndex, string newCategory)
        {
            if (transactionIndex >= 0 && transactionIndex < Transactions.Count)
            {
                Transactions[transactionIndex].Category = newCategory;
            }
            else
            {
                Console.WriteLine("Невірний номер транзакії.");
            }
        }
        
        
    }
}
using BudgetApp.Models;
using BudgetApp.Services;
using System;
using System.Linq;

namespace BudgetApp
{
    public class ConsoleApp
    {
        private readonly BudgetManager manager;
        private readonly FileService<Transaction> fileService;
        private readonly string filePath = "transactions.json";

        public ConsoleApp()
        {
            manager = new BudgetManager();
            fileService = new FileService<Transaction>();
            manager.Transactions = fileService.Load(filePath);
        }

        public void Run()
        {
            while (true)
            {
                ShowMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTransaction();
                        break;
                    case "2":
                        manager.PrintAllTransactions();
                        Pause();
                        break;
                    case "3":
                        ReportMenu();
                        break;
                    case "4":
                        Console.WriteLine($"Поточний баланс: {manager.GetBalance()} грн");
                        Pause();
                        break;
                    case "5":
                        fileService.Save(filePath, manager.Transactions);
                        Console.WriteLine("Дані збережено. Вихід...");
                        return;
                    case "6":
                        ShowTopExpenseCategories();
                        Pause();
                        break;
                    case "7":
                        RemoveTransaction();
                        Pause();
                        break;
                    default:
                        Console.WriteLine("Невідома опція.");
                        Pause();
                        break;
                }
            }
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("======= Домашня бухгалтерія =======");
            Console.WriteLine($"Дата: {DateTime.Now:dd.MM.yyyy}  Час: {DateTime.Now:HH:mm}");
            Console.WriteLine($"Баланс: {manager.GetBalance()} грн");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("1. Додати транзакцію");
            Console.WriteLine("2. Всі транзакції");
            Console.WriteLine("3. Звіт (місяць/дата)");
            Console.WriteLine("4. Баланс");
            Console.WriteLine("5. Зберегти і вийти");
            Console.WriteLine("6. Топ-3 витрати");
            Console.WriteLine("7. Видалити транзакцію");
            Console.WriteLine("-----------------------------------");
            Console.Write("Вибір: ");
        }

        private void AddTransaction()
        {
            try
            {
                Console.WriteLine("Оберіть тип транзакції:");
                Console.WriteLine("1. Зарплата");
                Console.WriteLine("2. Продукти");
                Console.WriteLine("3. Транспорт");
                Console.WriteLine("4. Ввести власну категорію");

                Console.Write("Ваш вибір: ");
                string catChoice = Console.ReadLine();

                string category = catChoice switch
                {
                    "1" => "Зарплата",
                    "2" => "Продукти",
                    "3" => "Транспорт",
                    "4" => ReadCustomCategory(),
                    _ => "Інше"
                };

                Console.Write("Сума: ");
                decimal amount = decimal.Parse(Console.ReadLine() ?? "0");

                Console.Write("Тип (0 - Доход, 1 - Витрата): ");
                TransactionType type = (TransactionType)int.Parse(Console.ReadLine() ?? "1");

                manager.AddTransaction(new Transaction
                {
                    Amount = amount,
                    Category = category,
                    Type = type,
                    Date = DateTime.Now
                });

                Console.WriteLine("Транзакцію додано! 🎉");
            }
            catch
            {
                Console.WriteLine("Помилка введення. Спробуй ще раз.");
            }

            Pause();
        }

        private string ReadCustomCategory()
        {
            Console.Write("Введіть свою категорію: ");
            return Console.ReadLine() ?? "Інше";
        }

        private void ReportMenu()
        {
            Console.WriteLine("Виберіть варіант:");
            Console.WriteLine("1. Звіт за місяць");
            Console.WriteLine("2. Звіт за конкретну дату");
            Console.Write("Ваш вибір: ");
            string reportChoice = Console.ReadLine();
            if (reportChoice == "1")
            {
                Console.Write("Введіть номер місяця (1-12): ");
                if (int.TryParse(Console.ReadLine(), out int month))
                    manager.PrintMonthlyReport(month);
                else
                    Console.WriteLine("Невірний номер місяця.");
            }
            else if (reportChoice == "2")
            {
                Console.Write("Введіть дату (РРРР-ММ-ДД): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    var filtered = manager.Transactions
                        .Where(t => t.Date.Date == date.Date)
                        .ToList();
                    if (filtered.Count == 0)
                        Console.WriteLine("За цю дату транзакцій не знайдено.");
                    else
                        foreach (var t in filtered)
                            Console.WriteLine(t);
                }
                else
                    Console.WriteLine("Неправильний формат дати.");
            }
            else
            {
                Console.WriteLine("Невірний вибір.");
            }
            Pause();
        }

        private void ShowTopExpenseCategories()
        {
            var topCategories = manager.Transactions
                .Where(t => t.Type == TransactionType.Витрата)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(g => g.Total)
                .Take(3);

            Console.WriteLine("Топ-3 категорії витрат:");
            foreach (var cat in topCategories)
                Console.WriteLine($"{cat.Category} — {cat.Total} грн");
        }

        private void RemoveTransaction()
        {
            manager.PrintAllTransactions();
            Console.Write("Введи номер транзакції для видалення (починаючи з 1): ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= manager.Transactions.Count)
            {
                manager.Transactions.RemoveAt(idx - 1);
                Console.WriteLine("Транзакцію видалено.");
            }
            else
                Console.WriteLine("Невірний номер.");
        }

        private void Pause()
        {
            Console.WriteLine("Натисни Enter щоб продовжити...");
            Console.ReadLine();
        }
    }
}

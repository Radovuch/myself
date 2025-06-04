using System.Text.Json;
using BudgetApp.Interfaces;

namespace BudgetApp.Services
{
    public class FileService<T> : IStorable<T>
    {
        public void Save(string filePath, List<T> data)
        {
            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }

        public List<T> Load(string filePath)
        {
            if (!File.Exists(filePath)) return new List<T>();
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}
namespace BudgetApp.Interfaces
{
    public interface IStorable<T>
    {
        void Save(string filePath, List<T> data);
        List<T> Load(string filePath);
    }
}
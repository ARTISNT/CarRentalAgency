using System.Reflection;

namespace CarService.Domain.Common;

public abstract class Enumeration
{
    public string Name { get; private set; }

    public int Id { get; private set; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public static T FromName<T>(string name) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var matchingItem = GetAll<T>().FirstOrDefault(item =>
            string.Equals(item.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));

        if (matchingItem == null)
            throw new InvalidOperationException($"'{name}' is not a valid name in {typeof(T).Name}");

        return matchingItem;
    }
    
    public static T FromValue<T>(int id) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Id == id);

        if (matchingItem == null)
            throw new InvalidOperationException($"{id} is not a valid id in {typeof(T)}");

        return matchingItem;
    } 
    
    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    
    public static bool operator ==(Enumeration left, Enumeration right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(Enumeration left, Enumeration right)
    {
        return !(left == right);
    } 
}

using System.Reflection;

namespace Infrastructure;

public class ValueType<T>
{
    private static PropertyInfo[] _properties;
    private static Type _type;

    static ValueType()
    {
        _type = typeof(T);
        _properties = _type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is T typedObj && Equals(typedObj);
    }

    public bool Equals(T obj) => obj is not null 
                                 && _properties.All(t => Equals(t.GetValue(this), t.GetValue(obj)));

    public override int GetHashCode()
    {
        var hash = (_properties[0].GetValue(this).GetHashCode() + 45) * 52;
        for(var i = 1; i < _properties.Length; i++)
            hash *= _properties[i].GetValue(this).GetHashCode() + (int)Math.Pow(2, i);
        return hash;
    }

    public override string ToString() => 
        $"{_type.Name}({string.Join("; ", _properties.OrderBy(x => x.Name).Select(p => $"{p.Name}: {p.GetValue(this)}"))})";
}
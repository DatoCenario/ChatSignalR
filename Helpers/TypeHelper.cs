using System.Linq.Expressions;
using System.Reflection;

namespace WebApplication2.Helpers;

public static class TypeHelper
{
    public static Dictionary<Type, ICollection<object>> CreateFlattenTypeArray(ICollection<object> objects,
        bool distinctById = false)
    {
        var result = new Dictionary<Type, ICollection<object>>();
        var queue = new Queue<object>(objects);

        while (queue.Count > 0)
        {
            var obj = queue.Dequeue();
            var type = obj.GetType();
            if (!result.ContainsKey(type))
            {
                result[type] = new List<object>();
            }

            if (distinctById)
            {
                var idProperty = type.GetProperty("Id");
                if (idProperty == null)
                {
                    throw new Exception($"Type {type.Name} does not have an 'Id' property");
                }

                if (result[type].All(o => idProperty.GetValue(o) != idProperty.GetValue(obj)))
                {
                    result[type].Add(obj);
                }
            }
            else
            {
                result[type].Add(obj);
            }

            var properties = type.GetProperties()
                .Where(p => typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType));
            foreach (var property in properties)
            {
                var childObjects = (IEnumerable<object>)property.GetValue(obj);
                if (childObjects != null)
                {
                    foreach (var childObject in childObjects)
                    {
                        queue.Enqueue(childObject);
                    }
                }
            }
        }

        return result;
    }

    public static EntitiyDiff<T> GetDiff<T>(T entity1, T entity2)
    {
        var diffsByTypes = GetDiffsByTypes(entity1, entity2);
        
        // Fill PropertyDiffs for IEnumerable and class-properties (exclude string)
        // match entities by Id property
        // if entities matched by Id - get diff from diffsByTypes and use as PropertyDiff
        // if new entity is null - ChangeType is Delete
        // if old entity is null - ChangeType is Add
        // if entity removed from IEnumerable - ChangeType is Delete
        // if entity added to IEnumerable - ChangeType is Add

        return new EntitiyDiff<T>(diffsByTypes[typeof(T)][(int)entity1.GetType().GetProperty("Id").GetValue(entity1)]);
    }

    public static Dictionary<Type, Dictionary<int, EntitiyDiff>> GetDiffsByTypes<T>(T entity1, T entity2)
    {
        var result = new Dictionary<Type, Dictionary<int, EntitiyDiff>>();

        var entity1Flatten = TypeHelper.CreateFlattenTypeArray(new[] { (object)entity1 }, true);
        var entity2Flatten = TypeHelper.CreateFlattenTypeArray(new[] { (object)entity2 }, true);

        foreach (var type in entity1Flatten.Keys)
        {
            var type1Objects = entity1Flatten[type];
            var type2Objects = entity2Flatten.ContainsKey(type) ? entity2Flatten[type] : new List<object>();
            var idProperty = type.GetProperty("Id");

            var visitedType2 = new HashSet<object>();
            foreach (var obj1 in type1Objects)
            {
                var obj1Id = idProperty.GetValue(obj1);
                var obj2 = type2Objects.FirstOrDefault(o => idProperty.GetValue(o).Equals(obj1Id));

                if (obj2 == null)
                {
                    AddDiff(type, new EntitiyDiff
                    {
                        ChangeType = ChangeType.Delete,
                        PropertyDiffs = GetPropertyDiffs(obj1, null, type),
                        EntityNew = obj2,
                        EntityOld = obj1,
                        EntityId = (int)obj1Id,
                        EntityType = type
                    });
                }
                else
                {
                    visitedType2.Add(obj2);
                    var propDiffs = GetPropertyDiffs(obj1, obj2, type);
                    AddDiff(type, new EntitiyDiff
                    {
                        ChangeType = propDiffs.Any(d => d.HaveDiff) ? ChangeType.Edit : ChangeType.None,
                        PropertyDiffs = propDiffs,
                        EntityNew = obj2,
                        EntityOld = obj1,
                        EntityId = (int)obj1Id,
                        EntityType = type
                    });
                }
            }

            foreach (var obj2 in type2Objects.Where(e => !visitedType2.Contains(e)))
            {
                var obj2Id = idProperty.GetValue(obj2);
                AddDiff(type, new EntitiyDiff
                {
                    ChangeType = ChangeType.Add,
                    PropertyDiffs = GetPropertyDiffs(null, obj2, type),
                    EntityNew = obj2,
                    EntityOld = null,
                    EntityId = (int)obj2Id,
                    EntityType = type
                });
            }
        }

        void AddDiff(Type type, EntitiyDiff diff)
        {
            if (!result.TryGetValue(type, out var diffs))
            {
                diffs = new Dictionary<int, EntitiyDiff>();
                result[type] = diffs;
            }

            if (!diffs.TryGetValue(diff.EntityId, out var diffD))
            {
                diffs.Add(diff.EntityId, diff);
            }
        }

        return result;
    }

    private static ICollection<PropertyDiff> GetPropertyDiffs(object? obj1, object? obj2, Type type)
    {
        var result = new List<PropertyDiff>();

        foreach (var property in type.GetProperties()
                     .Where(p => p.PropertyType.IsPrimitive || p.PropertyType == typeof(string))
                     .Where(p => p.Name != "Id"))
        {
            var propertyValue1 = obj1 == null ? null : property.GetValue(obj1);
            var propertyValue2 = obj2 == null ? null : property.GetValue(obj2);

            var diff = new PropertyDiff
            {
                PropertyInfo = property,
                OldValue = propertyValue1,
                NewValue = propertyValue2,
                HaveDiff = true
            };

            if ((propertyValue1 == null && propertyValue2 == null) || (propertyValue1 != null &&
                                                                       propertyValue2 != null &&
                                                                       propertyValue1.Equals(propertyValue2)))
            {
                diff.HaveDiff = false;
            }

            result.Add(diff);
        }

        return result;
    }
}

public class EntitiyDiff<T>
{
    private EntitiyDiff _entitiyDiff;

    public EntitiyDiff(EntitiyDiff entitiyDiff)
    {
        _entitiyDiff = entitiyDiff;
    }

    public Type EntityType => _entitiyDiff.EntityType;
    public long EntityId => _entitiyDiff.EntityId;
    public ICollection<PropertyDiff> PropertyDiffs => _entitiyDiff.PropertyDiffs;
    public ChangeType ChangeType => _entitiyDiff.ChangeType;
    public T EntityOld => (T)_entitiyDiff.EntityOld;
    public T EntityNew => (T)_entitiyDiff.EntityNew;

    public PropertyDiff? GetPropertyDiff<TProp>(Expression<Func<T, TProp>> propertySelector)
    {
        var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
        return PropertyDiffs.FirstOrDefault(p => p.PropertyInfo.Name == propertyName);
    }

    public ICollection<EntitiyDiff<TEntity>> GetEntityDiffs<TEntity>(
        Expression<Func<T, ICollection<TEntity>>> propertySelector)
    {
        var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
        var propertyDiff = PropertyDiffs.FirstOrDefault(p => p.PropertyInfo.Name == propertyName);
        if (propertyDiff != null)
        {
            return (ICollection<EntitiyDiff<TEntity>>)propertyDiff.EntitiyDiffs;
        }

        return new List<EntitiyDiff<TEntity>>();
    }

    public EntitiyDiff<TEntity> GetEntityDiffs<TEntity>(Expression<Func<T, TEntity>> propertySelector)
    {
        var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
        var propertyDiff = PropertyDiffs.FirstOrDefault(p => p.PropertyInfo.Name == propertyName);

        var entityDiff = propertyDiff?.EntitiyDiffs?.FirstOrDefault();
        if (entityDiff != null)
        {
            return new EntitiyDiff<TEntity>(entityDiff);
        }

        return null;
    }
}

public class EntitiyDiff
{
    public Type EntityType { get; set; }
    public int EntityId { get; set; }
    public ICollection<PropertyDiff> PropertyDiffs { get; set; }
    public ChangeType ChangeType { get; set; }
    public object EntityOld { get; set; }
    public object EntityNew { get; set; }
}

public class PropertyDiff
{
    public bool HaveDiff { get; set; }
    public PropertyInfo PropertyInfo { get; set; }
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
    public ICollection<EntitiyDiff> EntitiyDiffs { get; set; }
}

public enum ChangeType : short
{
    Add = 0,
    Delete = 1,
    Edit = 2,
    None = 3
}
using InformationSystemHZS.Models;
using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Collections;

/// <summary>
/// Stores and manages data that maps valid callsigns to entities of a given type. 
/// </summary>
/// <typeparam name="T">IBaseModel</typeparam>
public class CallsignEntityMap<T> where T : IModel
{
    private Dictionary<string, T> _map;
    public readonly string CallsignIdentifier;
    public const int MaxPossibleCallsign = 100 - 1;
    public const int CallsignDigitPlaces = 2;

    public int MaxCallsign { get; private set; }

    public CallsignEntityMap(string identifier)
    {
        _map = new Dictionary<string, T>();
        CallsignIdentifier = identifier;
        MaxCallsign = 0;
    }

    /// <summary>
    /// Returns an entity based on the given callsign.
    /// If the entity does not exist then returns default (see: https://learn.microsoft.com/cs-cz/dotnet/csharp/language-reference/operators/default).
    /// </summary>
    public T? GetEntity(string callsign)
    {
        T? entity;
        if (_map.TryGetValue(callsign, out entity)) {
            return entity;
        }

        return default;
    }

    /// <summary>
    /// Returns all mambers of the map.
    /// </summary>
    public List<T> GetAllEntities()
    {
        List<T> listOfEntites = _map.Values.ToList();
        listOfEntites.Sort((x, y) => x.Callsign.CompareTo(y.Callsign));
        return listOfEntites;
    }
    
    /// <summary>
    /// Returns the total number of entities in the map.
    /// </summary>
    public int GetEntitiesCount()
    {
        return _map.Count;
    }
    
    /// <summary>
    /// Tries to safely add an entity. If callsign already exists within this map or is not in a valid format (i.e. S01, H01, J01, ...), returns false.
    /// Otherwise adds an entity to this map and returns true.
    /// If no callsign is provided, it generates a new one by incrementing the current highest callsign by 1 (i.e. generates S04, if highest available is S03).
    /// </summary>
    public bool SafelyAddEntity(T entity, string? callsign)
    {
        if (callsign != null && _map.ContainsKey(callsign))
        {
            return false;
        }

        if (callsign == null)
        {
            if (MaxCallsign == MaxPossibleCallsign)
            {
                return false;
            }

            MaxCallsign++;
            callsign = CallsignIdentifier + MaxCallsign.ToString($"D{CallsignDigitPlaces}");
        }

        else if (MaxCallsign < int.Parse(callsign.Substring(1)))
        {
            MaxCallsign = int.Parse(callsign.Substring(1)); // [IDENTIFIER] |-> [ID0] [ID1]
        }

        _map.Add(callsign, entity);
        return true;
    }

    /// <summary>
    /// Tries to safely remove an entity from this map. If it does not exist in this map, return false.
    /// If it exists, remove it from this map and return true.
    /// </summary>
    public bool SafelyRemoveEntity(string callsign)
    {
        if (!_map.ContainsKey(callsign))
        {
            return false;
        }

        _map.Remove(callsign);
        if (MaxCallsign == int.Parse(callsign.Substring(1)))
        {
            string? maxCallsign = _map.Keys.Max();
            if (maxCallsign == null)
            {
                MaxCallsign = 0;
            }
            else
            {
                MaxCallsign = int.Parse(maxCallsign.Substring(1));
            }
        }

        return true;
    }

    public string GetMaxCallsign()
    {
        return CallsignIdentifier + MaxCallsign.ToString($"D{CallsignDigitPlaces}");
    }
}

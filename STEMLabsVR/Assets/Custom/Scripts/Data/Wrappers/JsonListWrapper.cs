using System.Collections.Generic;

namespace Custom.Scripts.Data.Wrappers
{
    // Wrapper class for JSON serialization of a list of items since only classes and structs can be serialized directly
    // with Unity's JsonUtility
    public class JsonListWrapper<T>
    {
        public List<T> items;
    }
}
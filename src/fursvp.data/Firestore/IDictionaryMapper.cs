using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.data.Firestore
{
    public interface IDictionaryMapper<T>
    {
        Dictionary<string, object> ToDictionary(T @object);
        T FromDictionary(Dictionary<string, object> dictionary);
    }
}

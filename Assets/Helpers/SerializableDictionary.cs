using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        this.keys.Clear();
        this.values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            this.keys.Add(pair.Key);
            this.values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < this.keys.Count; i++)
        {
            this.Add(this.keys[i], this.values[i]);
        }
    }

}

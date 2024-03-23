// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.Serialization;
//
// [Serializable]
// public class ReadableDictionaryItem<TItem1, TItem2>
// {
//     public TItem1 TKey;
//     public TItem2 TValue;
// }
//
// [Serializable]
// public class ReadableDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, ICollection, IDictionary, IEnumerable, ISerializable
// {
//     public ReadableDictionaryItem<TKey, TValue> dictionaryItems;
//
//     public Dictionary<TKey, TValue> readableDictionary;
//     private ICollection<TKey> keys;
//     private ICollection<TValue> values;
//     private ICollection keys1;
//     private ICollection values1;
//
//     public bool TryAdd<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value) => throw null;
//     public void Add(TKey key, TValue value) { }
//
//     public bool ContainsKey(TKey key)
//     {
//         throw null!;
//     }
//
//     public bool Remove(TKey key)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryGetValue(TKey key, out TValue value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public TValue this[TKey key]
//     {
//         get => throw new NotImplementedException();
//         set => throw new NotImplementedException();
//     }
//
//     ICollection<TKey> IDictionary<TKey, TValue>.Keys => keys;
//
//     ICollection IDictionary.Values => values1;
//
//     ICollection IDictionary.Keys => keys1;
//
//     ICollection<TValue> IDictionary<TKey, TValue>.Values => values;
//
//     public bool Remove<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, out TValue value) => throw null!;
//     public bool Contains(object key)
//     {
//         throw new NotImplementedException();
//     }
//
//     IDictionaryEnumerator IDictionary.GetEnumerator()
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Remove(object key)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool IsFixedSize { get; }
//
//     public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
//     {
//         throw new NotImplementedException();
//     }
//
//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return GetEnumerator();
//     }
//
//     public void Add(KeyValuePair<TKey, TValue> item)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Add(object key, object value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Clear() { }
//
//     public bool Contains(KeyValuePair<TKey, TValue> item)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool Remove(KeyValuePair<TKey, TValue> item)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void CopyTo(Array array, int index)
//     {
//         throw new NotImplementedException();
//     }
//
//     public int Count { get; }
//     public bool IsSynchronized { get; }
//     public object SyncRoot { get; }
//     public bool IsReadOnly { get; }
//
//     public object this[object key]
//     {
//         get => throw new NotImplementedException();
//         set => throw new NotImplementedException();
//     }
//
//     public void GetObjectData(SerializationInfo info, StreamingContext context)
//     {
//         throw new NotImplementedException();
//     }
// }
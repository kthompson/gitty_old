using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gitty
{

    /// <summary>
    /// Basic implementation of the NestedDictionaryBase using an underlying Dictionary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public sealed class NestedDictionary<K, V> : NestedDictionaryBase<K, V, NestedDictionary<K, V>>
    {

        #region Operator Overloads 
        
        public static implicit operator NestedDictionary<K, V>(V value)
        {
            NestedDictionary<K, V> d = new NestedDictionary<K, V>();
            d.Value = value;
            return d;
        }

        public static bool operator true(NestedDictionary<K, V> value)
        {
            return !(value.Value.Equals(default(V)));
        }

        public static bool operator false(NestedDictionary<K, V> value)
        {
            return (value.Value.Equals(default(V)));
        }

        public static bool operator !(NestedDictionary<K, V> value)
        {
            if (value)
                return false;
            else
                return true;
        }

        #endregion

    }

    /// <summary>
    /// Basic implementation of the NestedDictionaryBase using an underlying SortedDictionary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public sealed class NestedSortedDictionary<K, V> : NestedDictionaryBase<K, V, NestedSortedDictionary<K, V>>
    {

        #region Operator Overloads 

        public static implicit operator NestedSortedDictionary<K, V>(V value)
        {
            NestedSortedDictionary<K, V> d = new NestedSortedDictionary<K, V>();
            d.Value = value;
            return d;
        }

        #endregion

        #region Protected Methods 

        protected override IDictionary<K, NestedSortedDictionary<K, V>> CreateDictionary()
        {
            return new SortedDictionary<K, NestedSortedDictionary<K, V>>();
        }

        #endregion

    }

    /// <summary>
    /// Base class used for a nested dictionary
    /// NOTE: You should overload the implicit operator for converting V to your class for best functionality
    /// </summary>
    /// <typeparam name="K">Key Type</typeparam>
    /// <typeparam name="V">Value Type</typeparam>
    /// <typeparam name="D">Nested Dictionary Type (Typically inherits from NestedDictionaryBase)</typeparam>
    public abstract class NestedDictionaryBase<K, V, D> : IDictionary<K, D>, IXmlSerializable
        where D : NestedDictionaryBase<K, V, D>, new()
    {

        #region Constructors 

        public NestedDictionaryBase()
        {
        }

        public NestedDictionaryBase(V value)
        {
            this._value = value;
        }

        #endregion

        #region Operator Overloads 

        public static implicit operator NestedDictionaryBase<K, V, D>(V value)
        {
            D d = new D();
            d.Value = value;
            return d;
        }

        public static explicit operator V(NestedDictionaryBase<K, V, D> d)
        {
            return d.As<V>();
        }  

        #endregion

        #region Protected Methods 

        protected virtual IDictionary<K, D> CreateDictionary()
        {
            return new Dictionary<K, D>();
        }

        #endregion

        #region Public Methods 

        public virtual T As<T>()
        {
            if (!(_value is T))
                throw new InvalidOperationException(String.Format("Object is not of type {0}", typeof(T).ToString()));

            return (T)(object)_value;
        }

        public virtual D Add(K key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            D value = new D();
            ((IDictionary<K, D>)this).Add(key, value);
            return value;
        }

        public virtual D Add(K key, V value)
        {
            D d = this.Add(key);
            d.Value = value;
            return (D)this;
        }

        public virtual void AddRange(IDictionary<K, D> dict)
        {
            foreach (KeyValuePair<K, D> pair in dict)
                this.Add(pair);
        }

        #endregion

        #region IDictionary<K, D> Members 

        void IDictionary<K, D>.Add(K key, D value)
        {
            this.Dictionary.Add(key, value);
        }

        public bool ContainsKey(K key)
        {
            return this.Dictionary.ContainsKey(key);
        }

        public ICollection<K> Keys
        {
            get { return this.Dictionary.Keys; }
        }

        public bool Remove(K key)
        {
            return this.Dictionary.Remove(key);
        }

        public bool TryGetValue(K key, out D value)
        {
            return this.Dictionary.TryGetValue(key, out value);
        }

        public ICollection<D> Values
        {
            get { return this.Dictionary.Values; }
        }

        public D this[K key]
        {
            get
            {
                D value;

                if (!this.TryGetValue(key, out value))
                    return this.Add(key);
                
                return value;
            }
            set
            {
                this.Dictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<K, V>> Members 

        public void Add(KeyValuePair<K, D> item)
        {
            this.Dictionary.Add(item);
        }

        public void Clear()
        {
            this.Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<K, D> item)
        {
            return this.Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<K, D>[] array, int arrayIndex)
        {
            this.Dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.Dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<K, D> item)
        {
            return this.Dictionary.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<K, V>> Members 

        public IEnumerator<KeyValuePair<K, D>> GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members 

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        #endregion

        #region Properties 
        
        private IDictionary<K, D> _dictionary;
        protected virtual IDictionary<K, D> Dictionary
        {
            get
            {
                if (this._dictionary == null)
                    this._dictionary = this.CreateDictionary();

                return this._dictionary;
            }
        }

        private V _value;
        public virtual V Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        #endregion

        #region IXmlSerializable Members 

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new System.NotImplementedException("The method or operation is not implemented.");
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new System.NotImplementedException("The method or operation is not implemented.");
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            if (this.Count > 0)
            {
                foreach (K key in this.Keys)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("key", key.ToString());
                    this[key].WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            else if (_value != null)
            {
                writer.WriteValue(_value);
            }

        }

        #endregion

    }

}

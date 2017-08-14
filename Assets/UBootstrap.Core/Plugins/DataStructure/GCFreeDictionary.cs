using System.Collections.Generic;
using System;

namespace UBootstrap
{
    public class KeyVal<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public KeyVal ()
        {
        }

        public KeyVal (TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public class GCFreeDictionary <TKey, TValue>
    {
        protected List<KeyVal<TKey, TValue>> list;
        protected GCFreeDictionaryEnumerator <TKey, TValue> enumerator;

        public GCFreeDictionary ()
        {
            list = new List<KeyVal<TKey, TValue>> ();
            enumerator = new GCFreeDictionaryEnumerator <TKey, TValue> (list);
        }

        public GCFreeDictionaryEnumerator <TKey, TValue> GetEnumerator ()
        {
            enumerator.Reset ();
            return enumerator;
        }

        public int Count {
            get {
                return list.Count;
            }
        }

        public void Add (TKey key, TValue value)
        {
            if (key == null) {
                throw new ArgumentNullException ("key is null");
            }

            if (ContainsKey (key)) {
                throw new ArgumentException ("An element with the same key already exists");
            }

            list.Add (new KeyVal<TKey, TValue> (key, value));
        }

        public void Clear ()
        {
            list.Clear ();
        }

        public bool ContainsKey (TKey key)
        {
            if (key == null) {
                throw new ArgumentNullException ("key is null");
            }

            for (var i = 0; i < list.Count; i++) {
                if (list [i].Key.Equals (key)) {
                    return true;
                }
            }

            return false;
        }

        public bool Remove (TKey key)
        {
            if (key == null) {
                throw new ArgumentNullException ("key is null");
            }

            for (var i = 0; i < list.Count; i++) {
                if (list [i].Key.Equals (key)) {
                    list.RemoveAt (i);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetValue (TKey key, out TValue value)
        {
            if (key == null) {
                throw new ArgumentNullException ("key is null");
            }

            for (var i = 0; i < list.Count; i++) {
                if (list [i].Key.Equals (key)) {
                    value = list [i].Value;
                    return true;
                }
            }

            value = default (TValue);
            return false;
        }

        public TValue this [
            TKey key
        ] {
            get {
                if (key == null) {
                    throw new ArgumentNullException ("key is null");
                }
                for (var i = 0; i < list.Count; i++) {
                    if (list [i].Key.Equals (key)) {
                        return list [i].Value;
                    }
                }
                throw new KeyNotFoundException ("The property is retrieved and key does not exist in the collection");
            }
            set {
                if (key == null) {
                    throw new ArgumentNullException ("key is null");
                }
                for (var i = 0; i < list.Count; i++) {
                    if (list [i].Key.Equals (key)) {
                        list [i].Value = value;
                        return;
                    }
                }
                Add (key, value);
            }
        }
    }

    public class GCFreeDictionaryEnumerator <TKey, TValue>
    {
        private readonly List<KeyVal<TKey, TValue>> pool;
        private int index;

        public GCFreeDictionaryEnumerator (List<KeyVal<TKey, TValue>> pool)
        {
            this.pool = pool;
            this.index = 0;
        }

        public KeyVal<TKey, TValue> Current {
            get {
                if (this.pool == null || this.index == 0)
                    throw new System.InvalidOperationException ();

                return this.pool [this.index - 1];
            }
        }

        public bool MoveNext ()
        {
            this.index++;
            return this.pool != null && this.pool.Count >= this.index;
        }

        public void Reset ()
        {
            this.index = 0;
        }
    }
}
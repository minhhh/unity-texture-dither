using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UBootstrap
{
    public class LimitedSizeStack<T>
    {
        private readonly int _maxSize;
        List<T> _stack;
        LimitedSizeStackEnumerator<T> _enumerator;
        int head = 0;
        int tail = 0;

        public LimitedSizeStack (int maxSize)
        {
            _maxSize = maxSize;
            maxSize++;
            _stack = new List<T> (maxSize);
            for (int i = 0; i < maxSize; i++) {
                _stack.Add (default(T));
            }

            _enumerator = new LimitedSizeStackEnumerator<T> (_stack);
        }

        public int Count {
            get {
                if (tail >= head) {
                    return tail - head;
                } else {
                    return tail + _maxSize - head + 1;
                }
            }
        }

        public T this [int key] {
            get {
                int index = head + key;
                if (index > _maxSize) {
                    index -= _maxSize;
                    if (index >= head) {
                        throw new System.IndexOutOfRangeException (key.ToString ());
                    }
                }

                return _stack [index];
            }
            set {
                int index = head + key;
                if (index > _maxSize) {
                    index -= _maxSize;
                    if (index >= head) {
                        throw new System.IndexOutOfRangeException (key.ToString ());
                    }
                }
                _stack [index] = value;
            }
        }

        // \ 0 \ 1 \
        //   h   t
        public void Push (T item)
        {
            if (tail == head - 1 || (head == 0 && tail == _maxSize)) {
                if (head < _maxSize) {
                    head++;
                    if (tail == _maxSize) {
                        tail = 0;
                    } else {
                        tail++;
                    }
                } else {
                    head = 0;
                    tail++;
                }
            } else {
                if (tail == _maxSize) {
                    tail = 0;
                } else {
                    tail++;
                }
            }

            if (tail > 0) {
                _stack [tail - 1] = item;
            } else {
                _stack [_maxSize] = item;
            }

        }

        public T Pop ()
        {
            if (tail == head) {
                throw new System.InvalidOperationException ("Stack is empty");
            }
            if (tail == 0) {
                tail = _maxSize;
            } else {
                tail--;
            }

            return _stack [tail];
        }

        public LimitedSizeStackEnumerator<T> GetEnumerator ()
        {
            _enumerator.Reset ();
            return _enumerator;
        }

    }

    public class LimitedSizeStackEnumerator<T>
    {
        private readonly List<T> pool;
        private int index;

        public LimitedSizeStackEnumerator (List<T> pool)
        {
            this.pool = pool;
            this.index = 0;
        }

        public T Current {
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
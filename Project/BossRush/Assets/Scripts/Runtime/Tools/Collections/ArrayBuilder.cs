using System;

namespace TeamSuneat
{
    /// <summary>
    /// 동적 배열 구성을 위한 빌더 클래스
    /// </summary>
    /// <typeparam name="T">배열 요소 타입</typeparam>
    [Serializable]
    public class ArrayBuilder<T>
    {
        private T[] _array;
        private int _capacity;
        private int _count;

        /// <summary>
        /// 현재 저장된 요소 수
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 현재 배열 용량
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// 빈 ArrayBuilder 생성
        /// </summary>
        public ArrayBuilder() : this(16)
        {
        }

        /// <summary>
        /// 지정된 초기 용량으로 ArrayBuilder 생성
        /// </summary>
        /// <param name="initialCapacity">초기 용량</param>
        public ArrayBuilder(int initialCapacity)
        {
            if (initialCapacity <= 0)
                initialCapacity = 16;

            _capacity = initialCapacity;
            _array = new T[_capacity];
            _count = 0;
        }

        /// <summary>
        /// 요소를 배열에 추가합니다.
        /// </summary>
        /// <param name="item">추가할 요소</param>
        public void Add(T item)
        {
            if (_count >= _capacity)
            {
                Resize();
            }
            _array[_count++] = item;
        }

        /// <summary>
        /// 여러 요소를 배열에 추가합니다.
        /// </summary>
        /// <param name="items">추가할 요소 배열</param>
        public void AddRange(T[] items)
        {
            if (items == null || items.Length == 0)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }

        /// <summary>
        /// 여러 요소를 배열에 추가합니다.
        /// </summary>
        /// <param name="items">추가할 요소 컬렉션</param>
        public void AddRange(System.Collections.Generic.IEnumerable<T> items)
        {
            if (items == null)
                return;

            foreach (T item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// 지정된 인덱스에 요소를 삽입합니다.
        /// </summary>
        /// <param name="index">삽입할 인덱스</param>
        /// <param name="item">삽입할 요소</param>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_count >= _capacity)
            {
                Resize();
            }

            // 기존 요소들을 뒤로 이동
            for (int i = _count; i > index; i--)
            {
                _array[i] = _array[i - 1];
            }

            _array[index] = item;
            _count++;
        }

        /// <summary>
        /// 지정된 인덱스의 요소를 제거합니다.
        /// </summary>
        /// <param name="index">제거할 인덱스</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            // 요소들을 앞으로 이동
            for (int i = index; i < _count - 1; i++)
            {
                _array[i] = _array[i + 1];
            }

            _count--;
        }

        /// <summary>
        /// 모든 요소를 제거합니다.
        /// </summary>
        public void Clear()
        {
            _count = 0;
        }

        /// <summary>
        /// 최종 배열을 반환합니다.
        /// </summary>
        /// <returns>구성된 배열</returns>
        public T[] ToArray()
        {
            if (_count == 0)
                return new T[0];

            if (_count == _capacity)
            {
                return _array;
            }

            T[] result = new T[_count];
            Array.Copy(_array, result, _count);
            return result;
        }

        /// <summary>
        /// 지정된 인덱스의 요소를 반환합니다.
        /// </summary>
        /// <param name="index">인덱스</param>
        /// <returns>해당 인덱스의 요소</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _array[index];
            }
            set
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _array[index] = value;
            }
        }

        /// <summary>
        /// 배열 용량을 확장합니다.
        /// </summary>
        private void Resize()
        {
            _capacity *= 2;
            T[] newArray = new T[_capacity];
            Array.Copy(_array, newArray, _count);
            _array = newArray;
        }

        /// <summary>
        /// 배열 용량을 지정된 크기로 설정합니다.
        /// </summary>
        /// <param name="newCapacity">새로운 용량</param>
        public void SetCapacity(int newCapacity)
        {
            if (newCapacity < _count)
                throw new ArgumentException("새로운 용량은 현재 요소 수보다 작을 수 없습니다.");

            if (newCapacity != _capacity)
            {
                _capacity = newCapacity;
                T[] newArray = new T[_capacity];
                Array.Copy(_array, newArray, _count);
                _array = newArray;
            }
        }

        /// <summary>
        /// 배열을 지정된 크기로 조정합니다.
        /// </summary>
        /// <param name="newSize">새로운 크기</param>
        public void Resize(int newSize)
        {
            if (newSize < 0)
                throw new ArgumentException("크기는 0보다 작을 수 없습니다.");

            if (newSize > _capacity)
            {
                SetCapacity(newSize);
            }

            _count = newSize;
        }
    }
}

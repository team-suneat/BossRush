using System;
using UnityEngine;

namespace TeamSuneat
{
    [Serializable]
    public class Order
    {
        [SerializeField] protected bool _isCircular;
        [SerializeField] protected int _min;
        [SerializeField] protected int _max;
        [SerializeField] protected int _current;
        [NonSerialized] public Coroutine DropCoroutine;

        public bool IsCircular => _isCircular;

        public int Min => _min;

        public int Max => _max;

        public int Current
        {
            get => _current;
            private set => _current = value;
        }

        public Order()
        {
            _isCircular = true;
            _min = 0;
            _max = 0;
            _current = _min;
        }

        public bool Next()
        {
            if (Min == Max)
            {
                return false;
            }

            int prev = Current;
            Current += 1;

            if (IsCircular)
            {
                if (Current > Max)
                {
                    Current = Min;
                }
            }
            else
            {
                if (Current > Max)
                {
                    Current = Max;
                }
            }

            return prev != Current;
        }

        public void First()
        {
            Current = Min;
        }

        public void Set(int value)
        {
            if (value < Min || value > Max)
            {
                Log.Error($"값({value})은 최소값({Min})보다 작거나 최대값({Max})보다 클 수 없습니다.");
            }
            else
            {
                Current = value;
            }
        }

        public void SetMin(int index)
        {
            if (index > _max)
            {
                Log.Error($"최소값({index})은 최대값({_max})보다 클 수 없습니다.");
            }
            else
            {
                _min = index;
            }
        }

        public void SetMax(int index)
        {
            if (index < _min)
            {
                Log.Error($"최대값({index})은 최소값({_min})보다 작을 수 없습니다.");
            }
            else
            {
                _max = index;
            }
        }

        public void NextMax()
        {
            _max += 1;
        }

        public bool CheckMin()
        {
            return Current <= Min;
        }

        public bool CheckMax()
        {
            return Current >= Max;
        }

        public void Shuffle()
		{
			if (Max == 0)
			{
				return;
			}

			int random = Current;
			while (random == Current)
			{
				random = RandomEx.Range(Min, Max + 1);
			}

			_current = random;
		}
    }
}
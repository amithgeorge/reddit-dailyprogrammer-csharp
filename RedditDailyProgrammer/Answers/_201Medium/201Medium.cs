using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditDailyProgrammer.Answers._201Medium
{
    public class PriorityQueue<T>
    {
// ReSharper disable once InconsistentNaming
        protected readonly List<T> _queue = new List<T>();
        private readonly Func<Func<T, IComparable>, Func<List<T>, T>> _priorityStrategy;

        public PriorityQueue(bool lowestPriorityFirst)
        {
            _priorityStrategy = lowestPriorityFirst
                                    ? (Func<Func<T, IComparable>, Func<List<T>, T>>) _lowestFirstStrategy
                                    : _highestFirstStrategy;
        }
        
        
        public void Enqueue(T item)
        {
            _queue.Add(item);
        }

        public T Dequeue()
        {
            return Dequeue(x => x.First());
        }

        public T Dequeue(Func<T, IComparable> selector)
        {
            return Dequeue(_priorityStrategy.Invoke(selector));
        }


        private T Dequeue(Func<List<T>, T> itemSelector)
        {
            if (_queue.Count == 0)
            {
                throw new ApplicationException("Queue is empty");
            }

            var item = itemSelector(_queue);
            _queue.Remove(item);
            return item;
        }

        private Func<List<T>, T> _lowestFirstStrategy(Func<T, IComparable> selector)
        {
            return q => q.Aggregate((min, x) => selector(x).CompareTo(selector(min)) < 0 ? x : min);
        }

        private Func<List<T>, T> _highestFirstStrategy(Func<T, IComparable> selector)
        {
            return q => q.Aggregate((max, x) => selector(x).CompareTo(selector(max)) > 0 ? x : max);
        }
    }

    //  A queue with explicit priority values of type double

    public class NPriorityQueue<T>
    {
        private readonly int _maxPriorities;
        private readonly List<QueueItem<T>> _queue = new List<QueueItem<T>>();
        private readonly Func<int, Func<List<QueueItem<T>>, QueueItem<T>>> _priorityStrategy;

        public NPriorityQueue(int maxPriorities)
            : this(maxPriorities, lowestFirst: false)
        {

        }

        public NPriorityQueue(int maxPriorities, bool lowestFirst)
        {
            if (maxPriorities < 1)
            {
                throw new ArgumentException("maxPriorities needs to be more than 0");
            }
            _maxPriorities = maxPriorities;

            _priorityStrategy = lowestFirst ? _lowestFirst : _highestFirst;
        }

        public void Enqueue(T item, params double[] priorities)
        {
            if (priorities.Length != _maxPriorities)
            {
                throw new ArgumentException(string.Format("Expected {0} priorities. Actual: {1}", _maxPriorities, priorities.Length));
            }

            _queue.Add(new QueueItem<T>(item, priorities.ToList()));
        }

        public T Dequeue()
        {
            return Dequeue(q => q.First());
        }

        public T Dequeue(int priorityIndex)
        {
            return Dequeue(_priorityStrategy.Invoke(priorityIndex));
        }

        private T Dequeue(Func<List<QueueItem<T>>, QueueItem<T>> itemSelector)
        {
            if (_queue.Count == 0)
            {
                throw new ArgumentException("Queue is empty");
            }

            var item = itemSelector(_queue);

            _queue.Remove(item);
            return item.Item;
        }

        private readonly Func<int, Func<List<QueueItem<T>>, QueueItem<T>>> _lowestFirst =
            index => q => q.Aggregate((min, x) => x.Priorities[index] <
                                                  min.Priorities[index]
                                                      ? x
                                                      : min);

        private readonly Func<int, Func<List<QueueItem<T>>, QueueItem<T>>> _highestFirst =
            index => q => q.Aggregate((max, x) => x.Priorities[index] >
                                                  max.Priorities[index]
                                                      ? x
                                                      : max);

        private class QueueItem<T2> where T2 : T
        {
            public T2 Item { get; private set; }
            public List<double> Priorities { get; private set; }

            public QueueItem(T2 item, List<double> priorities)
            {
                Item = item;
                Priorities = priorities;
            }
        }
    }

    public class DualPriorityQueue<T>
    {
        private readonly NPriorityQueue<T> _queue;

        public DualPriorityQueue()
        {
            _queue = new NPriorityQueue<T>(2, lowestFirst: true);
        }

        public void Enqueue(T item, double priorityA, double priorityB)
        {
            _queue.Enqueue(item, priorityA, priorityB);
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public T DequeueA()
        {
            return _queue.Dequeue(0);
        }

        public T DequeueB()
        {
            return _queue.Dequeue(1);
        }
    }

}
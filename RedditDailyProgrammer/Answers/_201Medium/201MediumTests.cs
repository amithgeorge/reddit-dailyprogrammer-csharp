using System;
using Moq;
using Xunit;

namespace RedditDailyProgrammer.Answers._201Medium
{
    public class PriorityQueueTests
    {
        [Fact]
        public void Can_prioritize_using_IComparable_property_of_queue_item()
        {
            var queue = new PriorityQueue<Equipment>(lowestPriorityFirst: true);
            queue.Enqueue(new Equipment() { Name = "Item1", Cost = 50.1, ShippingTime = 5 });
            queue.Enqueue(new Equipment() { Name = "Item2", Cost = 10.1, ShippingTime = 10 });
            queue.Enqueue(new Equipment() { Name = "Item3", Cost = 100.2, ShippingTime = 2 });

            var result = queue.Dequeue(x => x.Cost);

            Assert.Equal("Item2", result.Name);
        }

        [Fact]
        public void Dequeuing_an_element_removes_it()
        {
            var queue = new PriorityQueueUT(lowestPriorityFirst: true);
            queue.Enqueue(new Equipment() { Name = "Item1", Cost = 50.1, ShippingTime = 5 });
            queue.Enqueue(new Equipment() { Name = "Item2", Cost = 10.1, ShippingTime = 10 });
            queue.Enqueue(new Equipment() { Name = "Item3", Cost = 100.2, ShippingTime = 2 });

            var result = queue.Dequeue(x => x.Cost);

            Assert.False(queue.Contains(x => x.Name == result.Name));
        }

        [Fact]
        public void Dequeueing_an_empty_queue_throws_ApplicationException()
        {
            var queue1 = new PriorityQueueUT(lowestPriorityFirst: true);
            Assert.Throws<ApplicationException>(() => queue1.Dequeue());

            var queue2 = new PriorityQueueUT(lowestPriorityFirst: true);
            Assert.Throws<ApplicationException>(() => queue2.Dequeue(x => x.Cost));
        }

        [Fact]
        public void When_two_items_have_same_priority_the_earliest_one_is_dequeued()
        {
            var queue = new PriorityQueueUT(lowestPriorityFirst: true);
            queue.Enqueue(new Equipment() {Name = "Item1", Cost = 50.1, ShippingTime = 5});
            queue.Enqueue(new Equipment() {Name = "Item2", Cost = 20.1, ShippingTime = 10});
            queue.Enqueue(new Equipment() {Name = "Item3", Cost = 10.2, ShippingTime = 2});
            queue.Enqueue(new Equipment() {Name = "Item4", Cost = 30.2, ShippingTime = 3});
            queue.Enqueue(new Equipment() {Name = "Item5", Cost = 10.2, ShippingTime = 4});
            queue.Enqueue(new Equipment() {Name = "Item6", Cost = 10.2, ShippingTime = 5});

            var result = queue.Dequeue(x => x.Cost);
            Assert.Equal("Item3", result.Name);
            result = queue.Dequeue(x => x.Cost);
            Assert.Equal("Item5", result.Name);
            result = queue.Dequeue(x => x.Cost);
            Assert.Equal("Item6", result.Name);
        }

        [Fact]
        public void Dequeueing_without_specifying_priority_removes_in_FIFO_order()
        {
            var queue = new PriorityQueueUT(lowestPriorityFirst: true);
            queue.Enqueue(new Equipment() { Name = "Item1", Cost = 50.1, ShippingTime = 5 });
            queue.Enqueue(new Equipment() { Name = "Item2", Cost = 10.1, ShippingTime = 10 });
            queue.Enqueue(new Equipment() { Name = "Item3", Cost = 100.2, ShippingTime = 2 });

            var item1 = queue.Dequeue();
            Assert.Equal("Item1", item1.Name);
            var item2 = queue.Dequeue();
            Assert.Equal("Item2", item2.Name);
            var item3 = queue.Dequeue();
            Assert.Equal("Item3", item3.Name);
        }

        [Fact]
        public void When_created_with_lowestFirst_false_returns_highest_priority_item()
        {
            var queue = new PriorityQueueUT(lowestPriorityFirst: false);
            queue.Enqueue(new Equipment() { Name = "Item1", Cost = 50.1, ShippingTime = 5 });
            queue.Enqueue(new Equipment() { Name = "Item2", Cost = 10.1, ShippingTime = 10 });
            queue.Enqueue(new Equipment() { Name = "Item3", Cost = 100.2, ShippingTime = 2 });

            var result = queue.Dequeue(x => x.Cost);

            Assert.Equal("Item3", result.Name);
        }

        [Fact]
        public void When_created_with_lowestFirst_true_returns_lowest_priority_item()
        {
            var queue = new PriorityQueueUT(lowestPriorityFirst: true);
            queue.Enqueue(new Equipment() { Name = "Item1", Cost = 50.1, ShippingTime = 5 });
            queue.Enqueue(new Equipment() { Name = "Item2", Cost = 10.1, ShippingTime = 10 });
            queue.Enqueue(new Equipment() { Name = "Item3", Cost = 100.2, ShippingTime = 2 });

            var result = queue.Dequeue(x => x.Cost);

            Assert.Equal("Item2", result.Name);
        }

        [Fact]
        public void Can_enqueue_item()
        {
            var queue = new PriorityQueueUT(lowestPriorityFirst: true);

            Assert.Equal(0, queue.Count);
            
            queue.Enqueue(new Equipment(){Name = "Item1", Cost = 10.2, ShippingTime = 3});

            Assert.Equal(1, queue.Count);
        }

// ReSharper disable once InconsistentNaming
        class PriorityQueueUT : PriorityQueue<Equipment>
        {
            public PriorityQueueUT(bool lowestPriorityFirst) : base(lowestPriorityFirst)
            {
            }

            public int Count { get { return _queue.Count; } }

            public bool Contains(Predicate<Equipment> condition)
            {
                return _queue.Exists(condition);
            }
        }

        class Equipment
        {
            public string Name { get; set; }
            public double Cost { get; set; }
            public int ShippingTime { get; set; }
        }
    }
}
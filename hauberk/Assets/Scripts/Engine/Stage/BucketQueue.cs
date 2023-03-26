using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

// TODO: Move to piecemeal?
/// A fast priority queue optimized for non-zero integer priorities in a narrow
/// range.
///
/// Internally, as the name implies, this uses a bucket queue. This means that
/// priorities ("costs", since lower is considered higher priority) need to be
/// integers ranging from 0 to some hopefully small-ish maximum.
///
/// This also does not support updating the priority of a previously enqueued
/// item. Instead, the item is enqueue redundantly. When the higher cost one is
/// visited later, it can be ignored. In practice, this tends to be faster than
/// finding the previous item to update its priority.
///
/// See:
///
/// * https://en.wikipedia.org/wiki/Bucket_queue
/// * https://www.redblobgames.com/pathfinding/a-star/implementation.html#algorithm
public class BucketQueue<T> where T : class
{
  public List<Queue<T>> _buckets = new List<Queue<T>>();
  int _bucket = 0;

  public void reset()
  {
    _buckets.Clear();
  }

  public void add(T value, int cost)
  {
    _bucket = Mathf.Min(_bucket, cost);

    // Grow the bucket array if needed.
    if (_buckets.Count <= cost + 1)
    {
      var off = cost + 1 - _buckets.Count;
      for (int i = 0; i < off; ++i)
        _buckets.Add(null);
    }

    // Find the bucket, or create it if needed.
    var bucket = _buckets[cost];
    if (bucket == null)
    {
      bucket = new Queue<T>();
      _buckets[cost] = bucket;
    }

    bucket.Enqueue(value);
  }

  /// Removes the best item from the queue or returns `null` if the queue is
  /// empty.
  public T removeNext()
  {
    // Advance past any empty buckets.
    while (_bucket < _buckets.Count &&
        (_buckets[_bucket] == null || _buckets[_bucket].Count == 0))
    {
      _bucket++;
    }

    // If we ran out of buckets, the queue is empty.
    if (_bucket >= _buckets.Count) return null;

    return _buckets[_bucket].Dequeue();
  }
}

using NUnit.Framework;

using OctoAwesome.Collections;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Tests;
internal class EnumerationmodifiableConcurrentListTests
{



    public EnumerationmodifiableConcurrentList<int> GetList()
    {
        EnumerationmodifiableConcurrentList<int> list = new();
        foreach (var item in Enumerable.Range(0, 100))
        {
            list.Add(item);
        }
        return list;
    }

    [Test]
    public void AddAdd()
    {
        var list = GetList();
        int maxIndex = list.Count * 3;
        int index = 0;
        foreach (var item in list)
        {
            list.Add(item);
            index++;
            if (index == maxIndex)
                break;
        }
        Assert.That(list.Count, Is.EqualTo(maxIndex / 3 * 4));

    }

    [Test]
    public void GetSameEnumeratorTwice()
    {
        var list = GetList();
        int hash = 0;
        using (var enumerator = list.GetEnumerator())
        {
            hash = enumerator.GetHashCode();
        }
        using (var enumerator = list.GetEnumerator())
        {
            Assert.That(hash, Is.EqualTo(enumerator.GetHashCode()));
        }
    }

    [Test]
    public void RemoveInsideForeach()
    {
        IList<int> list = GetList();
        int index = 0;
        foreach (var item in list)
        {
            Assert.That(index, Is.EqualTo(list[0]).And.EqualTo(item));
            list.Remove(item);
            index++;
        }
    }

    [Test]
    public void RemoveInsideForeachReverse()
    {
        IList<int> list = GetList();
        int index = list.Count - 1;
        foreach (var item in list.Reverse())
        {
            Assert.That(index, Is.EqualTo(list[index]).And.EqualTo(item));
            list.Remove(item);
            index--;
        }
    }
    [Test]
    public void RemoveAndIterateAtTheSameTime()
    {
        IList<int> list = GetList();
        list.Add(100);
        int index = list.Count - 1;
        int indexWhoCountsNormally = 0;

        foreach (var item in list)
        {
            Assert.That(index, Is.EqualTo(list[index]));
            Assert.That(indexWhoCountsNormally, Is.EqualTo(list[indexWhoCountsNormally]).And.EqualTo(item));
            list.RemoveAt(index);
            index--;
            indexWhoCountsNormally++;
        }
    }
    [Test]
    public void RemoveAndInsertAtTheSameTime()
    {
        var list = GetList();
        Random random = new Random();
        int removed = 0, added = 0;


        var t2 = Task.Run(() =>
        {
            foreach (var item in list)
            {
                list.Remove(item);
                removed++;
            }
        });

        var t1 = Task.Run(() =>
        {
            foreach (var item in list)
            {
                list.Add(random.Next(0, int.MaxValue));
                added++;
            }
        });

        Task.WaitAll(new[] { t1, t2 }, 16);

        ;
    }

    int abc = 0;
    [Test]
    public void InsertAtTheSameTime()
    {
        var list = new EnumerationmodifiableConcurrentList<(int dt, int index, int length, int forLoopI, int threadId)>();

        var t1 = Task.Run(() =>
        {
            System.Random r = new(123);
            for (int i = 0; i < 250; i++)
            {
                var c = list.Count;
                var ran = r.Next(0, list.Count);
                //list.Add((Interlocked.Increment(ref abc), ran, c, i, 0));
                list.Insert(ran, (Interlocked.Increment(ref abc), ran, c, i, 0));
            }
        });

        var t2 = Task.Run(() =>
        {
            System.Random r = new(1234);
            for (int i = 0; i < 250; i++)
            {
                var c = list.Count;
                var ran = r.Next(0, list.Count);
                //list.Add((Interlocked.Increment(ref abc), ran, c, i, 1));
                list.Insert(ran, (Interlocked.Increment(ref abc), ran, c, i, 1));
            }
        });
        var t3 = Task.Run(() =>
        {
            System.Random r = new(12345);
            for (int i = 0; i < 250; i++)
            {
                var c = list.Count;
                var ran = r.Next(0, list.Count);
                //list.Add((Interlocked.Increment(ref abc), ran, c, i, 2));
                list.Insert(ran, (Interlocked.Increment(ref abc), ran, c, i, 2));
            }
        });
        var t4 = Task.Run(() =>
        {
            System.Random r = new(123456);
            for (int i = 0; i < 250; i++)
            {
                var c = list.Count;
                var ran = r.Next(0, list.Count);
                //list.Add((Interlocked.Increment(ref abc), ran, c, i, 3));
                list.Insert(ran, (Interlocked.Increment(ref abc), ran, c, i, 3));
            }
        });
        t1.Wait();
        t2.Wait();
        t3.Wait();
        t4.Wait();
        var ordered = list.Distinct().OrderBy(x => x.dt).ToArray();
        ;
    }
}

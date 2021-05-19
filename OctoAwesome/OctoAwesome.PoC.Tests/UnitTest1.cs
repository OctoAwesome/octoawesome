using NUnit.Framework;

using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace OctoAwesome.PoC.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {


            //String => Bool => Long => ULong => Uint => Int

            //var root = new List<DependencyItem>();
            //var unresolved = new List<DependencyItem>();

            //foreach (var item in dependencies)
            //{
            //    if (item.AfterDependencyItems.Count == 0 && item.BeforeDependencyItems.Count > 0)
            //        root.Add(item);
            //    //if (item.AfterDependencyItems.Count == 0 && item.BeforeDependencyItems.Count == 0)
            //    else
            //        unresolved.Add(item);
            //}


            //var leafs = new DependencyItem[dependencies.Count];

            //for (int i = 0; i < dependencies.Count; i++)
            //{
            //    var item = dependencies[i];
            //    var earliestPos = 0;
            //    var latestPosition = 0;

            //    if (item.AfterDependencyItems.Count == 0 && item.BeforeDependencyItems.Count == 0)
            //    {
            //        for (int l = 0; l < leafs.Length; l++)
            //        {
            //            if (leafs[l] is not null)
            //                continue;

            //            leafs[l] = item;
            //            break;
            //        }
            //    }

            //    foreach (var parentDependencie in item.AfterDependencyItems)
            //    {
            //        for (int l = 0; l < leafs.Length; l++)
            //        {
            //            if ((leafs[l]?.Name) != parentDependencie)
            //                continue;

            //            if (earliestPos < l)
            //            {
            //                earliestPos = l;
            //            }
            //        }
            //    }

            //}

            var dependencies = new List<DependencyItem>()
            {
                new DependencyItem(typeof(string), "String", new List<string> { }, new List<string> { }),
                new DependencyItem(typeof(int), "Int", new List<string> { "Bool", "ULong" }, new List<string> { }),
                new DependencyItem(typeof(bool), "Bool", new List<string> { }, new List<string> { "Long", "UInt" }),
                new DependencyItem(typeof(long), "Long", new List<string> { }, new List<string> { "ULong" }),
                new DependencyItem(typeof(uint), "UInt", new List<string> { "ULong" }, new List<string> { }),
                new DependencyItem(typeof(ulong), "ULong", new List<string> { "String" }, new List<string> { }),
                new DependencyItem(typeof(ulong), "UShort", new List<string> { "String", "Long", "Bool", "Int" }, new List<string> { }),
                new DependencyItem(typeof(ulong), "Short", new List<string> { "Test" }, new List<string> {"Test2" }),
            };

            //String => Bool => Long => ULong => Uint => Int
            //Bool => String => Long => ULong => Uint => Int
            var dic = dependencies.ToDictionary(x => x.Name, x => new RefCount(x, new(), new(), 0));


            //Wenn kein Before: Muss nur nach besagten After geladen werden. Positionierung sonst irrelevant
            //Wenn kein After: Muss nur vor besagten Before geladen werden. Positionierung sonst irrelevant
            //Wenn beides nicht: Überall
            //Wenn beides vorhanden: Schwierig, irgendwo dazwischen

            foreach (var item in dependencies)
            {
                RefCount refCount;
                if (!dic.TryGetValue(item.Name, out refCount))
                    continue;

                foreach (var after in item.AfterDependencyItems)
                {
                    if (!dic.TryGetValue(after, out var afterCount))
                        continue;
                    afterCount.Before.Add(refCount);
                    refCount.After.Add(afterCount);
                }

                foreach (var before in item.BeforeDependencyItems)
                {
                    if (!dic.TryGetValue(before, out var beforeCount))
                        continue;
                    beforeCount.After.Add(refCount);
                    refCount.Before.Add(beforeCount);
                }
            }
            //Insert Dependencies to Path Logic here


            // BOOL => LONG => STRING => Ulong => INT/UINT
            // bool => Long => string => Ulong => int/uint

            //SeperateWay(dic.Select(v => v.Value));
            CalcDummy(dic.Select(v => v.Value));
            List<RefCount> tree = new();
            HashSet<string> LoadTreeNode(RefCount refCount, HashSet<string> alreadyDoneItems, int minIndex = 0, int maxIndex = int.MaxValue)
            {
                if (alreadyDoneItems.Contains(refCount.Item.Name))
                    return alreadyDoneItems;

                alreadyDoneItems.Add(refCount.Item.Name);

                bool anyIndex = false;

                int indexLeft = minIndex;

                foreach (var ba in refCount.After)
                {
                    int existIndex = tree.IndexOf(ba, minIndex);
                    if (existIndex > -1 && existIndex > indexLeft - 1)
                    {
                        anyIndex = true;
                        indexLeft = existIndex + 1;
                    }
                }
                int indexRight = int.MaxValue;

                foreach (var ba in refCount.Before)
                {
                    int existIndex = tree.IndexOf(ba, minIndex);
                    if (existIndex > -1 && existIndex < maxIndex && existIndex < indexRight)
                    {
                        anyIndex = true;
                        indexRight = existIndex;
                    }
                }

                if (indexLeft > indexRight)
                    throw new System.Exception("No possible slot found");

                var insertIndex = indexRight == int.MaxValue ? indexLeft : indexRight;
                if (!anyIndex)
                    tree.Add(refCount);
                else
                    tree.Insert(insertIndex, refCount);

                foreach (var ba in refCount.After)
                    LoadTreeNode(ba, alreadyDoneItems);
                foreach (var ba in refCount.Before)
                    LoadTreeNode(ba, alreadyDoneItems);

                return alreadyDoneItems;
            }

            var orderedDic = dic
                .OrderByDescending(x => x.Value.After.Count + x.Value.Before.Count)
                ;

            HashSet<string> loadedItemNames = new();
            foreach (var ba in orderedDic)
            {
                LoadTreeNode(ba.Value, loadedItemNames);
            }




            Assert.Pass();
        }

        private void SeperateWay(IEnumerable<RefCount> refCounts)
        {
            /*                  0(3)    -> 5(7)
             *                  1(4)
             *                  2       -> 1(8)
             *         2(1) <-  3
             *         2    <-  4(5)
             *         3(2) <-  5(6)    -> 1 (9),4(10)

            2,3,0,5,1,4
            Bool,Long,String,Ulong,Int,Uint

            */
            var index = 1;

            foreach (var item in refCounts)
            {
                foreach (var beforeRef in item.After)
                {
                    if (beforeRef.Position == 0)
                    {
                        beforeRef.Position = index;
                        index++;
                    }
                }
            }

            foreach (var item in refCounts)
            {
                if (item.Position == 0)
                {
                    item.Position = index;
                    index++;
                }
            }

            var indexBeforeAfter = index;

            foreach (var item in refCounts.OrderBy(x => x.Position))
            {
                foreach (var beforeRef in item.Before)
                {
                    if (beforeRef.Position > indexBeforeAfter)
                        continue;

                    beforeRef.Position = index;
                    index++;
                }
            }

            var result = refCounts.OrderBy(r => r.Position).Select(r => r.Item.Name);
            var resultstring = string.Join("=>", result);
        }


        private void CalcDummy(IEnumerable<RefCount> refCounts)
        {
            /*
             *                      0       -> 5,6
             *                      1       -> 6
             *                      2       -> 1,6   
             *         2    <-      3       -> 6
             *         2    <-      4   
             *         3    <-      5       -> 1,4 
             *                      6
             *                  
             *        0 = 0
             *        1 = 2
             *        2 = 0
             *        3 = 1
             *        4 = 2
             *        5 = 2
             *        6 = 3
             *        
             *        0,2,3,1,4,5,6
            */

            var index = 0;

            foreach (var item in refCounts)
            {
                CheckAndMove(item, null);
            }

            var result = refCounts.OrderBy(r => r.Position).Select(r => r.Item.Name);
            var resultstring = string.Join("=>", result);
        }

        private static void CheckAndMove(RefCount item, RefCount parent)
        {
            var maxLef = item.After.Count > 0 ? item.After.Max(r => r.Position) : 0;

            if (item.Position <= maxLef)
            {
                item.Position = maxLef + 1;               
            }

            foreach (var rightItem in item.Before)
            {
                if(parent == rightItem)
                {
                    throw new ArgumentException();
                }

                if (rightItem.Position <= item.Position)
                {
                    rightItem.Position = item.Position + 1;
                    CheckAndMove(rightItem, item);
                }
            }
        }

        class RefCount
        {
            public RefCount(DependencyItem Item, List<RefCount> Before, List<RefCount> After, int Position)
            {
                this.Item = Item;
                this.Before = Before;
                this.After = After;
                this.Position = Position;
            }

            public DependencyItem Item { get; }
            /// <summary>
            /// This loads before these items
            /// </summary>
            public List<RefCount> Before { get; }
            /// <summary>
            /// This loads after these items
            /// </summary>
            public List<RefCount> After { get; }
            public int Position { get; set; }

            public override string ToString()

                => $"{Item.Name} loads Before: {string.Join(", ", Before.Select(x => x.Item.Name))} and loads After: {string.Join(", ", After.Select(x => x.Item.Name))} (BeforeCount: {Before.Count}, AfterCount: {After.Count})";

        }
    }
}
using NUnit.Framework;

using OpenTK.Windowing.Desktop;

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
            };
            var dic = dependencies.ToDictionary(x => x.Name, x => new RefCount(x, new(), new()));

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
                }

                foreach (var before in item.BeforeDependencyItems)
                {
                    if (!dic.TryGetValue(before, out var beforeCount))
                        continue;
                    beforeCount.After.Add(refCount);
                }
            }
            //Insert Dependencies to Path Logic here

            Assert.Pass();
        }

        record RefCount(DependencyItem Item, List<RefCount> Before, List<RefCount> After)
        {
            public override string ToString()

                => $"{Item.Name}  Loads {Before.Count} Before:{string.Join(", ", Before.Select(x=>x.Item.Name))}  Loads {After.Count} After:{string.Join(", ", After.Select(x => x.Item.Name))}";
            
        }
    }
}
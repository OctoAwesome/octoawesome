using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC.Tests
{
    public static class DependencyTreeFactory
    {
        private static readonly Random random = new();

        public static List<DependencyLeaf> ConvertToLeaves(List<DependencyItem> items)
        {
            var dic = items.ToDictionary(x => x.Name, x => new DependencyLeaf(x, new(), new(), 0));

            foreach (var item in items)
            {
                if (!dic.TryGetValue(item.Name, out var leaf))
                    continue;

                foreach (var after in item.AfterDependencyItems)
                {
                    if (!dic.TryGetValue(after, out var afterLeaf))
                        continue;

                    afterLeaf.Children.Add(leaf);
                    leaf.Parents.Add(afterLeaf);
                }

                foreach (var before in item.BeforeDependencyItems)
                {
                    if (!dic.TryGetValue(before, out var beforeLeaf))
                        continue;

                    beforeLeaf.Parents.Add(leaf);
                    leaf.Children.Add(beforeLeaf);
                }
            }

            return dic.Select(x => x.Value).ToList();
        }

        public static List<DependencyItem> GetDependencies(int amount, bool valid)
        {
            var tree = new List<DependencyItem>();
            var uniqueNames = new HashSet<string>();

            for (int i = 0; i < amount; i++)
            {
                string name;
                do
                {
                    name = RandomString(30);
                } while (uniqueNames.Contains(name));
                uniqueNames.Add(name);

                tree.Add(new DependencyItem(typeof(string), name, new(), new()));
            }

            for (int i = 0; i < amount; i++)
            {
                var item = tree[i];

                int beforeAmount = random.Next(0, i);

                if (beforeAmount > 0)
                {
                    for (int before = 0; before < beforeAmount; before++)
                    {
                        DependencyItem beforeItem;
                        while (true)
                        {
                            beforeItem = tree[random.Next(0, i)];
                            if (!item.AfterDependencyItems.Contains(beforeItem.Name))
                            {
                                item.AfterDependencyItems.Add(beforeItem.Name);
                                break;
                            }
                        }
                    }
                }

                var afterAmount = random.Next(0, amount - i);

                if (afterAmount > 0)
                {
                    for (int after = 0; after < afterAmount; after++)
                    {
                        DependencyItem afterItem;
                        while (true)
                        {
                            afterItem = tree[random.Next(i + 1, amount)];
                            if (!item.BeforeDependencyItems.Contains(afterItem.Name))
                            {
                                item.BeforeDependencyItems.Add(afterItem.Name);
                                break;
                            }
                        }
                    }
                }
            }

            if (!valid && amount >= 2)
            {
                var item1 = tree[0];
                var item2 = tree[1];

                if (!item1.AfterDependencyItems.Contains(item2.Name))
                    item1.AfterDependencyItems.Add(item2.Name);
                if (!item2.AfterDependencyItems.Contains(item1.Name))
                    item2.AfterDependencyItems.Add(item1.Name);
            }

            return tree.OrderBy(x => x.Name).ToList();
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü";
            return new string(Enumerable.Range(0, length).Select(s => chars[random.Next(length)]).ToArray());
        }

    }
}

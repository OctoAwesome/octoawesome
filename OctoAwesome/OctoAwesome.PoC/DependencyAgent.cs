using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC
{
    public class DependencyAgent
    {
        private readonly DependencyTree internalTree;

        public DependencyAgent(DependencyTree tree)
        {
            internalTree = tree;
        }

        internal Dictionary<int, Type> GetDependencyTypeOrder<TKey>(TKey key, Type typeOfTKey, Type typeOfTValue)
        {
            return null;
        }
        public static bool TryCreateTree(IList<DependencyItem> dependencies, out DependencyTree tree)
        {
            if (dependencies is null)
                throw new ArgumentNullException(nameof(dependencies));

            tree = null;
            var graph = CreateGraph(dependencies);
            //TODO: Cycle detection

            if (HasCycle(graph))
                return false;

            tree = TopologicalSort(graph);
            return true;
        }
        private static bool HasCycle(IList<DependencyLeaf> dependencies)
        {
            HashSet<DependencyLeaf> visitedChildren = new();

            foreach (var dependency in dependencies)
            {
                foreach (var child in dependency.Children)
                {
                    if (visitedChildren.Contains(child))
                        continue;

                    if (CheckCycleForChildren(child, dependency, dependencies.Count - 1, visitedChildren))
                        return true;

                    visitedChildren.Add(child);
                }
            }

            return false;
        }

        private static bool CheckCycleForChildren(DependencyLeaf children, DependencyLeaf parent, int maxDepth, HashSet<DependencyLeaf> visitedChildren, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth
                || children == parent)
            {
                return true;
            }
            foreach (var child in children.Children)
            {
                if (visitedChildren.Contains(child))
                    continue;

                if (CheckCycleForChildren(child, parent, maxDepth, visitedChildren, currentDepth + 1))
                    return true;
                visitedChildren.Add(child);
            }

            return false;
        }

        private static List<DependencyLeaf> CreateGraph(IList<DependencyItem> dependencies)
        {
            var dic
                = dependencies
                .ToDictionary(
                    x => x.Name,
                    x => new DependencyLeaf(x, new(), new(), 0)
                );

            foreach (var item in dependencies)
            {
                DependencyLeaf refCount;
                if (!dic.TryGetValue(item.Name, out refCount))
                    continue;

                foreach (var after in item.AfterDependencyItems)
                {
                    if (!dic.TryGetValue(after, out var afterCount))
                        continue;
                    afterCount.Children.Add(refCount);
                    refCount.Parents.Add(afterCount);
                }

                foreach (var before in item.BeforeDependencyItems)
                {
                    if (!dic.TryGetValue(before, out var beforeCount))
                        continue;

                    beforeCount.Parents.Add(refCount);
                    refCount.Children.Add(beforeCount);
                }
            }

            return dic.Values.ToList();
        }

        private static DependencyTree TopologicalSort(List<DependencyLeaf> graph)
        {
            foreach (var item in graph)
            {
                CheckAndMove(item);
            }

            var items
                = graph
                .OrderBy(r => r.Position)
                .ToDictionary(l => l.Item.Type, l => l);

            return new(items);
        }

        private static void CheckAndMove(DependencyLeaf item)
        {
            var maxLef = item.Parents.Count > 0 ? item.Parents.Max(r => r.Position) : 0;

            if (item.Position <= maxLef)
            {
                item.Position = maxLef + 1;
            }

            foreach (var rightItem in item.Children)
            {
                if (rightItem.Position <= item.Position)
                {
                    rightItem.Position = item.Position + 1;
                    CheckAndMove(rightItem);
                }
            }
        }
    }
}

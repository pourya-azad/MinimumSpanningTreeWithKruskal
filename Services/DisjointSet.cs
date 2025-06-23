namespace MinimumSpanningTreeWithKruskal.Services
{
    public class DisjointSet
    {
        private int[] parent;
        public DisjointSet(int n)
        {
            parent = Enumerable.Range(0, n).ToArray();
        }
        public int Find(int x) =>
            parent[x] == x ? x : parent[x] = Find(parent[x]);
        public bool Union(int a, int b)
        {
            int ra = Find(a), rb = Find(b);
            if (ra == rb) return false;
            parent[ra] = rb;
            return true;
        }
    }
}

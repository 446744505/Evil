namespace Evil.Util
{
    public class AssemblySearch
    {
        public static void DoSearch(params ISearchAble[] searchAbles)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes());
            foreach (var searchAble in searchAbles)
            {
                List<Type> result = new();
                foreach (var type in types)
                {
                    if (searchAble.IsSearched(type))
                    {
                        result.Add(type);
                    }
                }

                searchAble.OnSearch(result);
            }
        }
    }

    public interface ISearchAble
    {
        bool IsSearched(Type type);
        void OnSearch(List<Type> types);
    }
}
namespace Evil.Provide
{
    public partial class Provide
    {
        public static Provider[] ParseProvider(string provider)
        {
            var urls = provider.Split(';');
            List<Provider> providers = new();
            foreach (var url in urls)
            {
                var arr = url.Split(':');
                if (arr.Length == 1)
                {
                    providers.Add(new Provider { Host = "127.0.0.1", Port = int.Parse(arr[0]) });
                }
                providers.Add(new Provider { Host = arr[0], Port = int.Parse(arr[1]) });
            }

            return providers.ToArray();
        }
    }

    public struct Provider
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
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
                var host = arr[0];
                if (string.IsNullOrEmpty(host))
                    host = "127.0.0.1";
                var port = int.Parse(arr[1]);
                providers.Add(new Provider { Host = host, Port = port });
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
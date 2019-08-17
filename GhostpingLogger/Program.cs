using System.Threading.Tasks;

namespace GhostpingLogger {
    class Program {
        static async Task Main(string[] args) {
            Client client = new Client();
            await client.SetupClient();
            await client.ConnectClient();
        }
    }
}

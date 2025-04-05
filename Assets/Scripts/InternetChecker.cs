using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Youregone.Utils
{
    public static class InternetChecker
    {
        public static async Task<bool> IsInternetAvailableAsync()
        {
            using (UnityWebRequest request = UnityWebRequest.Head("https://www.google.com"))
            {
                request.timeout = 5;

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                return request.result == UnityWebRequest.Result.Success;
            }
        }
    }
}
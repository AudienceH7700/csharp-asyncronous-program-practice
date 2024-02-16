using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows;



namespace AsyncronousProgramPractice
{
    class Button
    {
        public Func<object, object, Task>? Clicked
        {
            get; 
            internal set;
        }
    }

    class DamageResult
    {
        public int Damage
        {
            get { return 0; }
        }
    }

    class User
    {
        public bool isEnabled { get; set; }
        public int id { get; set; }
    }

    public class Program
    {
        private static readonly Button s_downloadButton = new();
        private static readonly Button s_calculateButton = new();

        private static readonly HttpClient s_httpClient = new();

        private static readonly IEnumerable<string> s_urlList = new string[]
        {
            "https://learn.microsoft.com",
            "https://learn.microsoft.com/aspnet/core",
            "https://learn.microsoft.com/azure",
            "https://learn.microsoft.com/azure/devops",
            "https://learn.microsoft.com/dotnet",
            "https://learn.microsoft.com/dotnet/desktop/wpf/get-started/create-app-visual-studio",
            "https://learn.microsoft.com/education",
            "https://learn.microsoft.com/shows/net-core-101/what-is-net",
            "https://learn.microsoft.com/enterprise-mobility-security",
            "https://learn.microsoft.com/gaming",
            "https://learn.microsoft.com/graph",
            "https://learn.microsoft.com/microsoft-365",
            "https://learn.microsoft.com/office",
            "https://learn.microsoft.com/powershell",
            "https://learn.microsoft.com/sql",
            "https://learn.microsoft.com/surface",
            "https://dotnetfoundation.org",
            "https://learn.microsoft.com/visualstudio",
            "https://learn.microsoft.com/windows",
            "https://learn.microsoft.com/xamarin"
        };

        private static void Calculate()
        {
            // <PerformGameCalculation>
            static DamageResult CalculateDamageDone()
            {
                return new DamageResult()
                {

                };
            }

            s_calculateButton.Clicked += async (o,e) => {
                // 해당 라인에서는 CalculateDamageDone 함수가 호출되는 동안 UI로 제어권을 넘겨줌
                var damageResult = await Task.Run(() => CalculateDamageDone());
                DisplayDamage(damageResult);
            };
            // </PerformGameCalculation>
        }

        private static void DisplayDamage(DamageResult damage)
        {
            Console.WriteLine(damage.Damage);
        }

        private static void Download(string url)
        {
            // <UnblockingDownload>
            s_downloadButton.Clicked += async (o, e) => {
                // 아래 로직에서는 웹 서비스의 요청이 진행되는 동안 UI(사용자 인터페이스)에 제어권을 넘긴다.
                // UI 스레드는 이제 다른 작업을 수행할 수 있게 된다.
                var stringData = await s_httpClient.GetStringAsync(url);
                DoSomethingWithData(stringData);
            };
            // </UnblockingDownload>
        }

        private static void DoSomethingWithData(object stringData)
        {
            Console.WriteLine("Displaying data : " + stringData);
        }

        // <GetUserForDataset>
        private static async Task<User> GetUserAsync(int userId)
        {
            // {userId}를 통해 조회된 데이터를 가져와 반환한다.
            return await Task.FromResult(new User() { id = userId });
        }

        private static async Task<IEnumerable<User>> GetUserAsync(IEnumerable<int> userIds)
        {
            var getUserTasks = new List<Task<User>>();
            foreach (int userId in userIds)
            {
                getUserTasks.Add(GetUserAsync(userId));
            }

            return await Task.WhenAll(getUserTasks);
        }
        // </GetUserForDataset>

        // <GetUsersForDatasetByLINQ>
        private static async Task<User[]> GetUsersAsyncByLINQ(IEnumerable<int> userIds)
        {
            var getUserTasks = userIds.Select(id => GetUserAsync(id)).ToArray();
            return await Task.WhenAll(getUserTasks);
        }
        // </GetUsersForDatasetByLINQ>

        // <ExtractDataFromNetwork>
        /*[HttpGet, Route("DotNetCount")]*/
        static public async Task<int> GetDotNetCount(string url)
        {
            var html = await s_httpClient.GetStringAsync(url);
            return Regex.Matches(html, @"\.NET").Count();
        }

        static async Task Main()
        {
            Console.WriteLine("Application started.");

            Console.WriteLine("Counting '.NET' phrase in websites..");
            int total = 0;
            foreach (string url in s_urlList)
            {
                var result = await GetDotNetCount(url);
                Console.WriteLine($"{url} : {result}");
                total += result;
            }

            Console.WriteLine("Total : " + total);

            Console.WriteLine("Retrieving User objects with list of IDs...");
            IEnumerable<int> ids = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            var users = await GetUserAsync(ids);
            foreach(User? user in users)
            {
                Console.WriteLine($"{user.id} : isEnabled={user.isEnabled}");
            }

            Console.WriteLine("Application ending");
        }

    }
}
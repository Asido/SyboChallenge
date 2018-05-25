using SyboChallenge.Module.User.Abstraction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SyboChallenge.Benchmark
{
    public class Benchmark
    {
        private readonly IUserService userService;

        private TimeSpan printThreshold = TimeSpan.FromSeconds(1);
        private DateTime lastPrint = DateTime.UtcNow;
        private int lastOperationsCompleted = 0;
        private int operationsCompleted = 0;

        private List<User> benchmarkUser;

        public Benchmark(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task Start()
        {
            benchmarkUser = new List<User>
            {
                await userService.FindOrCreate("benchmark1"),
                await userService.FindOrCreate("benchmark2"),
                await userService.FindOrCreate("benchmark3"),
                await userService.FindOrCreate("benchmark4"),
                await userService.FindOrCreate("benchmark5"),
                await userService.FindOrCreate("benchmark6"),
                await userService.FindOrCreate("benchmark7"),
                await userService.FindOrCreate("benchmark8"),
                await userService.FindOrCreate("benchmark9"),
                await userService.FindOrCreate("benchmark10"),
                await userService.FindOrCreate("benchmark11"),
                await userService.FindOrCreate("benchmark12"),
                await userService.FindOrCreate("benchmark13"),
                await userService.FindOrCreate("benchmark14"),
                await userService.FindOrCreate("benchmark15"),
                await userService.FindOrCreate("benchmark16"),
                await userService.FindOrCreate("benchmark17"),
                await userService.FindOrCreate("benchmark18"),
                await userService.FindOrCreate("benchmark19"),
                await userService.FindOrCreate("benchmark20"),
                await userService.FindOrCreate("benchmark21"),
                await userService.FindOrCreate("benchmark22"),
                await userService.FindOrCreate("benchmark23"),
                await userService.FindOrCreate("benchmark24"),
                await userService.FindOrCreate("benchmark25"),
                await userService.FindOrCreate("benchmark26"),
                await userService.FindOrCreate("benchmark27"),
                await userService.FindOrCreate("benchmark28"),
                await userService.FindOrCreate("benchmark29"),
                await userService.FindOrCreate("benchmark30"),
                await userService.FindOrCreate("benchmark31"),
                await userService.FindOrCreate("benchmark32"),
                await userService.FindOrCreate("benchmark33"),
                await userService.FindOrCreate("benchmark34"),
                await userService.FindOrCreate("benchmark35"),
                await userService.FindOrCreate("benchmark36"),
                await userService.FindOrCreate("benchmark37"),
                await userService.FindOrCreate("benchmark38"),
                await userService.FindOrCreate("benchmark39"),
                await userService.FindOrCreate("benchmark40"),
                await userService.FindOrCreate("benchmark41"),
                await userService.FindOrCreate("benchmark42"),
                await userService.FindOrCreate("benchmark43"),
                await userService.FindOrCreate("benchmark44"),
                await userService.FindOrCreate("benchmark45"),
                await userService.FindOrCreate("benchmark46"),
                await userService.FindOrCreate("benchmark47"),
                await userService.FindOrCreate("benchmark48"),
                await userService.FindOrCreate("benchmark49"),
                await userService.FindOrCreate("benchmark50"),
            };

            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                var tasks = Enumerable.Repeat(0, 10).Select(async e =>
                {
                    await RunBatch().ConfigureAwait(false);

                    var now = DateTime.UtcNow;
                    var sinceLastPrint = now - lastPrint;
                    if (sinceLastPrint >= printThreshold)
                    {
                        lastPrint = now;
                        var completed = operationsCompleted - lastOperationsCompleted;
                        Console.WriteLine($"{stopwatch.ElapsedMilliseconds}:\t{completed}\t{Math.Round(1000.0 / sinceLastPrint.TotalMilliseconds * completed)}/s");
                        lastOperationsCompleted = operationsCompleted;
                    }
                });
                await Task.WhenAll(tasks);
            }
        }

        private async Task RunBatch()
        {
            var tasks = Enumerable.Repeat(0, 50)
                .Select(async e =>
                {
                    var user = benchmarkUser[operationsCompleted % benchmarkUser.Count];
                    await userService.UpdateGameState(user.Key, new State { GamesPlayed = 42, Score = 50 }).ConfigureAwait(false);
                    Interlocked.Increment(ref operationsCompleted);
                });
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}

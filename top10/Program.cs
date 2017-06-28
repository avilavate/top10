using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top10
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            parser p = new parser();
            sw.Restart();
            ConcurrentDictionary<string, int> cDict = new ConcurrentDictionary<string, int>();

            Parallel.ForEach(p.parse(), line =>
            {
                cDict.AddOrUpdate(
                    p.GetUserId(line), 
                    1,
                    (userid, value) =>
                  {
                      return value + 1;
                  });
            });

            var q = (from user in cDict
                     orderby user.Value descending, user.Key ascending
                     select new { userId = user.Key, reviews = user.Value })
                    .Take(10).ToList();

            long timems = sw.ElapsedMilliseconds;
            Console.WriteLine();
            Console.WriteLine("** Top 10 users reviewing movies:");

            foreach (var item in q)
            {
                Console.WriteLine($"{item.userId} : {item.reviews}");
            }

            double time = timems / 1000.0;  // convert milliseconds to secs

            Console.WriteLine();
            Console.WriteLine("** Done! Time: {0:0.000} secs", time);
            Console.WriteLine();
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AppInsightsIssue.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<string> Names { get; set; } = new List<string>();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }



        public void OnGet()
        {

            //Issue reproduced
             Names = AppDomain.CurrentDomain.GetAssemblies().SelectMany(c => c.GetTypes().Where(x => !x.IsAbstract && x.IsContextful))
                                                            .Select(c=> c.Name).ToList(); 

            //PREFERRED SOLUTION #1
            /*
            Names = AppDomain.CurrentDomain.GetAssemblies().Where(c=> c.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company != "Microsoft Corporation")
                .SelectMany(c => c.GetTypes().Where(x => !x.IsAbstract)).Select(c => c.Name)
                                                            .ToList();
            */


            //SOLUTION #2
            /*var bag = new ConcurrentBag<List<string>>();

            Parallel.ForEach(AppDomain.CurrentDomain.GetAssemblies(), assembly =>
            {
                try
                {
                    var types = assembly.GetTypes().Where(x => !x.IsAbstract).Select(c => c.Name).ToList();

                    bag.Add(types);
                }
                catch (ReflectionTypeLoadException)
                {
                    //keep going
                }

            });

            Names = bag.SelectMany(c => c).ToList();
            */


            // Solution #3
            /* var appInsightAssemblies = new List<string>() 
                                       { "DiagnosticServices.SnapshotCollector.HostingStartup", 
                                       "Microsoft.ApplicationInsights",
                                       "Microsoft.ApplicationInsights.SnapshotCollector" };

                   Names = AppDomain.CurrentDomain.GetAssemblies().Where(c => !appInsightAssemblies.Contains(c.GetName().Name))
                                                              .SelectMany(c => c.GetTypes().Where(x => !x.IsAbstract)).Select(c => c.Name)
                                                              .ToList();

               */
        }
    }
}

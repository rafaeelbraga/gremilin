using System;
using System.Threading.Tasks;
using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Providers.WebSocket;
using gremlin.Models.BaseClass;
using gremlin.Models.Edges;
using gremlin.Models.Vertices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static ExRam.Gremlinq.Core.GremlinQuerySource;

namespace gremlin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();

            var gremlinQuerySource = ConfigureGremilim(configuration);

            // apaga o grafo a cada interação do programa
            await gremlinQuerySource.V().Drop();
            
            // cadastra o vertex rafael
            var rafael = await gremlinQuerySource.AddV(new Person("Rafael", "30")).FirstAsync();

            //cadastrar o vertex Be
            var be = await gremlinQuerySource.AddV(new Person("BeMarques", "31")).FirstAsync();

            var legolas = await gremlinQuerySource.AddV(new Dogs("Legolas", "8", "Pastor Suíco")).FirstAsync();
            var noctus = await gremlinQuerySource.AddV(new Dogs("Noctus", "3", "Labrador")).FirstAsync();

            
            // realiza o vinculos
            await gremlinQuerySource.V(rafael.Id!).AddE<Married>(new Married("Casado")).To(_ => _.V(be.Id!));
            await gremlinQuerySource.V(rafael.Id!).AddE<Owner>(new Owner("É dono")).To(_ => _.V(noctus.Id!));

            await gremlinQuerySource.V(be.Id!).AddE<Married>(new Married("Casado")).To(_ => _.V(rafael.Id!));
            await gremlinQuerySource.V(be.Id!).AddE<Owner>(new Owner("É dona")).To(_ => _.V(legolas.Id!));

            //recupera os dados
            var response = await gremlinQuerySource.V(rafael.Id)
                .Out<Married>()
                .OfType<Person>()
                .Values(_ => _.Name)
                .ToArrayAsync();

            foreach (var item in response)
                Console.WriteLine($"Rafael é casado com :{item}");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) 
            => Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hosting, configuration) => 
            {
                configuration.Sources.Clear();

                configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            });
        
        private static IGremlinQuerySource ConfigureGremilim(IConfiguration configuration)
        {
            var gremilimUrl = configuration.GetSection("GremlimInfo:Uri").Value;
            var gremilimDb = configuration.GetSection("GremlimInfo:Database").Value;
            var graph = configuration.GetSection("GremlimInfo:GraphName").Value;
            var key = configuration.GetSection("GremlimInfo:key").Value;

            var grelimLogger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Queries");

            var gremlinQuerySource = g
                .ConfigureEnvironment(env => env.UseLogger(grelimLogger))
                .ConfigureEnvironment(env => env.UseModel
                (
                    GraphModel.FromBaseTypes<Vertex,Edge>(lookup => lookup.IncludeAssembliesOfBaseTypes())
                    .ConfigureProperties(model => model.ConfigureElement<Vertex>(config => config.IgnoreOnUpdate(_ => _.PartitionKey)))
                )
                .ConfigureOptions(options => options.SetValue(WebSocketGremlinqOptions.QueryLogLogLevel, LogLevel.None))
                .UseCosmosDb(builder => builder.At(new Uri(gremilimUrl),gremilimDb, graph).AuthenticateBy(key)));
            
            return gremlinQuerySource;
        }
    }   
}

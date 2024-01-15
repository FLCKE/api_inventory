using System;
using System.Net;
using System.Threading.Tasks;
using InventoryManagement.Api.Repositories;
using InventoryManagement.Api.Controllers;

namespace InventoryManagement.Api
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Testez la connectivité avec la base de donnée
            Console.WriteLine("Testing Database Connection...");

            // créer une instance de inventoryRepository 
            InventoryRepository repository = new InventoryRepository();

            // Vérifier si on sai bien connecter a la base de donnée 
            if (await repository.TestDatabaseConnectionAsync())
            {

                Console.WriteLine("Database connection test successful!");

                //lancer API listener
                Task apiListenerTask = StartApiListener();

                // creer une instance cli avec Inventory()
                InventoryCLI cli = new InventoryCLI();

                // lancer le  CLI 
                Task cliTask = cli.StartAsync();


                await Task.WhenAny(apiListenerTask, cliTask);


                Console.WriteLine("Press any key to exit.");

                Console.ReadKey();
            }
            else
            {
                // afficher le message d'erreur 
                Console.WriteLine("Database connection test failed. Please check your connection string and database server.");
            }
        }

        static async Task StartApiListener()
        {
            //specifier l'url de l'api
            var apiPrefix = "http://localhost:8080/";
            var apiListener = new HttpListener();
            apiListener.Prefixes.Add(apiPrefix);

            Console.WriteLine($"API listening on {apiPrefix}");
            //lancer l'api
            apiListener.Start();

            try
            {
                while (true)
                {
                    //recuperer le context
                    var context = await apiListener.GetContextAsync();

                    //afficher l'url
                    Console.WriteLine($"Received request for: {context.Request.Url}");

                    var controller = new InventoryController();
                    //lancer le traitement de la requette
                    await controller.HandleRequestAsync(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                apiListener.Stop();
            }
        }
    }
}

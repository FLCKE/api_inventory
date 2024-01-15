// InventoryController.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using InventoryManagement.Api.Models;
using InventoryManagement.Api.Repositories;

namespace InventoryManagement.Api.Controllers
{
    public class InventoryController
    {
        private readonly InventoryRepository _repository = new InventoryRepository();
        private readonly UserRepository _userRepository = new UserRepository();

//Il extrait les informations pertinentes de la demande, transmet la demande au gestionnaire approprié, gère 
//les exceptions de manière élégante et ferme le flux de réponses.
        public async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                // Extract version from the request URL
                var version = GetApiVersion(request.Url);
                

                switch (request.HttpMethod)
                {
                    case "GET":
                        await HandleGetRequestAsync(request, response, version);
                        break;
                    case "POST":
                        await HandlePostRequestAsync(request, response, version);
                        break;
                    case "PUT":
                        await HandlePutRequestAsync(request, response, version);
                        break;
                    case "DELETE":
                        await HandleDeleteRequestAsync(request, response, version);
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await WriteTextToResponseAsync(context.Response, $"Internal server error: {ex.Message}");
            }
            finally
            {
                context.Response.Close();
            }
        }

        //le code gère efficacement les requêtes GET pour les articles d'inventaire
        // et les informations sur les utilisateurs, en veillant à ce que les utilisateurs 
        //reçoivent des réponses appropriées à leurs requêtes. Il traite également les demandes
        // non valides ou non prises en charge de manière élégante.

        private async Task HandleGetRequestAsync(HttpListenerRequest request, HttpListenerResponse response, string version)
{
    if (request?.Url?.AbsolutePath == null)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    switch (request.Url.AbsolutePath)
    {
        case "/api/v1/inventory":
            if (!string.IsNullOrEmpty(request.RawUrl) && request.RawUrl.Equals("/api/v1/inventory", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var items = new List<InventoryItem>();
                            items = await _repository.GetItemsAsync();
                            //var items = await _repository.GetItemsAsync() ?? new List<InventoryItem>();
                            await WriteJsonToResponseAsync(response, items);
                        }
            else if (!string.IsNullOrEmpty(request.RawUrl) && request.RawUrl.StartsWith($"/api/v1/inventory") && request.RawUrl.Length > 13)
                        {
                            var idString = request.QueryString["id"];
                            if (int.TryParse(idString, out int id))
                            {
                                var item = await _repository.GetItemByIdAsync(id);
                                if (item != null)
                                    await WriteJsonToResponseAsync(response, item);
                                else
                                    response.StatusCode = (int)HttpStatusCode.NotFound;
                            }
                            else
                            {
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                            }

                        }
            break;
        case "/api/v1/getuser":
            if (!string.IsNullOrEmpty(request.RawUrl) && request.RawUrl.Equals("/api/v1/getuser", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var items = new List<UserItem>();
                            items = await _userRepository.GetAllUserAsync();
                            //var items = await _repository.GetItemsAsync() ?? new List<InventoryItem>();
                            await WriteJsonToResponseAsync(response, items);
                        }
            else if (!string.IsNullOrEmpty(request.RawUrl) && request.RawUrl.StartsWith($"/api/v1/getuser") && request.RawUrl.Length > 11)
                        {
                            var idString = request.QueryString["id"];
                            if (int.TryParse(idString, out int id))
                            {
                                var item = await _userRepository.GetUserByIdAsync(id);
                                if (item != null)
                                    await WriteJsonToResponseAsync(response, item);
                                else
                                    response.StatusCode = (int)HttpStatusCode.NotFound;
                            }
                            else
                            {
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                            }

                        }
            break;
        default:
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            break;

    }
}
//le code gère efficacement les requêtes POST pour les articles d'inventaire
        // et les informations sur les utilisateurs, en veillant à ce que les utilisateurs 
        //reçoivent des réponses appropriées à leurs requêtes. Il traite également les demandes
        // non valides ou non prises en charge de manière élégante.

        private async Task HandlePostRequestAsync(HttpListenerRequest request, HttpListenerResponse response, string version)
{
    if (request?.HttpMethod != "POST" || request.Url?.AbsolutePath == null)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    switch (request.Url.AbsolutePath)
    {
        case "/api/v1/addproduct":
            // Parse the JSON request body into an InventoryItem object
            var item = await ReadJsonFromRequestAsync<InventoryItem>(request);

            // Check if the item is null
            if (item == null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            // Add the item to the repository
            int? productId = await _repository.AddItemAsync(item);

            // Write the product ID to the response body as JSON
            await WriteJsonToResponseAsync(response, productId);
            response.StatusCode = (int)HttpStatusCode.OK;
            break;

        case "/api/v1/adduser":
            // Parse the JSON request body into a UserItem object
            var userItem = await ReadJsonFromRequestAsync<UserItem>(request);

            // Check if the user item is null
            if (userItem == null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            // Add the user item to the repository
            int? userId = await _userRepository.AddUserItemAsync(userItem);

            // Write the user ID to the response body as JSON
            await WriteJsonToResponseAsync(response, userId);
            response.StatusCode = (int)HttpStatusCode.OK;
            break;

        default:
            response.StatusCode = (int)HttpStatusCode.NotFound;
            break;
    }
}

//le code gère efficacement les requêtes PUT pour les articles d'inventaire
        // et les informations sur les utilisateurs, en veillant à ce que les utilisateurs 
        //reçoivent des réponses appropriées à leurs requêtes. Il traite également les demandes
        // non valides ou non prises en charge de manière élégante.
       private async Task HandlePutRequestAsync(HttpListenerRequest request, HttpListenerResponse response, string version)
{
    if (request?.Url?.AbsolutePath == null)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    switch (request.Url.AbsolutePath)
    {
        case "/api/v1/putproduct":
            var idString = request.QueryString?["id"];
            if (int.TryParse(idString, out int id))
            {
                var existingItem = await _repository.GetItemByIdAsync(id);
                if (existingItem != null)
                {
                    var updatedItem = await ReadJsonFromRequestAsync<InventoryItem>(request);
                    if (updatedItem != null)
                    {
                        updatedItem.product_id = id;
                        await _repository.UpdateItemAsync(updatedItem);
                        response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            break;

        case "/api/v1/putuser":
            var idString_user = request.QueryString?["id"];
            if (int.TryParse(idString_user, out int id_user))
            {
                Console.WriteLine("ffffffffff");
                var existingUser = await _userRepository.VerifyUserAsync(id_user);

                if (existingUser != false)
                {
                    var updatedItem = await ReadJsonFromRequestAsync<UserItem>(request);
                    if (updatedItem != null)
                    {
                        updatedItem.user_id = id_user;
                        await _userRepository.UpdateUserItemAsync(updatedItem);
                        response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            break;

        default:
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            break;
    }
}


//le code gère efficacement les requêtes Delete pour les articles d'inventaire
        // et les informations sur les utilisateurs, en veillant à ce que les utilisateurs 
        //reçoivent des réponses appropriées à leurs requêtes. Il traite également les demandes
        // non valides ou non prises en charge de manière élégante.
private async Task HandleDeleteRequestAsync(HttpListenerRequest request, HttpListenerResponse response, string version)
{
    if (request?.Url?.AbsolutePath == null)
    {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    switch (request.Url.AbsolutePath)
    {
        case "/api/v1/deleteproduct":
            var idString = request.QueryString?["id"];
            if (int.TryParse(idString, out int id))
            {
                var existingItem = await _repository.GetItemByIdAsync(id);
                if (existingItem != null)
                {
                    await _repository.DeleteItemAsync(id);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            break;

        case "/api/v1/deleteuser":
            var idString_user = request.QueryString?["id"];
            if (int.TryParse(idString_user, out int id_user))
            {
                Console.WriteLine(id_user + " " + idString_user);
                var existingItem = await _userRepository.VerifyUserAsync(id_user);
                if (existingItem != false)
                {
                    await _userRepository.DeleteUserItemAsync(id_user);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            break;

        default:
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            break;
    }
}

//Ce code fournit un moyen propre et efficace de lire des données JSON à partir d'une requête HTTP 
//et de les désérialiser en un objet fortement typé. Il gère le flux, l'encodage des caractères et 
//les cas d'erreur de manière gracieuse.
        private async Task<T?> ReadJsonFromRequestAsync<T>(HttpListenerRequest request) where T : class
        {
            try
            {
                using (var body = request.InputStream)
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch
            {
                return null;
            }
        }
//conçu pour envoyer des reponses json
        private async Task WriteJsonToResponseAsync(HttpListenerResponse response, object obj)
        {
            response.ContentType = "application/json";
            var json = JsonConvert.SerializeObject(obj);
            await WriteTextToResponseAsync(response, json);
        }
//pour envoyer des reponses textuelles
        private async Task WriteTextToResponseAsync(HttpListenerResponse response, string text)
        {
            using (var output = response.OutputStream)
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                await output.WriteAsync(bytes, 0, bytes.Length);
            }
        }

//le code identifie efficacement la version de l'API à partir d'une URL de requête HTTP, en supposant un modèle
// spécifique de construction d'URL. Il gère les URL de requête valides et non valides avec élégance.
        private string GetApiVersion(Uri? requestUrl)
{
            if (requestUrl != null)
            {
                var segments = requestUrl.Segments;
                if (segments.Length > 2 && segments[1].Equals("api", StringComparison.OrdinalIgnoreCase))
                {
                    return segments[2].TrimEnd('/');
                }
            }

            return null;
        }

    }
}

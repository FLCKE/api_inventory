using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using InventoryManagement.Api.Models;


namespace InventoryManagement.Api.Repositories
{
    public class InventoryRepository
    { 
// Chaîne de connexion pour la base de données  MYSQL
        private string connectionString = "Server=127.0.0.1;Database=boutique;User Id=root;Password=;";

// Teste de manière asynchrone la connexion à la base de données
        public async Task<bool> TestDatabaseConnectionAsync()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // OUVERTURE CONNEXION AVEC LA BDD
                    await connection.OpenAsync();
                    Console.WriteLine("Connection to the database is successful.");
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                //gerer les execeptions en relation avec la connexion de la bdd
                Console.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                //gerer les exception general en relation avec la connexion bdd
                Console.WriteLine($"Error connecting to the database: {ex.Message}");
                return false;
            }
        }

        //method Asynchronous pour obtenir la liste des tous les inventaire des produits de la bdd
        public async Task<List<InventoryItem>> GetItemsAsync()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {

                    //ouverture de la  connexion avec la bdd
                    await connection.OpenAsync();

                    //execution de la request select pour obtenir tous les produits de la table
                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM product", connection))
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        //lecture de chaque ligne et associer a element d'inventaire
                        while (await reader.ReadAsync())
                        {
                            InventoryItem item = MapInventoryItem(reader);
                            items.Add(item);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
      // Gérer les exceptions liées à la récupération des éléments de la base de données

                HandleDatabaseException(ex, "Error fetching items from the database.");
            }

            return items;
        }
        
        public async Task<InventoryItem?> GetItemByIdAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // ouverture connexion a la bdd
                    await connection.OpenAsync();

                    //execute la requet select pour fetch les products par id sur la table product
                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM product WHERE product_id = @product_id", connection))
                    {
                        command.Parameters.AddWithValue("@product_id", id);

                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            //si un product correspondant est trouve, l'associe avec inventaire product
                            if (await reader.ReadAsync())
                            {
                                return MapInventoryItem(reader);
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //gerer les exeception en relation avec le fetch des product par id dans la bdd
                HandleDatabaseException(ex, "Error fetching item by ID from the database.");
            }

            return null;  //productpar trouver ou error survenu
        }



        public async Task<int?> AddItemAsync(InventoryItem item)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouverture connexion avec bdd
                    await connection.OpenAsync();

                    //executer la requete insert pour ajouter un nouveau product a la table 
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO product (product_name, description, price, category, stock_quantity) VALUES (@product_name, @description, @price, @category, @stock_quantity); SELECT LAST_INSERT_ID();", connection))
                    {
                        // Définir les paramètres du nouvel élément
                        AddInventoryItemParameters(command, item);

                        
                        int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return insertedId;
                    }
                }
            }
            catch (MySqlException ex)
            {
       // Gérer les exceptions liées à l'ajout d'un élément à la base de données
                HandleDatabaseException(ex, "Error adding item to the database.");
                return null;
            }
        }
        
        public async Task UpdateItemAsync(InventoryItem item)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    //executer la requete update pour mettre a jour les elements dans la table product
                    
                    using (MySqlCommand command = new MySqlCommand("UPDATE product SET product_name = @product_name, description = @description, price = @price, category = @category, stock_quantity = @stock_quantity WHERE product_id = @product_id", connection))
                    {
  // Définir les paramètres de l'élément mis à jour
                        command.Parameters.AddWithValue("@product_id", item.product_id);
                        AddInventoryItemParameters(command, item);

                        // Execute la requette
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException ex)
            {
// Gérer les exceptions liées à la mise à jour d'un élément dans la base de données
                
                HandleDatabaseException(ex, "Error updating item in the database.");
            }
        }
        
        public async Task DeleteItemAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
           // Ouvrir la connexion à la base de données
                    await connection.OpenAsync();

// Exécute une requête DELETE pour supprimer un élément de la table 'product'
                    using (MySqlCommand command = new MySqlCommand("DELETE FROM product WHERE product_id = @product_id", connection))
                    {
// Définir le paramètre pour l'élément à supprimer
                        command.Parameters.AddWithValue("@product_id", id);

                        // Execute the query
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException ex)
            {
// Gérer les exceptions liées à la suppression d'un élément de la base de données
                HandleDatabaseException(ex, "Error deleting item from the database.");
            }
        }

        // ce code mappe efficacement les données d'un objet MySqlDataReader à un objet InventoryItem, 
        //garantissant que les données extraites sont correctement converties et organisées en une représentation d'objet structurée.
        private InventoryItem MapInventoryItem(MySqlDataReader reader)
        {
           
            return new InventoryItem
            {
                product_id = Convert.ToInt32(reader["product_id"]),
                product_name = Convert.ToString(reader["product_name"]),
                description = Convert.ToString(reader["description"]),
                price = Convert.ToDouble(reader["price"]),
                category = Convert.ToString(reader["category"]),
                stock_quantity = Convert.ToInt32(reader["stock_quantity"]),
            };
        }


        

        //definit les parametres pour l'ajout ou mise a jour d'un InventoryItem
        private void AddInventoryItemParameters(MySqlCommand command, InventoryItem item)
        {
            command.Parameters.AddWithValue("@product_name", item.product_name);
            command.Parameters.AddWithValue("@description", item.description);
            command.Parameters.AddWithValue("@price", item.price);
            command.Parameters.AddWithValue("@category", item.category);
            command.Parameters.AddWithValue("@stock_quantity", item.stock_quantity);
        }
        //identifier et résoudre les problèmes qui surviennent lors des interactions avec la base de données.
        // Il fournit des informations précieuses sur le code d'erreur, le message et le contexte, facilitant ainsi un débogage efficace.
        private void HandleDatabaseException(MySqlException ex, string errorMessage)
        {
            Console.WriteLine($"{errorMessage}\nError Code: {ex.ErrorCode}\nMessage: {ex.Message}");
        }
    }
}

using InventoryManagement.Api.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Repositories
{
    class UserRepository
    {
        private string connectionString = "Server=127.0.0.1;Database=boutique;User Id=root;Password=;";

        //test Asynchronous pour la connexion avec la bdd
        public async Task<bool> TestDatabaseConnectionAsync()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouverture connexion avec la bdd
                    await connection.OpenAsync();
                    Console.WriteLine("Connection to the database is successful.");
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                //gerer des exceptions mysql specific en relation avec la bdd
                Console.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                //gerer des exceptions mysql en relation avec la bdd
                Console.WriteLine($"Error connecting to the database: {ex.Message}");
                return false;
            }
        }


//récupère efficacement toutes les informations relatives à l'utilisateur à partir 
//d'une base de données MySQL, en veillant à ce que les opérations de base de données
// soient traitées de manière asynchrone et que les exceptions soient correctement gérées.


        public async Task<List<UserItem>> GetAllUserAsync()
        {
            List<UserItem> items = new List<UserItem>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouverture connexion avec la bdd
                    await connection.OpenAsync();

                    //execution de la requete select pour fetch  les utilsateur dans la table
                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM user", connection))
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
// Lire chaque ligne et l'associer user

                        while (await reader.ReadAsync())
                        {
                            UserItem item = MapUserItem(reader);
                            items.Add(item);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
// Gérer les exceptions liées à l'extraction d'éléments de la base de données

                HandleDatabaseException(ex, "Error fetching items from the database.");
            }

            return items;
        }


        //ce code permet de récupérer un utilisateur par son identifiant dans une
        // base de données MySQL, en veillant à ce que les opérations de la base de données soient 
        //traitées de manière asynchrone et que les exceptions soient correctement gérées.

        public async Task<UserItem?> GetUserByIdAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouvert connexion avec la bdd
                    await connection.OpenAsync();

                    //execution de la requet select pour fetch les utilisateur par id dans la tabl users
                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM user WHERE user_id = @user_id", connection))
                    {
                        command.Parameters.AddWithValue("@user_id", id);

                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
              // Si un élément correspondant est trouvé, le mapper à un utilisateur

                            if (await reader.ReadAsync())
                            {
                                return MapUserItem(reader);
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //gerer les exceptions en relations avec le fetch des users par id dans la bdd
                HandleDatabaseException(ex, "Error fetching item by ID from the database.");
            }

            return null;   //utilsateur pas trouve ou error srvenu
        }

        //ce code vérifie efficacement l'existence d'un utilisateur sur
        // la base de l'identifiant fourni, en veillant à ce que les opérations de
        // base de données soient traitées de manière asynchrone et à ce que les exceptions soient correctement gérées.


        public async Task<bool> VerifyUserAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouverture connexion avec la bdd
                    await connection.OpenAsync();

                    // Execute a SELECT query to fetch an item by its ID from the 'product' table
                    //execution de la requet select pour fetch les utilisateur apres verification 
                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM user WHERE user_id = @user_id", connection))
                    {
                        command.Parameters.AddWithValue("@user_id", id);

                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
// Si un élément correspondant est trouvé, le mapper à un user
                            while (reader.Read())
                            {
                                Console.WriteLine(reader.GetString(0));
                                return true;

                            }

                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
// Gérer les exceptions liées à l'extraction d'un user de la base de données par son identifiant.


                HandleDatabaseException(ex, "Error fetching item by ID from the database.");
            }

            return false;// L'élément n'a pas été trouvé ou une erreur s'est produite
        }


// ce code ajoute effectivement un nouvel utilisateur à la base de données MySQL, en veillant à ce que les opérations de la base 
//de données soient traitées de manière asynchrone et que les exceptions soient correctement gérées.

        public async Task<int?> AddUserItemAsync(UserItem item)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouverture connexion avec la bdd
                    await connection.OpenAsync();

                    //execution requet  insert pour ahouter a nouveau user
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO user ( username, password, email, first_name, last_name, phone_number, registration_date) VALUES (@username, @password, @email, @first_name, @last_name, @phone_number, @registration_date); SELECT LAST_INSERT_ID();", connection))
                    {
            // Définit les paramètres du nouvel élément

                        AddUserItemParameters(command, item);
                        command.Parameters.AddWithValue("@registration_date", DateTime.Now);

                        

                        int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return insertedId;

                    }
                }
            }
            catch (MySqlException ex)
            {
                //gerer les exception en relation avec ajout  des utilisateur dans la bdd
                HandleDatabaseException(ex, "Error adding item to the database.");
                return null;
            }
        }

        //ce code met à jour un utilisateur existant dans la base de données MySQL,
        // en veillant à ce que les opérations de la base de données soient traitées de manière asynchrone 
        //et que les exceptions soient correctement gérées.

        public async Task UpdateUserItemAsync(UserItem item)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //ouverture connexion avec la bdd
                    await connection.OpenAsync();
                    //execution the la requet update  des users dans la table 
                    using (MySqlCommand command = new MySqlCommand("UPDATE user SET username = @username, email = @email, password = @password, first_name = @first_name, last_name = @last_name, phone_number = @phone_number WHERE user_id = @user_id", connection))
                    {
                        // Définir les paramètres de l'élément mis à jour

                        command.Parameters.AddWithValue("@user_id", item.user_id);
                        AddUserItemParameters(command, item);

    // Exécuter la requête       
                     await command.ExecuteNonQueryAsync();
                        Console.WriteLine("ghhkjkhg");
                    }
                }
            }
            catch (MySqlException ex)
            {
                //gerer les exception en relation avec le update pour les users
                Console.WriteLine($"{ex.Message}");
                HandleDatabaseException(ex, "Error updating item in the database.");
            }
        }

        //ce code met permet de supprimer  un utilisateur existant dans la base de données MySQL,
        // en veillant à ce que les opérations de la base de données soient traitées de manière asynchrone 
        //et que les exceptions soient correctement gérées.
        public async Task DeleteUserItemAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //overture connexion avec la bdd
                    await connection.OpenAsync();

                    //execution de la requet delete pour retirer des users dans la table
                    using (MySqlCommand command = new MySqlCommand("DELETE FROM user WHERE user_id = @user_id", connection))
                    {
             // Définir le paramètre de l'élément à supprimer
                        command.Parameters.AddWithValue("@user_id", id);

                        // execution de la requet 
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException ex)
            {
                //gerer les exceptions en relations avec opertion delete des users dans la bdd
                HandleDatabaseException(ex, "Error deleting item from the database.");
            }
        }

        //ce code permet de mapper efficacement les données d'un objet MySqlDataReader
        // vers un objet UserItem, ce qui garantit que les données extraites sont correctement
        // converties et organisées dans une représentation structurée de l'objet.

        private UserItem MapUserItem(MySqlDataReader reader)
        {

            return new UserItem
            {
                user_id = Convert.ToInt16(reader["user_id"]),
                user_name = Convert.ToString(reader["username"]),
                password = Convert.ToString(reader["password"]),
                first_name = Convert.ToString(reader["first_name"]),
                last_name = Convert.ToString(reader["last_name"]),
                phone_number = Convert.ToString(reader["phone_number"]),
                registration_date = Convert.ToDateTime(reader["registration_date"])
            };
        }

        //ce code définit effectivement les paramètres d'un objet MySqlCommand en 
        //fonction des propriétés des éléments de l'utilisateur, préparant ainsi 
        //la commande à exécuter les requêtes de la base de données relatives aux éléments de l'utilisateur.
        private void AddUserItemParameters(MySqlCommand command, UserItem item)
        {
            command.Parameters.AddWithValue("@username", item.user_name);
            command.Parameters.AddWithValue("@email", item.email);
            command.Parameters.AddWithValue("@password", item.password);
            command.Parameters.AddWithValue("@first_name", item.first_name);
            command.Parameters.AddWithValue("@last_name", item.last_name);
            command.Parameters.AddWithValue("@phone_number", item.phone_number);
            //command.Parameters.AddWithValue("@registration_date", item.registration_date);
        }
        //gerer les exception en relation avec les operations de la bdd
        private void HandleDatabaseException(MySqlException ex, string errorMessage)
        {
            Console.WriteLine($"{errorMessage}\nError Code: {ex.ErrorCode}\nMessage: {ex.Message}");
        }
    }
}

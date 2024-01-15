// InventoryCLI.cs
using System;
using System.Threading.Tasks;
using InventoryManagement.Api.Repositories;
using InventoryManagement.Api.Models;
using System.Net.Mail;

namespace InventoryManagement.Api
{
    public class InventoryCLI
    {
        //création des variable d'acces aux produits et au utilisateurs
        private readonly InventoryRepository _repository;
        private readonly UserRepository _userRepository;
        //initialisation des variables 
        public InventoryCLI()
        {
            _repository = new InventoryRepository();
            _userRepository = new UserRepository();
        }
        //affichage du menu
        public async Task StartAsync()
        {
            Console.WriteLine("Inventory Management CLI");
            Console.WriteLine("Commands:");
            Console.WriteLine("1. View Inventory: view inventory");
            Console.WriteLine("2. View users: view users");
            Console.WriteLine("3. Add Item: add product");
            Console.WriteLine("4. Add user: add user");
            Console.WriteLine("5. Update Item: update inventory");
            Console.WriteLine("6. Update Item: update user");
            Console.WriteLine("7. Delete Item: delete inventory");
            Console.WriteLine("8. Delete Item: delete user");
            Console.WriteLine("9. Exit: exit");
            //boucle traitement des commandes
            while (true)
            {
                Console.Write("Enter a command: ");
                var command = Console.ReadLine()?.ToLower();

                switch (command)
                {
                    case "view inventory":
                        await ViewInventoryAsync();
                        break;
                    case "view users":
                        await ViewUserAsync();
                        break;
                    case "add product":
                        await AddItemAsync();
                        break;
                    case "add user":
                        await AddUserItemAsync();
                        break;
                    case "update product":
                        await UpdateItemAsync();
                        break;
                    case "update user":
                        await UpdateUserItemAsync();
                        break;
                    case "delete product":
                        await DeleteItemAsync();
                        break;
                    case "delete user":
                        await DeleteUserItemAsync();
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Invalid command. Try again.");
                        break;
                }
            }
        }

        // Voir les inventory
        private async Task ViewInventoryAsync()
        {
            var items = await _repository.GetItemsAsync();
            if (items != null)
            {
                Console.WriteLine("Viewing Inventory:");
                foreach (var item in items)
                {
                    Console.WriteLine($"ID: {item.product_id}, Name: {item.product_name}, Price: {item.price}, Category: {item.category}, Stock: {item.stock_quantity} ");
                }
            }
            else
            {
                Console.WriteLine("Error fetching inventory. Please try again.");
            }
        }
        //voir les utilisateur 
        private async Task ViewUserAsync()
        {
            var items = await _userRepository.GetAllUserAsync();
            if (items != null)
            {
                Console.WriteLine("Viewing Users:");
                foreach (var item in items)
                {
                    Console.WriteLine($"ID: {item.user_id}, Name: {item.user_name}, Email: {item.email}, Password: {item.password}, Firstname: {item.first_name}, Lastname: {item.last_name}, Phone_number: {item.phone_number}, Registration_date: {item.registration_date} ");
                }
            }
            else
            {
                Console.WriteLine("Error fetching user. Please try again.");
            }
        }
        // Ajout nouveau utlisateur
        private async Task AddUserItemAsync()
        {
            Console.WriteLine("Adding Item:");

            var newItem = new UserItem();

            // collecter les informations de l'utilisateur
            Console.Write("Enter user name: ");
            newItem.user_name = Console.ReadLine();

            Console.Write("Enter email: ");
            newItem.email = Console.ReadLine();

            Console.Write("Enter password: ");
            newItem.password = Console.ReadLine();


            Console.Write("Enter firstname: ");
            newItem.first_name = Console.ReadLine();

            Console.Write("Enter lastname: ");
            newItem.last_name = Console.ReadLine();

            Console.Write("Enter phone number: ");
            newItem.phone_number = Console.ReadLine();

            newItem.registration_date = DateTime.Now;

            await _userRepository.AddUserItemAsync(newItem);
            Console.WriteLine("Item added successfully.");
        }

        // Ajout nouveau inventory
        private async Task AddItemAsync()
        {
            Console.WriteLine("Adding Item:");

            var newItem = new InventoryItem();

            // collecter les informations du produit
            Console.Write("Enter product name: ");
            newItem.product_name = Console.ReadLine();

            Console.Write("Enter description: ");
            newItem.description = Console.ReadLine();

            Console.Write("Enter price: ");
            if (double.TryParse(Console.ReadLine(), out double price))
            {
                newItem.price = price;
            }
            else
            {
                Console.WriteLine("Invalid price format. Setting price to null.");
                newItem.price = null;
            }

            Console.Write("Enter category: ");
            newItem.category = Console.ReadLine();

            Console.Write("Enter stock quantity: ");
            if (int.TryParse(Console.ReadLine(), out int stockQuantity))
            {
                newItem.stock_quantity = stockQuantity;
            }
            else
            {
                Console.WriteLine("Invalid stock quantity format. Setting stock quantity to null.");
                newItem.stock_quantity = null;
            }

            await _repository.AddItemAsync(newItem);
            Console.WriteLine("Item added successfully.");
        }

        //mettre a jour un utilisateur
        private async Task UpdateUserItemAsync()
        {
            Console.WriteLine("Updating User Item:");

            Console.Write("Enter product ID to update: ");
            if (int.TryParse(Console.ReadLine(), out var userId))
            {
                var existingItem = await _userRepository.GetUserByIdAsync(userId);
                if (existingItem != null)
                {


                    // Cpllecter les information de l'utilisateur a modifier
                    Console.Write("Enter user name: ");
                    existingItem.user_name = Console.ReadLine();

                    Console.Write("Enter email: ");
                    existingItem.email = Console.ReadLine();

                    Console.Write("Enter password: ");
                    existingItem.password = Console.ReadLine();


                    Console.Write("Enter firstname: ");
                    existingItem.first_name = Console.ReadLine();

                    Console.Write("Enter lastname: ");
                    existingItem.last_name = Console.ReadLine();

                    Console.Write("Enter phone number: ");
                    existingItem.phone_number = Console.ReadLine();



                    await _userRepository.UpdateUserItemAsync(existingItem);
                    Console.WriteLine("Item updated successfully.");
                }
                else
                {
                    Console.WriteLine($"Item with ID {userId} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid product ID.");
            }
        }
        // Mettre a jour le produit 
        private async Task UpdateItemAsync()
        {
            Console.WriteLine("Updating Item:");

            Console.Write("Enter product ID to update: ");
            if (int.TryParse(Console.ReadLine(), out var productId))
            {
                var existingItem = await _repository.GetItemByIdAsync(productId);
                if (existingItem != null)
                {
                    // Collect updated information for the existing item
                    Console.Write("Enter new product name: ");
                    existingItem.product_name = Console.ReadLine();

                    Console.Write("Enter new description: ");
                    existingItem.description = Console.ReadLine();

                    Console.Write("Enter new price: ");
                    if (double.TryParse(Console.ReadLine(), out double price))
                    {
                        existingItem.price = price;
                    }
                    else
                    {
                        Console.WriteLine("Invalid price format. Keeping the existing price.");
                    }

                    Console.Write("Enter new category: ");
                    existingItem.category = Console.ReadLine();

                    Console.Write("Enter new stock quantity: ");
                    if (int.TryParse(Console.ReadLine(), out int stockQuantity))
                    {
                        existingItem.stock_quantity = stockQuantity;
                    }
                    else
                    {
                        Console.WriteLine("Invalid stock quantity format. Keeping the existing stock quantity.");
                    }

                    await _repository.UpdateItemAsync(existingItem);
                    Console.WriteLine("Item updated successfully.");
                }
                else
                {
                    Console.WriteLine($"Item with ID {productId} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid product ID.");
            }
        }

        // supprimer les produits dans la bdd
        private async Task DeleteItemAsync()
        {
            Console.WriteLine("Deleting Item:");

            Console.Write("Enter product ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out var productId))
            {
                var existingItem = await _repository.GetItemByIdAsync(productId);
                if (existingItem != null)
                {
                    await _repository.DeleteItemAsync(productId);
                    Console.WriteLine("Item deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Item with ID {productId} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid product ID.");
            }
        }

        //supprimé  les utilisateur dans la bdd
        private async Task DeleteUserItemAsync()
        {
            Console.WriteLine("Deleting Item:");

            Console.Write("Enter product ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out var userId))
            {
                var existingItem = await _userRepository.GetUserByIdAsync(userId);
                if (existingItem != null)
                {
                    await _userRepository.DeleteUserItemAsync(userId);
                    Console.WriteLine("Item deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Item with ID {userId} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid product ID.");
            }
        }
    }
}

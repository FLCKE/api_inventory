
namespace InventoryManagement.Api.Models
{
    public class UserItem
    {
        public int? user_id { get; set; }
        public string? user_name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? phone_number { get; set; }
        public DateTime? registration_date { get; set; }
        // Constructeur par défaut avec initialisation des propriétés
        public UserItem()
        {
            user_id=null; 
            user_name=null;
            email=null;
            password=null;
            first_name=null;
            last_name=null; 
            phone_number=null;
            registration_date=null;
        }
    }
}
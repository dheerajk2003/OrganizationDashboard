using System.ComponentModel.DataAnnotations;

namespace mvc4.Models
{
    public class RegistrationModel
    {
        [Key]
        public int _id { get; set; }
        public string _email { get; set; }
        public string _name { get; set; }
        public string _password { get; set; }
    }
}

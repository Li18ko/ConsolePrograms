using System.Reflection;

namespace ListRepositories.Model {
    public class User {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public Role? Role { get; set; }
        public int RoleId { get; set; }
    }

    public class Role {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

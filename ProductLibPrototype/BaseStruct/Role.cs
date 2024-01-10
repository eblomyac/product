namespace ProductLibPrototype.DataBase
{
    public enum RoleType
    {
        Admin,
        PostMaster,
        Master
    }
    public class Role
    {
        public long Id { get; set; }
        public RoleType Type { get; set; }
        public User User { get; set; }
        public Post? Post { get; set; }
    }
}
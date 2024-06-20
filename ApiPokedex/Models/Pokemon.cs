namespace ApiPokedex.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public IEnumerable<Type> Types { get; set; } = Enumerable.Empty<Type>();
        public Sprite Sprites { get; set; } = new Sprite();
        public string ImageUrl { get; set; } = "";
    }

    public class Type
    {
        public string Name { get; set; } = "";
    }

    public class Sprite
    {
        public string FrontDefault { get; set; } = string.Empty;
    }
}
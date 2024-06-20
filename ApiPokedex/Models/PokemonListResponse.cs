namespace ApiPokedex.Models
{
    public class PokemonListResponse
    {
        public int Count { get; set; }
        public string Next { get; set; } = "";
        public string Previous { get; set; } = "";
        public List<PokemonResult> Results { get; set; }= new List<PokemonResult>();
    }

    public class PokemonResult
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
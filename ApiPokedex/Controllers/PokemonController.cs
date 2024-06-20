using ApiPokedex.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ApiPokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PokemonController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetPokemons()
        {
            var pokemons = new List<Pokemon>();

            // Hacer la solicitud para obtener los primeros 151 Pokémon en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(1, 151), async (i, token) =>
            {
                try
                {
                    // Hacer la solicitud para obtener los detalles de un Pokémon específico
                    var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{i}");

                    // Verificar si la solicitud fue exitosa
                    if (!response.IsSuccessStatusCode)
                    {
                        // Return a custom error response for non-success status codes
                        throw new Exception($"Error {response.StatusCode} ({response.ReasonPhrase})");
                    }

                    // Leer el contenido de la respuesta como una cadena JSON
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Deserializar el JSON en un objeto dinámico
                    var pokemonDetails = JsonConvert.DeserializeObject<dynamic>(jsonString);

                    // Verificar si la deserialización fue exitosa
                    if (pokemonDetails == null)
                    {
                        // Return a custom error response for deserialization errors
                        throw new Exception("Error deserializing JSON");
                    }

                    // Crear un nuevo objeto Pokemon y asignarle los datos necesarios
                    var pokemon = new Pokemon
                    {
                        Id = i,
                        Name = pokemonDetails.name,
                        Types = ((JArray)pokemonDetails.types).Select(t => new ApiPokedex.Models.Type { Name = t["type"]?["name"]?.ToString() ?? "unknown" }).ToList(),
                        ImageUrl = pokemonDetails.sprites["front_default"].ToString()
                    };

                    // Agregar el Pokémon a la lista
                    pokemons.Add(pokemon);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it in some way
                    // For example, you can return a custom error response for exceptions
                    Console.WriteLine(ex.Message);
                }
            });

            // Devolver la lista de Pokémon como respuesta HTTP OK
            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPokemonById(int id)
        {
            try
            {
                if (id < 1 || id > 151)
                {
                    return BadRequest("El ID del Pokémon debe estar en el rango de 1 a 151.");
                }

                // Hacer la solicitud para obtener los detalles del Pokémon específico por su ID
                var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}");

                // Verificar si la solicitud fue exitosa
                if (!response.IsSuccessStatusCode)
                {
                    // Return a custom error response for non-success status codes
                    return StatusCode((int)response.StatusCode, $"Error {response.StatusCode} ({response.ReasonPhrase})");
                }

                // Leer el contenido de la respuesta como una cadena JSON
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserializar el JSON en un objeto Pokemon
                var pokemonDetails = JsonConvert.DeserializeObject<dynamic>(jsonString);

                // Verificar si la deserialización fue exitosa
                if (pokemonDetails == null)
                {
                    // Return a custom error response for deserialization errors
                    return StatusCode(500, "Error deserializando JSON");
                }

                // Crear un nuevo objeto Pokemon y asignarle los datos necesarios
                var pokemon = new Pokemon
                {
                    Id = id,
                    Name = pokemonDetails.name ?? "Unknown",
                    Types = ((JArray)pokemonDetails.types).Select(t => new ApiPokedex.Models.Type { Name = t["type"]?["name"]?.ToString() ?? "unknown" }).ToList(),
                    ImageUrl = pokemonDetails.sprites?["front_default"]?.ToString() ?? ""
                };

                // Devolver el Pokémon como respuesta HTTP OK
                return Ok(pokemon);
            }
            catch (Exception ex)
            {
                // Log the error or handle it in some way
                // For example, you can return a custom error response for exceptions
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
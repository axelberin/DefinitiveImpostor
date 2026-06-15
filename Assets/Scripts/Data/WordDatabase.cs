using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Impostor/Word Database")]
public class WordDatabase : ScriptableObject
{
    [SerializeField] private List<WordCategory> categories = new();

    public IReadOnlyList<WordCategory> Categories => categories;

    private void OnEnable()
    {
        LoadDefaultCategories();
    }

    public List<string> GetCategoryNames()
    {
        List<string> names = new();

        foreach (WordCategory category in categories)
            names.Add(category.CategoryName);

        return names;
    }

    public WordData GetRandomWordFromEnabledCategories(List<string> enabledCategories, out string selectedCategory)
    {
        selectedCategory = "";

        List<WordCategory> availableCategories = categories.FindAll(category =>
            enabledCategories.Contains(category.CategoryName) &&
            category.Words.Count > 0
        );

        if (availableCategories.Count == 0)
        {
            Debug.LogError("No hay categorías habilitadas con palabras.");
            return null;
        }

        WordCategory randomCategory = availableCategories[Random.Range(0, availableCategories.Count)];
        selectedCategory = randomCategory.CategoryName;

        return randomCategory.Words[Random.Range(0, randomCategory.Words.Count)];
    }

    private void LoadDefaultCategories()
    {
        categories = new()
        {
            CreateCategory("Famosos argentinos",
                W("Messi", new[] { "Pelota." }, "Lionel Messi es un futbolista argentino, considerado uno de los mejores jugadores de la historia."),
                W("Maradona", new[] { "Diez." }),
                W("Lali Espósito", new[] { "Pop." }),
                W("Tini Stoessel", new[] { "Disney." }),
                W("Duki", new[] { "Trap." }),
                W("Bizarrap", new[] { "Sesión." }),
                W("Ricardo Darín", new[] { "Cine." }),
                W("Guillermo Francella", new[] { "Racing." }),
                W("Susana Giménez", new[] { "Teléfono." }),
                W("Mirtha Legrand", new[] { "Almuerzo." }),
                W("Charly García", new[] { "Piano." }),
                W("Fito Páez", new[] { "Rosario." }),
                W("Gustavo Cerati", new[] { "Soda." }),
                W("Abel Pintos", new[] { "Pelado." }),
                W("Nicki Nicole", new[] { "Petisa." }),
                W("Wanda Nara", new[] { "Show." }),
                W("Moria Casán", new[] { "Rating." }),
                W("Paulo Dybala", new[] { "Joya." }),
                W("Ángel Di María", new[] { "Pasta." }),
                W("Ricardo Ford", new[] { "Rol Royce." })
            ),

            CreateCategory("Famosos globales",
                W("Taylor Swift", new[] { "Eras." }),
                W("Cristiano Ronaldo", new[] { "Ego." }),
                W("Dwayne Johnson", new[] { "Piedra." }),
                W("Beyoncé", new[] { "Corona." }),
                W("Shakira", new[] { "Caderas." }),
                W("Bad Bunny", new[] { "Maldad." }),
                W("Ariana Grande", new[] { "Agudo." }),
                W("Leonardo DiCaprio", new[] { "Oscar." }),
                W("Tom Holland", new[] { "Araña." }),
                W("Kim Kardashian", new[] { "Reality." }),
                W("Elon Musk", new[] { "Cohete." }),
                W("Billie Eilish", new[] { "Susurro." }),
                W("Rihanna", new[] { "Fenty." }),
                W("Justin Bieber", new[] { "Bebé." }),
                W("Drake", new[] { "Canadá." }),
                W("Lady Gaga", new[] { "Ropa." }),
                W("Brad Pitt", new[] { "Facha." }),
                W("Angelina Jolie", new[] { "Juego." }),
                W("Ed Sheeran", new[] { "Colorado." })
            ),

            CreateCategory("Países",
                W("Argentina", new[] { "Mate." }),
                W("Brasil", new[] { "Carnaval." }),
                W("Chile", new[] { "Cordillera." }),
                W("Uruguay", new[] { "Termo." }),
                W("Paraguay", new[] { "Albañil." }),
                W("Bolivia", new[] { "Altura." }),
                W("Perú", new[] { "Ruinas." }),
                W("Colombia", new[] { "Fafa." }),
                W("México", new[] { "Inseguro." }),
                W("Estados Unidos", new[] { "Estrellas." }),
                W("Canadá", new[] { "Maple." }),
                W("España", new[] { "Flamenco." }),
                W("Francia", new[] { "Torre." }),
                W("Italia", new[] { "Pasta." }),
                W("Alemania", new[] { "Autos." }),
                W("Japón", new[] { "Anime." }),
                W("China", new[] { "Muralla." }),
                W("Australia", new[] { "Animales." }),
                W("Egipto", new[] { "Pirámide." }),
                W("Sudáfrica", new[] { "Safari." })
            ),

            CreateCategory("Comidas",
                W("Pizza", new[] { "Queso." }),
                W("Hamburguesa", new[] { "Pan." }),
                W("Empanada", new[] { "Repulgue." }),
                W("Milanesa", new[] { "Empanado." }),
                W("Asado", new[] { "Parrilla." }),
                W("Sushi", new[] { "Arroz." }),
                W("Taco", new[] { "Tortilla." }),
                W("Pasta", new[] { "Salsa." }),
                W("Ravioles", new[] { "Relleno." }),
                W("Ñoquis", new[] { "Papa." }),
                W("Helado", new[] { "Frío." }),
                W("Torta", new[] { "Vela." }),
                W("Pancho", new[] { "Salchicha." }),
                W("Lomito", new[] { "Sándwich." }),
                W("Choripán", new[] { "Chorizo." }),
                W("Ensalada", new[] { "Verde." }),
                W("Pollo frito", new[] { "Crocante." }),
                W("Papas fritas", new[] { "Bastón." }),
                W("Arroz", new[] { "Grano." }),
                W("Lasagna", new[] { "Capas." })
            ),

            CreateCategory("Objetos",
                W("Celular", new[] { "Pantalla." }),
                W("Computadora", new[] { "Trabajo." }),
                W("Teclado", new[] { "Teclas." }),
                W("Mouse", new[] { "Cursor." }),
                W("Monitor", new[] { "Imagen." }),
                W("Mesa", new[] { "Apoyo." }),
                W("Silla", new[] { "Sentarse." }),
                W("Botella", new[] { "Agua." }),
                W("Vaso", new[] { "Bebida." }),
                W("Plato", new[] { "Comida." }),
                W("Cuchara", new[] { "Sopa." }),
                W("Tenedor", new[] { "Puntas." }),
                W("Cuchillo", new[] { "Corte." }),
                W("Mochila", new[] { "Espalda." }),
                W("Libro", new[] { "Páginas." }),
                W("Lápiz", new[] { "Grafito." }),
                W("Lapicera", new[] { "Tinta." }),
                W("Cuaderno", new[] { "Hojas." }),
                W("Auriculares", new[] { "Sonido." }),
                W("Control remoto", new[] { "Botones." }),
                W("Televisor", new[] { "Series." }),
                W("Lámpara", new[] { "Luz." }),
                W("Cama", new[] { "Sueño." }),
                W("Almohada", new[] { "Cabeza." }),
                W("Reloj", new[] { "Hora." }),
                W("Billetera", new[] { "Plata." }),
                W("Llaves", new[] { "Puerta." }),
                W("Paraguas", new[] { "Lluvia." }),
                W("Zapatilla", new[] { "Cordones." }),
                W("Campera", new[] { "Abrigo." }),
                W("Gorra", new[] { "Visera." }),
                W("Espejo", new[] { "Reflejo." }),
                W("Cepillo", new[] { "Cerdas." }),
                W("Toalla", new[] { "Seco." }),
                W("Jabón", new[] { "Espuma." }),
                W("Shampoo", new[] { "Pelo." }),
                W("Ventilador", new[] { "Aire." }),
                W("Heladera", new[] { "Frío." }),
                W("Microondas", new[] { "Rápido." }),
                W("Horno", new[] { "Calor." }),
                W("Sartén", new[] { "Fuego." }),
                W("Olla", new[] { "Agua." }),
                W("Tijera", new[] { "Papel." }),
                W("Cinta", new[] { "Pegamento." }),
                W("Caja", new[] { "Guardar." }),
                W("Bolsa", new[] { "Compras." }),
                W("Pelota", new[] { "Juego." }),
                W("Bicicleta", new[] { "Ruedas." }),
                W("Cargador", new[] { "Batería." }),
                W("Pendrive", new[] { "Archivos." })
            ),

            CreateCategory("Videojuegos",
                W("Minecraft", new[] { "Bloques." }, "Videojuego de mundo abierto donde los jugadores exploran, construyen y sobreviven usando bloques."),
                W("Fortnite", new[] { "Construcción." }),
                W("Roblox", new[] { "Mundos." }),
                W("Among Us", new[] { "Impostor." }),
                W("GTA", new[] { "Ciudad." }),
                W("The Sims", new[] { "Vida." }),
                W("FIFA", new[] { "Cancha." }),
                W("Call of Duty", new[] { "Guerra." }),
                W("Valorant", new[] { "Agentes." }),
                W("League of Legends", new[] { "Líneas." }),
                W("Counter Strike", new[] { "Bomba." }),
                W("Fall Guys", new[] { "Carrera." }),
                W("Mario Kart", new[] { "Kart." }),
                W("Zelda", new[] { "Trifuerza." }),
                W("Pokémon", new[] { "Criaturas." }),
                W("Resident Evil", new[] { "Zombies." }),
                W("Silent Hill", new[] { "Niebla." }),
                W("Elden Ring", new[] { "Anillo." }),
                W("God of War", new[] { "Hacha." }),
                W("The Last of Us", new[] { "Infectados." })
            ),

            CreateCategory("Animales",
                W("Perro", new[] { "Ladrido." }),
                W("Gato", new[] { "Maullido." }),
                W("León", new[] { "Melena." }),
                W("Tigre", new[] { "Rayas." }),
                W("Elefante", new[] { "Trompa." }),
                W("Jirafa", new[] { "Cuello." }),
                W("Mono", new[] { "Banana." }),
                W("Caballo", new[] { "Galope." }),
                W("Vaca", new[] { "Leche." }),
                W("Cerdo", new[] { "Granja." }),
                W("Gallina", new[] { "Huevos." }),
                W("Pato", new[] { "Cuac." }),
                W("Conejo", new[] { "Orejas." }),
                W("Oso", new[] { "Miel." }),
                W("Lobo", new[] { "Manada." }),
                W("Zorro", new[] { "Astuto." }),
                W("Delfín", new[] { "Salto." }),
                W("Tiburón", new[] { "Dientes." }),
                W("Pingüino", new[] { "Hielo." }),
                W("Cocodrilo", new[] { "Mandíbula." })
            ),

            CreateCategory("Películas",
                W("Titanic", new[] { "Barco." }),
                W("Avatar", new[] { "Azul." }),
                W("Toy Story", new[] { "Juguetes." }),
                W("Shrek", new[] { "Ogro." }),
                W("Frozen", new[] { "Hielo." }),
                W("Harry Potter", new[] { "Varita." }),
                W("El Señor de los Anillos", new[] { "Anillo." }),
                W("Star Wars", new[] { "Galaxia." }),
                W("Jurassic Park", new[] { "Dinosaurio." }),
                W("Spider-Man", new[] { "Telaraña." }),
                W("Batman", new[] { "Murciélago." }),
                W("Avengers", new[] { "Equipo." }),
                W("Iron Man", new[] { "Armadura." }),
                W("Coco", new[] { "Recuerdo." }),
                W("Buscando a Nemo", new[] { "Océano." }),
                W("Rápidos y Furiosos", new[] { "Autos." }),
                W("Matrix", new[] { "Código." }),
                W("El Rey León", new[] { "Selva." }),
                W("Intensamente", new[] { "Emoción." }),
                W("Barbie", new[] { "Rosa." })
            ),

            CreateCategory("Marcas",
                W("Nike", new[] { "Zapatilla." }),
                W("Adidas", new[] { "Rayas." }),
                W("Apple", new[] { "Manzana." }),
                W("Samsung", new[] { "Pantalla." }),
                W("Coca-Cola", new[] { "Rojo." }),
                W("Pepsi", new[] { "Azul." }),
                W("McDonald's", new[] { "Cajita." }),
                W("Burger King", new[] { "Corona." }),
                W("Netflix", new[] { "Streaming." }),
                W("Disney", new[] { "Ratón." }),
                W("Google", new[] { "Buscar." }),
                W("Microsoft", new[] { "Ventana." }),
                W("Amazon", new[] { "Paquete." }),
                W("PlayStation", new[] { "Control." }),
                W("Xbox", new[] { "Verde." }),
                W("Nintendo", new[] { "Bigote." }),
                W("Puma", new[] { "Felino." }),
                W("Gucci", new[] { "Lujo." }),
                W("Zara", new[] { "Ropa." }),
                W("Mercado Libre", new[] { "Compra." })
            ),

            CreateCategory("Profesiones",
                W("Médico", new[] { "Guardapolvo." }),
                W("Abogado", new[] { "Juicio." }),
                W("Profesor", new[] { "Aula." }),
                W("Programador", new[] { "Código." }),
                W("Diseñador", new[] { "Visual." }),
                W("Arquitecto", new[] { "Planos." }),
                W("Ingeniero", new[] { "Técnica." }),
                W("Cocinero", new[] { "Receta." }),
                W("Policía", new[] { "Sirena." }),
                W("Bombero", new[] { "Fuego." }),
                W("Veterinario", new[] { "Mascota." }),
                W("Psicólogo", new[] { "Mente." }),
                W("Contador", new[] { "Impuestos." }),
                W("Periodista", new[] { "Noticia." }),
                W("Actor", new[] { "Escena." }),
                W("Cantante", new[] { "Micrófono." }),
                W("Electricista", new[] { "Cable." }),
                W("Plomero", new[] { "Caño." }),
                W("Carpintero", new[] { "Madera." }),
                W("Peluquero", new[] { "Tijera." })
            ),

            CreateCategory("Superhéroes",
                W("Spider-Man", new[] { "Máscara" }),
                W("Batman", new[] { "Noche." }),
                W("Superman", new[] { "Volar." }),
                W("Wonder Woman", new[] { "Látigo." }),
                W("Iron Man", new[] { "Genio." }),
                W("Captain America", new[] { "Escudo." }),
                W("Thor", new[] { "Martillo." }),
                W("Hulk", new[] { "Verde." }),
                W("Black Widow", new[] { "Insecto." }),
                W("Doctor Strange", new[] { "Capa." }),
                W("Black Panther", new[] { "Animal." }),
                W("Flash", new[] { "Velocidad." }),
                W("Aquaman", new[] { "Tridente." }),
                W("Wolverine", new[] { "Garras." }),
                W("Deadpool", new[] { "Sarcasmo." }),
                W("Ant-Man", new[] { "Chiquito." }),
                W("Captain Marvel", new[] { "Energía." }),
                W("Green Lantern", new[] { "Anillo." }),
                W("Daredevil", new[] { "Vista." }),
                W("Venom", new[] { "Negro." })
            ),
            new WordCategory
            {
                CategoryName = "Jugadores",
                CategoryType = CategoryType.PlayerNames
            }
        };
    }

    private WordCategory CreateCategory(string categoryName, params WordData[] words)
    {
        return new WordCategory
        {
            CategoryName = categoryName,
            Words = new List<WordData>(words)
        };
    }

    private WordData W(string word, string[] hints, string description = "")
    {
        return new WordData
        {
            Word = word,
            Hints = hints.ToList(),
            Description = description
        };
    }

    public WordCategory GetCategory(string categoryName)
    {
        return categories.Find(c => c.CategoryName == categoryName);
    }
}
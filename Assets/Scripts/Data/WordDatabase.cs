using System.Collections.Generic;
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
        categories = new List<WordCategory>
        {
            CreateCategory("Famosos argentinos",
                W("Messi", "Pelota.", "Lionel Messi es un futbolista argentino, considerado uno de los mejores jugadores de la historia."),
                W("Maradona", "Diez."),
                W("Lali Espósito", "Pop."),
                W("Tini Stoessel", "Disney."),
                W("Duki", "Trap."),
                W("Bizarrap", "Sesión."),
                W("Ricardo Darín", "Cine."),
                W("Guillermo Francella", "Racing."),
                W("Susana Giménez", "Teléfono."),
                W("Mirtha Legrand", "Almuerzo."),
                W("Charly García", "Piano."),
                W("Fito Páez", "Rosario."),
                W("Gustavo Cerati", "Soda."),
                W("Abel Pintos", "Pelado."),
                W("Nicki Nicole", "Petisa."),
                W("Wanda Nara", "Show."),
                W("Moria Casán", "Rating."),
                W("Paulo Dybala", "Joya."),
                W("Ángel Di María", "Pasta."),
                W("Ricardo Ford", "Rol Royce.")
            ),

            CreateCategory("Famosos globales",
                W("Taylor Swift", "Eras."),
                W("Cristiano Ronaldo", "Ego."),
                W("Dwayne Johnson", "Piedra."),
                W("Beyoncé", "Corona."),
                W("Shakira", "Caderas."),
                W("Bad Bunny", "Maldad."),
                W("Ariana Grande", "Agudo."),
                W("Leonardo DiCaprio", "Oscar."),
                W("Tom Holland", "Araña."),
                W("Kim Kardashian", "Reality."),
                W("Elon Musk", "Cohete."),
                W("Billie Eilish", "Susurro."),
                W("Rihanna", "Fenty."),
                W("Justin Bieber", "Bebé."),
                W("Drake", "Canadá."),
                W("Lady Gaga", "Ropa."),
                W("Brad Pitt", "Facha."),
                W("Angelina Jolie", "Juego."),
                W("Ed Sheeran", "Colorado.")
            ),

            CreateCategory("Países",
                W("Argentina", "Mate."),
                W("Brasil", "Carnaval."),
                W("Chile", "Cordillera."),
                W("Uruguay", "Termo."),
                W("Paraguay", "Albañil."),
                W("Bolivia", "Altura."),
                W("Perú", "Ruinas."),
                W("Colombia", "Fafa."),
                W("México", "Inseguro."),
                W("Estados Unidos", "Estrellas."),
                W("Canadá", "Maple."),
                W("España", "Flamenco."),
                W("Francia", "Torre."),
                W("Italia", "Pasta."),
                W("Alemania", "Autos."),
                W("Japón", "Anime."),
                W("China", "Muralla."),
                W("Australia", "Animales."),
                W("Egipto", "Pirámide."),
                W("Sudáfrica", "Safari.")
            ),

            CreateCategory("Comidas",
                W("Pizza", "Queso."),
                W("Hamburguesa", "Pan."),
                W("Empanada", "Repulgue."),
                W("Milanesa", "Empanado."),
                W("Asado", "Parrilla."),
                W("Sushi", "Arroz."),
                W("Taco", "Tortilla."),
                W("Pasta", "Salsa."),
                W("Ravioles", "Relleno."),
                W("Ñoquis", "Papa."),
                W("Helado", "Frío."),
                W("Torta", "Vela."),
                W("Pancho", "Salchicha."),
                W("Lomito", "Sándwich."),
                W("Choripán", "Chorizo."),
                W("Ensalada", "Verde."),
                W("Pollo frito", "Crocante."),
                W("Papas fritas", "Bastón."),
                W("Arroz", "Grano."),
                W("Lasagna", "Capas.")
            ),

            CreateCategory("Objetos",
                W("Celular", "Pantalla."),
                W("Computadora", "Trabajo."),
                W("Teclado", "Teclas."),
                W("Mouse", "Cursor."),
                W("Monitor", "Imagen."),
                W("Mesa", "Apoyo."),
                W("Silla", "Sentarse."),
                W("Botella", "Agua."),
                W("Vaso", "Bebida."),
                W("Plato", "Comida."),
                W("Cuchara", "Sopa."),
                W("Tenedor", "Puntas."),
                W("Cuchillo", "Corte."),
                W("Mochila", "Espalda."),
                W("Libro", "Páginas."),
                W("Lápiz", "Grafito."),
                W("Lapicera", "Tinta."),
                W("Cuaderno", "Hojas."),
                W("Auriculares", "Sonido."),
                W("Control remoto", "Botones."),
                W("Televisor", "Series."),
                W("Lámpara", "Luz."),
                W("Cama", "Sueño."),
                W("Almohada", "Cabeza."),
                W("Reloj", "Hora."),
                W("Billetera", "Plata."),
                W("Llaves", "Puerta."),
                W("Paraguas", "Lluvia."),
                W("Zapatilla", "Cordones."),
                W("Campera", "Abrigo."),
                W("Gorra", "Visera."),
                W("Espejo", "Reflejo."),
                W("Cepillo", "Cerdas."),
                W("Toalla", "Seco."),
                W("Jabón", "Espuma."),
                W("Shampoo", "Pelo."),
                W("Ventilador", "Aire."),
                W("Heladera", "Frío."),
                W("Microondas", "Rápido."),
                W("Horno", "Calor."),
                W("Sartén", "Fuego."),
                W("Olla", "Agua."),
                W("Tijera", "Papel."),
                W("Cinta", "Pegamento."),
                W("Caja", "Guardar."),
                W("Bolsa", "Compras."),
                W("Pelota", "Juego."),
                W("Bicicleta", "Ruedas."),
                W("Cargador", "Batería."),
                W("Pendrive", "Archivos.")
            ),

            CreateCategory("Videojuegos",
                W("Minecraft", "Bloques.", "Videojuego de mundo abierto donde los jugadores exploran, construyen y sobreviven usando bloques."),
                W("Fortnite", "Construcción."),
                W("Roblox", "Mundos."),
                W("Among Us", "Impostor."),
                W("GTA", "Ciudad."),
                W("The Sims", "Vida."),
                W("FIFA", "Cancha."),
                W("Call of Duty", "Guerra."),
                W("Valorant", "Agentes."),
                W("League of Legends", "Líneas."),
                W("Counter Strike", "Bomba."),
                W("Fall Guys", "Carrera."),
                W("Mario Kart", "Kart."),
                W("Zelda", "Trifuerza."),
                W("Pokémon", "Criaturas."),
                W("Resident Evil", "Zombies."),
                W("Silent Hill", "Niebla."),
                W("Elden Ring", "Anillo."),
                W("God of War", "Hacha."),
                W("The Last of Us", "Infectados.")
            ),

            CreateCategory("Animales",
                W("Perro", "Ladrido."),
                W("Gato", "Maullido."),
                W("León", "Melena."),
                W("Tigre", "Rayas."),
                W("Elefante", "Trompa."),
                W("Jirafa", "Cuello."),
                W("Mono", "Banana."),
                W("Caballo", "Galope."),
                W("Vaca", "Leche."),
                W("Cerdo", "Granja."),
                W("Gallina", "Huevos."),
                W("Pato", "Cuac."),
                W("Conejo", "Orejas."),
                W("Oso", "Miel."),
                W("Lobo", "Manada."),
                W("Zorro", "Astuto."),
                W("Delfín", "Salto."),
                W("Tiburón", "Dientes."),
                W("Pingüino", "Hielo."),
                W("Cocodrilo", "Mandíbula.")
            ),

            CreateCategory("Películas",
                W("Titanic", "Barco."),
                W("Avatar", "Azul."),
                W("Toy Story", "Juguetes."),
                W("Shrek", "Ogro."),
                W("Frozen", "Hielo."),
                W("Harry Potter", "Varita."),
                W("El Señor de los Anillos", "Anillo."),
                W("Star Wars", "Galaxia."),
                W("Jurassic Park", "Dinosaurio."),
                W("Spider-Man", "Telaraña."),
                W("Batman", "Murciélago."),
                W("Avengers", "Equipo."),
                W("Iron Man", "Armadura."),
                W("Coco", "Recuerdo."),
                W("Buscando a Nemo", "Océano."),
                W("Rápidos y Furiosos", "Autos."),
                W("Matrix", "Código."),
                W("El Rey León", "Selva."),
                W("Intensamente", "Emoción."),
                W("Barbie", "Rosa.")
            ),

            CreateCategory("Marcas",
                W("Nike", "Zapatilla."),
                W("Adidas", "Rayas."),
                W("Apple", "Manzana."),
                W("Samsung", "Pantalla."),
                W("Coca-Cola", "Rojo."),
                W("Pepsi", "Azul."),
                W("McDonald's", "Cajita."),
                W("Burger King", "Corona."),
                W("Netflix", "Streaming."),
                W("Disney", "Ratón."),
                W("Google", "Buscar."),
                W("Microsoft", "Ventana."),
                W("Amazon", "Paquete."),
                W("PlayStation", "Control."),
                W("Xbox", "Verde."),
                W("Nintendo", "Bigote."),
                W("Puma", "Felino."),
                W("Gucci", "Lujo."),
                W("Zara", "Ropa."),
                W("Mercado Libre", "Compra.")
            ),

            CreateCategory("Profesiones",
                W("Médico", "Guardapolvo."),
                W("Abogado", "Juicio."),
                W("Profesor", "Aula."),
                W("Programador", "Código."),
                W("Diseñador", "Visual."),
                W("Arquitecto", "Planos."),
                W("Ingeniero", "Técnica."),
                W("Cocinero", "Receta."),
                W("Policía", "Sirena."),
                W("Bombero", "Fuego."),
                W("Veterinario", "Mascota."),
                W("Psicólogo", "Mente."),
                W("Contador", "Impuestos."),
                W("Periodista", "Noticia."),
                W("Actor", "Escena."),
                W("Cantante", "Micrófono."),
                W("Electricista", "Cable."),
                W("Plomero", "Caño."),
                W("Carpintero", "Madera."),
                W("Peluquero", "Tijera.")
            ),

            CreateCategory("Superhéroes",
                W("Spider-Man", "Máscara"),
                W("Batman", "Noche."),
                W("Superman", "Volar."),
                W("Wonder Woman", "Látigo."),
                W("Iron Man", "Genio."),
                W("Captain America", "Escudo."),
                W("Thor", "Martillo."),
                W("Hulk", "Verde."),
                W("Black Widow", "Insecto."),
                W("Doctor Strange", "Capa."),
                W("Black Panther", "Animal."),
                W("Flash", "Velocidad."),
                W("Aquaman", "Tridente."),
                W("Wolverine", "Garras."),
                W("Deadpool", "Sarcasmo."),
                W("Ant-Man", "Chiquito."),
                W("Captain Marvel", "Energía."),
                W("Green Lantern", "Anillo."),
                W("Daredevil", "Vista."),
                W("Venom", "Negro.")
            )
        };

        categories.Add(new WordCategory
        {
            CategoryName = "Jugadores",
            CategoryType = CategoryType.PlayerNames
        });
    }

    private WordCategory CreateCategory(string categoryName, params WordData[] words)
    {
        return new WordCategory
        {
            CategoryName = categoryName,
            Words = new List<WordData>(words)
        };
    }

    private WordData W(string word, string hint, string description = "")
    {
        return new WordData
        {
            Word = word,
            Hint = hint,
            Description = description
        };
    }

    public WordCategory GetCategory(string categoryName)
    {
        return categories.Find(c => c.CategoryName == categoryName);
    }
}
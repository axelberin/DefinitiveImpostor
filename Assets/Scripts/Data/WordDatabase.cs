using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Impostor/Word Database")]
public class WordDatabase : ScriptableObject
{
    [SerializeField] private List<WordCategory> categories = new();
    [SerializeField] private List<CategoryVisualData> categoryVisualData = new();

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

    private CategoryVisualData GetVisualData(string categoryName)
    {
        return categoryVisualData.Find(x => x.CategoryName == categoryName);
    }

    private void LoadDefaultCategories()
    {
        categories = new()
        {
            CreateCategory("Famosos argentinos", CategoryType.Normal,
            W("Messi", new[] { "Pelota.", "8" }, "Lionel Messi es un futbolista argentino, considerado uno de los mejores jugadores de la historia."),
            W("Maradona", new[] { "Doping", "Mujeriego" }),
            W("Lali Espósito", new[] { "Pop", "Anti Milei" }),
            W("Tini Stoessel", new[] { "Disney", "Amante" }),
            W("Duki", new[] { "Rockstar", "Tatuajes" }),
            W("Carito Muller", new[] { "Tríos", "Relación abierta" }, "Influencer"),
            W("Ricardo Darín", new[] { "Cine", "Empanadas" }),
            W("Guillermo Francella", new[] { "Racing", "Edificio" }),
            W("Susana Giménez", new[] { "Teléfono", "Cómica" }),
            W("Mirtha Legrand", new[] { "Dinosaurio", "Polémica" }),
            W("Charly García", new[] { "Piano", "Noveno piso" }),
            W("China Suarez", new[] { "Roba cunas", "Hombres" }),
            W("Pampita", new[] { "Perfecta", "Cornuda" }),
            W("Abel Pintos", new[] { "Pelado", "Gay" }),
            W("Albere", new[] { "Villeros", "Embarazo" }, "Influencer"),
            W("Wanda Nara", new[] { "Show", "Bananero" }),
            W("Moria Casán", new[] { "Rating", "Memes" }),
            W("Julián Álvares", new[] { "Campeón" }),
            W("Ángel Di María", new[] { "Pasta", "Narigón" }),
            W("Bananero", new[] { "Semen", "Cerveza" }),
            W("Ricardo Ford", new[] { "Rol Royce", "Miami" })
        ),

            CreateCategory("Famosos globales", CategoryType.Normal,
            W("Cristiano Ronaldo", new[] { "Ego.", "Millonario" }),
            W("Mia Khalifa", new[] { "Anteojos .", "Actriz" }),
            W("Shakira", new[] { "Caderas.", "Cornuda" }),
            W("Bad Bunny", new[] { "Malo", "Latino" }),
            W("Tom Holland", new[] { "Araña." }),
            W("Kim Kardashian", new[] { "Reality." }),
            W("Elon Musk", new[] { "Cohete." }),
            W("Jacob Elordi", new[] { "Besos", "Papucho" }),
            W("Rubius", new[] { "Twich", "Gatos" }),
            W("Justin Bieber", new[] { "Bebé." }),
            W("Germán Garmendia", new[] { "Saludo", "Chile" }),
            W("Lady Gaga", new[] { "Ropa." }),
            W("Brad Pitt", new[] { "Facha." }),
            W("Angelina Jolie", new[] { "Juego." }),
            W("Ed Sheeran", new[] { "Colorado." })
        ),

        CreateCategory("Países", CategoryType.Normal,
            W("Argentina", new[] { "Mate.", "Piola" }),
            W("Brasil", new[] { "Carnaval.", "Joda" }),
            W("Chile", new[] { "Cordillera.", "Pasillo" }),
            W("Uruguay", new[] { "Termo.", "Rebelde" }),
            W("Paraguay", new[] { "Albañil.", "Idioma" }),
            W("Bolivia", new[] { "Altura.", "Verdulería" }),
            W("Perú", new[] { "Ruinas.", "Limón" }),
            W("Colombia", new[] { "Fafa.", "Acento" }),
            W("México", new[] { "Inseguro.", "Narcos" }),
            W("Estados Unidos", new[] { "Estrellas.", "Azul" }),
            W("Canadá", new[] { "Maple.", "Hoja" }),
            W("España", new[] { "Flamenco.", "Youtubers" }),
            W("Francia", new[] { "Torre.", "amor" }),
            W("Italia", new[] { "Pasta.", "Costa" }),
            W("Alemania", new[] { "Autos.", "Historia" }),
            W("Japón", new[] { "Anime.", "Autos" }),
            W("China", new[] { "Muralla.", "Sucios" }),
            W("Australia", new[] { "Animales.", "Arañas" }),
            W("Egipto", new[] { "Pirámide.", "Arena" }),
            W("Sudáfrica", new[] { "Safari.", "Negros" })
        ),

        CreateCategory("Comidas", CategoryType.Normal,
            W("Pizza", new[] { "Queso." }),
            W("Hamburguesa", new[] { "Pan." }),
            W("Empanada", new[] { "Salta" }),
            W("Milanesa", new[] { "Empanado." }),
            W("Asado", new[] { "Domingo" }),
            W("Sushi", new[] { "Arroz." }),
            W("Taco", new[] { "Tortilla." }),
            W("Pasta", new[] { "Salsa." }),
            W("Ravioles", new[] { "Relleno." }),
            W("Ñoquis", new[] { "Relieve" }),
            W("Helado", new[] { "Frío." }),
            W("Torta", new[] { "Vela." }),
            W("Pancho", new[] { "Yanquee" }),
            W("Lomito", new[] { "Sándwich." }),
            W("Choripán", new[] { "Ruta" }),
            W("Ensalada", new[] { "Verde." }),
            W("Pollo frito", new[] { "Crocante." }),
            W("Papas fritas", new[] { "Bastón." }),
            W("Arroz", new[] { "Grano." }),
            W("Lasagna", new[] { "Capas." })
        ),

        CreateCategory("Objetos", CategoryType.Normal,
            W("Celular", new[] { "Vicio" }),
            W("Computadora", new[] { "Trabajo." }),
            W("Teclado", new[] { "Luces" }),
            W("Mouse", new[] { "Roedor" }),
            W("Monitor", new[] { "Imagen." }),
            W("Mesa", new[] { "Apoyo." }),
            W("Silla", new[] { "Trono" }),
            W("Botella", new[] { "Gimnasio" }),
            W("Vaso", new[] { "Vidrio" }),
            W("Plato", new[] { "Volador" }),
            W("Cuchara", new[] { "Sopa." }),
            W("Tenedor", new[] { "Punta." }),
            W("Cuchillo", new[] { "Asesinato" }),
            W("Mochila", new[] { "Espalda." }),
            W("Libro", new[] { "Hobbie" }),
            W("Lápiz", new[] { "Grafito." }),
            W("Lapicera", new[] { "Escuela" }),
            W("Cuaderno", new[] { "Escuela" }),
            W("Auriculares", new[] { "Sonido." }),
            W("Control remoto", new[] { "Sensor" }),
            W("Televisor", new[] { "Series." }),
            W("Lámpara", new[] { "Luz." }),
            W("Cama", new[] { "Sexo" }),
            W("Almohada", new[] { "Cabeza." }),
            W("Reloj", new[] { "Lujo." }),
            W("Billetera", new[] { "Robo" }),
            W("Llaves", new[] { "Perdida" }),
            W("Paraguas", new[] { "Paraguay" }),
            W("Zapatilla", new[] { "Cable." }),
            W("Campera", new[] { "Frío." }),
            W("Gorra", new[] { "Ropa." }),
            W("Espejo", new[] { "Vestidor" }),
            W("Cepillo", new[] { "Cerdas." }),
            W("Toalla", new[] { "Pileta" }),
            W("Jabón", new[] { "Limpio" }),
            W("Shampoo", new[] { "Ducha" }),
            W("Ventilador", new[] { "Viento" }),
            W("Heladera", new[] { "Frío." }),
            W("Microondas", new[] { "Cocina" }),
            W("Horno", new[] { "Calor." }),
            W("Sartén", new[] { "Fuego." }),
            W("Olla", new[] { "Agua." }),
            W("Tijera", new[] { "Papel.", "Piedra" }),
            W("Cinta", new[] { "Círculo" }),
            W("Caja", new[] { "Guardar." }),
            W("Bolsa", new[] { "Guardar" }),
            W("Pelota", new[] { "Juego." }),
            W("Bicicleta", new[] { "Distancia" }),
            W("Cargador", new[] { "Energía." }),
            W("Pendrive", new[] { "Archivos.", "Enchufar" })
        ),

        CreateCategory("Videojuegos", CategoryType.Normal,
            W("Minecraft", new[] { "Bloques.", "Mundos" }, "Videojuego de mundo abierto donde los jugadores exploran, construyen y sobreviven usando bloques."),
            W("Fortnite", new[] { "Construcción." }, "Fortnite es un videojuego multijugador de acción y supervivencia conocido por su modo battle royale y construcción."),
            W("Roblox", new[] { "Variedad" }, "Roblox es una plataforma donde usuarios crean, comparten y juegan experiencias interactivas."),
            W("Among Us", new[] { "Impostor." }, "Among Us es un juego social de deducción donde tripulantes intentan descubrir impostores."),
            W("GTA", new[] { "Ciudad.", "Armas" }, "GTA es una saga de videojuegos de mundo abierto centrada en crimen, conducción y exploración urbana."),
            W("The Sims", new[] { "Vida." }, "The Sims es una saga de simulación social donde se crean personajes, casas y rutinas de vida."),
            W("FIFA", new[] { "Cancha." }, "FIFA es una saga de videojuegos de fútbol con equipos, jugadores y competiciones oficiales."),
            W("Call of Duty", new[] { "Guerra." }, "Call of Duty es una saga de videojuegos de disparos en primera persona con temática bélica y multijugador."),
            W("Valorant", new[] { "Agentes.", "Táctica" }, "Valorant es un shooter táctico competitivo donde personajes con habilidades luchan por objetivos."),
            W("League of Legends", new[] { "Líneas." }, "League of Legends es un MOBA competitivo donde equipos intentan destruir la base rival."),
            W("Counter Strike", new[] { "Bomba." }, "Counter-Strike es un shooter táctico competitivo basado en rondas entre terroristas y antiterroristas."),
            W("Fall Guys", new[] { "Carrera." }, "Fall Guys es un juego multijugador de obstáculos donde personajes compiten en pruebas eliminatorias."),
            W("Mario Kart", new[] { "Italia" }, "Mario Kart es una saga de carreras arcade con personajes de Nintendo y objetos especiales."),
            W("Zelda", new[] { "Héroe" }, "Zelda es una saga de aventuras y acción protagonizada principalmente por Link en mundos de fantasía."),
            W("Pokémon", new[] { "Criaturas." }, "Pokémon es una franquicia de videojuegos donde se capturan, entrenan y combaten criaturas."),
            W("Resident Evil", new[] { "Zombies." }, "Resident Evil es una saga de terror y acción centrada en brotes, monstruos y supervivencia."),
            W("Silent Hill", new[] { "Niebla." }, "Silent Hill es una saga de terror psicológico conocida por su atmósfera opresiva y simbolismo."),
            W("Elden Ring", new[] { "Anillo." }, "Elden Ring es un RPG de acción en mundo abierto con combate desafiante y fantasía oscura."),
            W("God of War", new[] { "Hacha." }, "God of War es una saga de acción protagonizada por Kratos, con combates basados en mitologías."),
            W("The Last of Us", new[] { "Infectados." }, "The Last of Us es una saga de acción narrativa ambientada en un mundo postapocalíptico con infectados.")
        ),

        CreateCategory("Animales", CategoryType.Normal,
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
        
        CreateCategory("Películas", CategoryType.Normal,
            W("Titanic", new[] { "Barco.", "Amor" }, "Titanic es una película dramática y romántica sobre el famoso naufragio del transatlántico."),
            W("Avatar", new[] { "Azul.", "Conexión" }, "Avatar es una película de ciencia ficción ambientada en Pandora, un mundo habitado por los Na'vi."),
            W("Toy Story", new[] { "Suela", "Fiel" }, "Toy Story es una película animada sobre juguetes que cobran vida cuando los humanos no los ven."),
            W("Shrek", new[] { "Princesa", "Cuernos" }, "Shrek es una película animada que parodia los cuentos de hadas con humor irreverente."),
            W("Frozen", new[] { "Hielo.", "Frio" }, "Frozen es una película animada de Disney sobre dos hermanas, magia de hielo y aceptación personal."),
            W("Harry Potter", new[] { "Varita." }, "Harry Potter es una saga de fantasía sobre un joven mago y su lucha contra fuerzas oscuras."),
            W("El Señor de los Anillos", new[] { "Anillo." }, "El Señor de los Anillos es una saga fantástica sobre una misión para destruir un anillo poderoso."),
            W("Star Wars", new[] { "Galaxia." }, "Star Wars es una saga de ciencia ficción y aventura sobre conflictos galácticos y el poder de la Fuerza."),
            W("Jurassic Park", new[] { "Dinosaurio." }, "Jurassic Park es una película de aventura y ciencia ficción sobre dinosaurios recreados genéticamente."),
            W("Avengers", new[] { "Equipo." }, "Avengers es una saga de superhéroes donde varios personajes se unen para enfrentar amenazas globales o cósmicas."),
            W("Coco", new[] { "Recuerdo." }, "Coco es una película animada sobre familia, música y la tradición mexicana del Día de Muertos."),
            W("Buscando a Nemo", new[] { "Océano." }, "Buscando a Nemo es una película animada sobre un pez payaso que busca a su hijo en el océano."),
            W("Rápidos y Furiosos", new[] { "Autos." }, "Rápidos y Furiosos es una saga de acción centrada en carreras, autos, robos y familia."),
            W("Matrix", new[] { "Código." }, "Matrix es una película de ciencia ficción sobre una realidad simulada y la rebelión contra máquinas."),
            W("El Rey León", new[] { "Selva." }, "El Rey León es una película animada sobre un león joven que debe aceptar su lugar como rey."),
            W("Intensamente", new[] { "Emoción." }, "Intensamente es una película animada que representa las emociones dentro de la mente de una niña."),
            W("Donde están las rubias?", new[] { "Baile", "Negros" }, "Comedia en la que dos agentes del FBI se hacen pasar por dos jóvenes millonarias para resolver un caso y protegerlas de un posible secuestro."),
            W("50 sombras de Grey", new[] { "Sexo" }, "Película romántica y dramática centrada en la relación entre Anastasia Steele y Christian Grey, un empresario misterioso con gustos poco convencionales"),
            W("Barbie", new[] { "Rosa." }, "Barbie es una película que mezcla comedia y crítica social a partir del universo de la famosa muñeca.")
        ),
        
        CreateCategory("Marcas", CategoryType.Normal,
            W("Nike", new[] { "Zapatilla." }, "Nike es una marca global de indumentaria, calzado y artículos deportivos."),
            W("Adidas", new[] { "Rayas." }, "Adidas es una marca alemana de ropa, calzado y equipamiento deportivo."),
            W("Apple", new[] { "Manzana." }, "Apple es una empresa tecnológica conocida por sus dispositivos, software y diseño integrado."),
            W("Samsung", new[] { "Pantalla." }, "Samsung es una empresa surcoreana que fabrica celulares, televisores, electrodomésticos y tecnología."),
            W("Coca-Cola", new[] { "Rojo." }, "Coca-Cola es una marca de bebida gaseosa reconocida mundialmente por su identidad roja y sabor característico."),
            W("Pepsi", new[] { "Azul." }, "Pepsi es una marca de bebida gaseosa cola y competidora histórica de Coca-Cola."),
            W("McDonald's", new[] { "Cajita." }, "McDonald's es una cadena internacional de comida rápida famosa por hamburguesas y papas fritas."),
            W("Burger King", new[] { "Corona." }, "Burger King es una cadena internacional de comida rápida conocida por sus hamburguesas a la parrilla."),
            W("Netflix", new[] { "Streaming." }, "Netflix es una plataforma de streaming de películas, series y documentales."),
            W("Disney", new[] { "Ratón." }, "Disney es una compañía de entretenimiento conocida por animación, parques, películas y personajes icónicos."),
            W("Google", new[] { "Buscar." }, "Google es una empresa tecnológica conocida por su buscador, servicios digitales y productos de software."),
            W("Microsoft", new[] { "Ventana." }, "Microsoft es una empresa tecnológica conocida por Windows, Office, Xbox y servicios en la nube."),
            W("Amazon", new[] { "Paquete." }, "Amazon es una empresa de comercio electrónico, servicios en la nube y entretenimiento digital."),
            W("PlayStation", new[] { "Control." }, "PlayStation es una marca de consolas y videojuegos desarrollada por Sony."),
            W("Xbox", new[] { "Verde." }, "Xbox es una marca de consolas, videojuegos y servicios creada por Microsoft."),
            W("Nintendo", new[] { "Bigote." }, "Nintendo es una empresa japonesa de videojuegos conocida por sus consolas y personajes icónicos."),
            W("Puma", new[] { "Felino." }, "Puma es una marca alemana de indumentaria, calzado y accesorios deportivos."),
            W("Gucci", new[] { "Lujo." }, "Gucci es una marca italiana de moda de lujo conocida por ropa, accesorios y diseño distintivo."),
            W("Zara", new[] { "Ropa." }, "Zara es una marca española de indumentaria conocida por moda rápida y presencia internacional."),
            W("Mercado Libre", new[] { "Compra." }, "Mercado Libre es una plataforma latinoamericana de comercio electrónico y pagos digitales.")
        ),
        
        CreateCategory("Profesiones", CategoryType.Normal,
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
        
        CreateCategory("Superhéroes", CategoryType.Normal,
            W("Spider-Man", new[] { "Máscara" }),
            W("Batman", new[] { "Animal" }, "Spider-Man es un superhéroe de Marvel con poderes arácnidos, agilidad y sentido de peligro."),
            W("Superman", new[] { "Volar." }, "Batman es un héroe de DC que combate el crimen usando entrenamiento, tecnología e inteligencia."),
            W("Wonder Woman", new[] { "Látigo." }, "Superman es un superhéroe de DC con fuerza, vuelo y poderes derivados del sol amarillo."),
            W("Iron Man", new[] { "Genio." }, "Wonder Woman es una heroína de DC, princesa amazona y guerrera con habilidades sobrehumanas."),
            W("Captain America", new[] { "Escudo." }, "Iron Man es un héroe de Marvel que usa armaduras tecnológicas creadas por Tony Stark."),
            W("Thor", new[] { "Dios" }, "Captain America es un héroe de Marvel, símbolo de justicia y resultado de un experimento militar."),
            W("Hulk", new[] { "Verde." }, "Thor es un héroe de Marvel inspirado en la mitología nórdica, asociado al trueno y a un arma mágica."),
            W("Black Widow", new[] { "Insecto." }, "Hulk es un héroe de Marvel que se transforma en una criatura enorme y fuerte cuando se enfurece."),
            W("Doctor Strange", new[] { "Capa." }, "Black Widow es una heroína de Marvel experta en espionaje, combate y operaciones encubiertas."),
            W("Black Panther", new[] { "Nación" }, "Doctor Strange es un héroe de Marvel que utiliza artes místicas para proteger la realidad."),
            W("Flash", new[] { "Velocidad." }, "Black Panther es un héroe de Marvel, rey de Wakanda y portador de un traje de vibranium."),
            W("Aquaman", new[] { "Tridente." }, "Flash es un héroe de DC con supervelocidad y conexión con la Speed Force."),
            W("Wolverine", new[] { "Garras.", "Sexy" }, "Aquaman es un héroe de DC, gobernante de Atlantis y capaz de comunicarse con vida marina."),
            W("Deadpool", new[] { "Sarcasmo." }, "Wolverine es un héroe de Marvel con regeneración, sentidos agudos y garras de adamantium."),
            W("Ant-Man", new[] { "Chiquito." }, "Deadpool es un antihéroe de Marvel conocido por su humor, regeneración y conciencia de estar en una ficción."),
            W("Captain Marvel", new[] { "Energía." }, "Ant-Man es un héroe de Marvel que puede reducir o aumentar su tamaño usando tecnología especial."),
            W("Green Lantern", new[] { "Intergaláctico" }, "Captain Marvel es una heroína de Marvel con fuerza, vuelo y poderes de energía cósmica."),
            W("Daredevil", new[] { "Vista." }, "Green Lantern es un héroe de DC que crea construcciones de energía usando un anillo de poder."),
            W("Venom", new[] { "Negro." }, "Daredevil es un héroe de Marvel con sentidos aumentados que combate el crimen en Hell's Kitchen.")
        ),
        
        CreateCategory("Hot", CategoryType.Normal,
            W("Kamasutra", new[] { "Pose" }, "Venom es un personaje de Marvel formado por la unión de un huésped humano y un simbionte alienígena."),
            W("Dildo", new[] { "Grande" }),
            W("Vibrador", new[] { "Negro." }),
            W("Muñeca inflable", new[] { "Hombres" }),
            W("Porno", new[] { "Gay" }),
            W("Vagina", new[] { "Olor" }),
            W("Pene", new[] { "Medida" }),
            W("Ano", new[] { "Placer" }),
            W("Nude", new[] { "Snapchat" }),
            W("Tetas", new[] { "Agarrar" }),
            W("Orgía", new[] { "Amigos" }),
            W("Chupón", new[] { "Marca" })
        ),
        
        CreateCategory("Funable", CategoryType.Normal,
            W("Epstein", new[] { "Nenes" }),
            W("Judíos", new[] { "Gorro" }),
            W("Pedofilia", new[] { "Iglesia" }),
            W("Zoofilia", new[] { "Zoológico" }),
            W("Necrofilia", new[] { "Morgue" })
        ),
            CreateCategory("Jugadores", CategoryType.PlayerNames)
        };
    }

    private WordCategory CreateCategory(string name, CategoryType categoryType, params WordData[] words)
    {
        CategoryVisualData visualData = GetVisualData(name);

        return new WordCategory
        {
            CategoryName = name,

            Description = visualData != null
                ? visualData.Description
                : "Sin descripción",

            CategoryColor = visualData != null
                ? visualData.CategoryColor
                : Color.gray,

            CategoryImage = visualData != null
                ? visualData.CategoryImage
                : null,

            CategoryType = categoryType,
            Words = words.ToList()
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

[System.Serializable]
public class CategoryVisualData
{
    public string CategoryName;

    [TextArea]
    public string Description;

    public Color CategoryColor = Color.white;

    public Sprite CategoryImage;
}
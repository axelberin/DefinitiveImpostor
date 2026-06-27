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
            W("Minecraft", new[] { "Bloques.", "Mundos" }, "Mundo abierto donde los jugadores exploran, construyen y sobreviven usando bloques."),
            W("Fortnite", new[] { "Construcción." }, "Multijugador de acción y supervivencia conocido por su modo battle royale y construcción."),
            W("Roblox", new[] { "Variedad" }, "Plataforma donde usuarios crean, comparten y juegan experiencias interactivas."),
            W("Among Us", new[] { "Impostor." }, "Juego social de deducción donde tripulantes intentan descubrir impostores."),
            W("GTA", new[] { "Ciudad.", "Armas" }, "Saga de videojuegos de mundo abierto centrada en crimen, conducción y exploración urbana."),
            W("The Sims", new[] { "Vida." }, "Saga de simulación social donde se crean personajes, casas y rutinas de vida."),
            W("FIFA", new[] { "Cancha." }, "Saga de videojuegos de fútbol con equipos, jugadores y competiciones oficiales."),
            W("Call of Duty", new[] { "Guerra." }, "Saga de videojuegos de disparos en primera persona con temática bélica y multijugador."),
            W("Valorant", new[] { "Agentes.", "Táctica" }, "Shooter táctico competitivo donde personajes con habilidades luchan por objetivos."),
            W("League of Legends", new[] { "Líneas." }, "MOBA competitivo donde equipos intentan destruir la base rival."),
            W("Counter Strike", new[] { "Bomba." }, "Shooter táctico competitivo basado en rondas entre terroristas y antiterroristas."),
            W("Fall Guys", new[] { "Carrera." }, "Multijugador de obstáculos donde personajes compiten en pruebas eliminatorias."),
            W("Mario Kart", new[] { "Italia" }, "Saga de carreras arcade con personajes de Nintendo y objetos especiales."),
            W("Zelda", new[] { "Héroe" }, "Saga de aventuras y acción protagonizada principalmente por Link en mundos de fantasía."),
            W("Pokémon", new[] { "Criaturas." }, "Franquicia de videojuegos donde se capturan, entrenan y combaten criaturas."),
            W("Resident Evil", new[] { "Zombies." }, "Saga de terror y acción centrada en brotes, monstruos y supervivencia."),
            W("Silent Hill", new[] { "Niebla." }, "Saga de terror psicológico conocida por su atmósfera opresiva y simbolismo."),
            W("Elden Ring", new[] { "Anillo." }, "RPG de acción en mundo abierto con combate desafiante y fantasía oscura."),
            W("God of War", new[] { "Hacha." }, "Saga de acción protagonizada por Kratos, con combates basados en mitologías."),
            W("The Last of Us", new[] { "Infectados." }, "Saga de acción narrativa ambientada en un mundo postapocalíptico con infectados.")
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
            W("Titanic", new[] { "Barco.", "Amor" }, "Película dramática y romántica sobre el famoso naufragio del transatlántico."),
            W("Avatar", new[] { "Azul.", "Conexión" }, "Película de ciencia ficción ambientada en Pandora, un mundo habitado por los Na'vi."),
            W("Toy Story", new[] { "Suela", "Fiel" }, "Película animada sobre juguetes que cobran vida cuando los humanos no los ven."),
            W("Shrek", new[] { "Princesa", "Cuernos" }, "Película animada que parodia los cuentos de hadas con humor irreverente."),
            W("Frozen", new[] { "Hielo.", "Frio" }, "Película animada de Disney sobre dos hermanas, magia de hielo y aceptación personal."),
            W("Harry Potter", new[] { "Varita." }, "Saga de fantasía sobre un joven mago y su lucha contra fuerzas oscuras."),
            W("El Señor de los Anillos", new[] { "Anillo." }, "Saga fantástica sobre una misión para destruir un anillo poderoso."),
            W("Star Wars", new[] { "Galaxia." }, "Saga de ciencia ficción y aventura sobre conflictos galácticos y el poder de la Fuerza."),
            W("Jurassic Park", new[] { "Dinosaurio." }, "Película de aventura y ciencia ficción sobre dinosaurios recreados genéticamente."),
            W("Avengers", new[] { "Equipo." }, "Saga de superhéroes donde varios personajes se unen para enfrentar amenazas globales o cósmicas."),
            W("Coco", new[] { "Recuerdo." }, "Película animada sobre familia, música y la tradición mexicana del Día de Muertos."),
            W("Buscando a Nemo", new[] { "Océano." }, "Película animada sobre un pez payaso que busca a su hijo en el océano."),
            W("Rápidos y Furiosos", new[] { "Autos." }, "Saga de acción centrada en carreras, autos, robos y familia."),
            W("Matrix", new[] { "Código." }, "Película de ciencia ficción sobre una realidad simulada y la rebelión contra máquinas."),
            W("El Rey León", new[] { "Selva." }, "Película animada sobre un león joven que debe aceptar su lugar como rey."),
            W("Intensamente", new[] { "Emoción." }, "Película animada que representa las emociones dentro de la mente de una niña."),
            W("Donde están las rubias?", new[] { "Baile", "Negros" }, "Comedia en la que dos agentes del FBI se hacen pasar por dos jóvenes millonarias para resolver un caso y protegerlas de un posible secuestro."),
            W("50 sombras de Grey", new[] { "Sexo" }, "Película romántica y dramática centrada en la relación entre Anastasia Steele y Christian Grey, un empresario misterioso con gustos poco convencionales"),
            W("Barbie", new[] { "Rosa." }, "Película que mezcla comedia y crítica social a partir del universo de la famosa muñeca.")
        ),

        CreateCategory("Marcas", CategoryType.Normal,
            W("Nike", new[] { "Zapatilla." }, "Marca global de indumentaria, calzado y artículos deportivos."),
            W("Adidas", new[] { "Rayas." }, "Marca alemana de ropa, calzado y equipamiento deportivo."),
            W("Apple", new[] { "Manzana." }, "Empresa tecnológica conocida por sus dispositivos, software y diseño integrado."),
            W("Samsung", new[] { "Pantalla." }, "Empresa surcoreana que fabrica celulares, televisores, electrodomésticos y tecnología."),
            W("Coca-Cola", new[] { "Rojo." }, "Marca de bebida gaseosa reconocida mundialmente por su identidad roja y sabor característico."),
            W("Pepsi", new[] { "Azul." }, "Marca de bebida gaseosa cola y competidora histórica de Coca-Cola."),
            W("McDonald's", new[] { "Cajita." }, "Cadena internacional de comida rápida famosa por hamburguesas y papas fritas."),
            W("Burger King", new[] { "Corona." }, "Cadena internacional de comida rápida conocida por sus hamburguesas a la parrilla."),
            W("Netflix", new[] { "Streaming." }, "Plataforma de streaming de películas, series y documentales."),
            W("Disney", new[] { "Ratón." }, "Compañía de entretenimiento conocida por animación, parques, películas y personajes icónicos."),
            W("Google", new[] { "Buscar." }, "Empresa tecnológica conocida por su buscador, servicios digitales y productos de software."),
            W("Microsoft", new[] { "Ventana." }, "Empresa tecnológica conocida por Windows, Office, Xbox y servicios en la nube."),
            W("Amazon", new[] { "Paquete." }, "Empresa de comercio electrónico, servicios en la nube y entretenimiento digital."),
            W("PlayStation", new[] { "Control." }, "Marca de consolas y videojuegos desarrollada por Sony."),
            W("Xbox", new[] { "Verde." }, "Marca de consolas, videojuegos y servicios creada por Microsoft."),
            W("Nintendo", new[] { "Bigote." }, "Empresa japonesa de videojuegos conocida por sus consolas y personajes icónicos."),
            W("Puma", new[] { "Felino." }, "Marca alemana de indumentaria, calzado y accesorios deportivos."),
            W("Gucci", new[] { "Lujo." }, "Marca italiana de moda de lujo conocida por ropa, accesorios y diseño distintivo."),
            W("Zara", new[] { "Ropa." }, "Marca española de indumentaria conocida por moda rápida y presencia internacional."),
            W("Mercado Libre", new[] { "Compra." }, "Plataforma latinoamericana de comercio electrónico y pagos digitales.")
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
            W("Spider-Man", new[] { "Máscara" }, "Spider-Man es un superhéroe de Marvel con poderes arácnidos, agilidad y sentido de peligro."),
            W("Batman", new[] { "Animal" }, "Batman es un héroe de DC que combate el crimen usando entrenamiento, tecnología e inteligencia."),
            W("Superman", new[] { "Volar." }, "Superman es un superhéroe de DC con fuerza, vuelo y poderes derivados del sol amarillo."),
            W("Wonder Woman", new[] { "Látigo." }, "Wonder Woman es una heroína de DC, princesa amazona y guerrera con habilidades sobrehumanas."),
            W("Iron Man", new[] { "Genio." }, "Iron Man es un héroe de Marvel que usa armaduras tecnológicas creadas por Tony Stark."),
            W("Captain America", new[] { "Escudo." }, "Captain America es un héroe de Marvel, símbolo de justicia y resultado de un experimento militar."),
            W("Thor", new[] { "Dios" }, "Thor es un héroe de Marvel inspirado en la mitología nórdica, asociado al trueno y a un arma mágica."),
            W("Hulk", new[] { "Verde." }, "Hulk es un héroe de Marvel que se transforma en una criatura enorme y fuerte cuando se enfurece."),
            W("Black Widow", new[] { "Insecto." }, "Black Widow es una heroína de Marvel experta en espionaje, combate y operaciones encubiertas."),
            W("Doctor Strange", new[] { "Capa." }, "Doctor Strange es un héroe de Marvel que utiliza artes místicas para proteger la realidad."),
            W("Black Panther", new[] { "Nación" }, "Black Panther es un héroe de Marvel, rey de Wakanda y portador de un traje de vibranium."),
            W("Flash", new[] { "Velocidad." }, "Flash es un héroe de DC con supervelocidad y conexión con la Speed Force."),
            W("Aquaman", new[] { "Tridente." }, "Aquaman es un héroe de DC, gobernante de Atlantis y capaz de comunicarse con vida marina."),
            W("Wolverine", new[] { "Garras.", "Sexy" }, "Wolverine es un héroe de Marvel con regeneración, sentidos agudos y garras de adamantium."),
            W("Deadpool", new[] { "Sarcasmo." }, "Deadpool es un antihéroe de Marvel conocido por su humor, regeneración y conciencia de estar en una ficción."),
            W("Ant-Man", new[] { "Chiquito." }, "Ant-Man es un héroe de Marvel que puede reducir o aumentar su tamaño usando tecnología especial."),
            W("Captain Marvel", new[] { "Energía." }, "Captain Marvel es una heroína de Marvel con fuerza, vuelo y poderes de energía cósmica."),
            W("Green Lantern", new[] { "Intergaláctico" }, "Green Lantern es un héroe de DC que crea construcciones de energía usando un anillo de poder."),
            W("Daredevil", new[] { "Vista." }, "Daredevil es un héroe de Marvel con sentidos aumentados que combate el crimen en Hell's Kitchen."),
            W("Venom", new[] { "Negro." }, "Venom es un personaje de Marvel formado por la unión de un huésped humano y un simbionte alienígena.")
        ),

        CreateCategory("Hot", CategoryType.Normal,
            W("Kamasutra", new[] { "Pose" }),
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
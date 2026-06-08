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
                W("Messi", "Futbolista argentino campeón del mundo."),
                W("Maradona", "Ídolo argentino relacionado con el número 10."),
                W("Lali Espósito", "Cantante y actriz pop argentina."),
                W("Tini Stoessel", "Cantante argentina que empezó en Disney."),
                W("Duki", "Referente argentino del trap."),
                W("Bizarrap", "Productor argentino famoso por sus sesiones."),
                W("Ricardo Darín", "Actor argentino muy reconocido en cine."),
                W("Guillermo Francella", "Actor argentino de comedia y cine."),
                W("Susana Giménez", "Conductora argentina muy famosa."),
                W("Mirtha Legrand", "Conductora argentina histórica de almuerzos."),
                W("Charly García", "Músico argentino de rock nacional."),
                W("Fito Páez", "Cantautor rosarino de rock nacional."),
                W("Gustavo Cerati", "Cantante de Soda Stereo."),
                W("Abel Pintos", "Cantante argentino de música romántica/folklórica."),
                W("Nicki Nicole", "Cantante argentina de música urbana."),
                W("Wanda Nara", "Mediática y empresaria argentina."),
                W("Moria Casán", "Vedette y figura televisiva argentina."),
                W("Luisana Lopilato", "Actriz argentina que trabajó en Rebelde Way."),
                W("Paulo Dybala", "Futbolista argentino apodado La Joya."),
                W("Ángel Di María", "Futbolista argentino clave en finales.")
            ),

            CreateCategory("Famosos globales",
                W("Taylor Swift", "Cantante estadounidense con muchas eras musicales."),
                W("Cristiano Ronaldo", "Futbolista portugués famoso por el SIUUU."),
                W("Dwayne Johnson", "Actor conocido como La Roca."),
                W("Beyoncé", "Cantante global ex Destiny's Child."),
                W("Shakira", "Cantante colombiana famosa por sus caderas."),
                W("Bad Bunny", "Artista puertorriqueño de música urbana."),
                W("Ariana Grande", "Cantante pop con voz muy aguda."),
                W("Leonardo DiCaprio", "Actor de Titanic y El Renacido."),
                W("Tom Holland", "Actor que interpretó a Spider-Man."),
                W("Zendaya", "Actriz de Euphoria y Spider-Man."),
                W("Kim Kardashian", "Figura mediática y empresaria estadounidense."),
                W("Elon Musk", "Empresario relacionado con Tesla y SpaceX."),
                W("Billie Eilish", "Cantante pop alternativa de voz suave."),
                W("Rihanna", "Cantante y empresaria de belleza."),
                W("Justin Bieber", "Cantante canadiense que empezó muy joven."),
                W("Drake", "Rapero canadiense muy popular."),
                W("Lady Gaga", "Cantante pop famosa por looks extravagantes."),
                W("Brad Pitt", "Actor de Hollywood muy conocido."),
                W("Angelina Jolie", "Actriz de Hollywood y Lara Croft."),
                W("Ed Sheeran", "Cantante pelirrojo con guitarra.")
            ),

            CreateCategory("Países",
                W("Argentina", "País del mate, el tango y el asado."),
                W("Brasil", "País famoso por el carnaval y el fútbol."),
                W("Chile", "País largo y angosto de Sudamérica."),
                W("Uruguay", "País vecino de Argentina, famoso por el mate."),
                W("Paraguay", "País sudamericano con guaraní como idioma oficial."),
                W("Bolivia", "País andino con el Salar de Uyuni."),
                W("Perú", "País donde está Machu Picchu."),
                W("Colombia", "País famoso por el café y la cumbia."),
                W("México", "País de tacos, mariachis y Día de Muertos."),
                W("Estados Unidos", "País de Hollywood y Nueva York."),
                W("Canadá", "País famoso por el frío y el maple."),
                W("España", "País europeo donde se habla español."),
                W("Francia", "País de la Torre Eiffel."),
                W("Italia", "País de la pasta y la pizza."),
                W("Alemania", "País europeo famoso por autos y cerveza."),
                W("Japón", "País del anime, sushi y tecnología."),
                W("China", "País asiático con la Gran Muralla."),
                W("Australia", "País de canguros y koalas."),
                W("Egipto", "País de pirámides y faraones."),
                W("Sudáfrica", "País africano con tres capitales.")
            ),

            CreateCategory("Comidas",
                W("Pizza", "Comida redonda con queso y salsa."),
                W("Hamburguesa", "Pan con carne y acompañamientos."),
                W("Empanada", "Masa rellena típica de Argentina."),
                W("Milanesa", "Carne empanada y frita o al horno."),
                W("Asado", "Carne cocinada a la parrilla."),
                W("Sushi", "Comida japonesa con arroz y pescado."),
                W("Taco", "Tortilla mexicana rellena."),
                W("Pasta", "Comida italiana con muchas formas."),
                W("Ravioles", "Pasta rellena en forma de cuadraditos."),
                W("Ñoquis", "Pasta blanda hecha con papa o harina."),
                W("Helado", "Postre frío de muchos sabores."),
                W("Torta", "Postre que suele comerse en cumpleaños."),
                W("Pancho", "Salchicha dentro de un pan."),
                W("Lomito", "Sándwich argentino con carne."),
                W("Choripán", "Chorizo dentro de pan."),
                W("Ensalada", "Comida fresca con verduras."),
                W("Pollo frito", "Pollo crocante y aceitoso."),
                W("Papas fritas", "Bastones de papa fritos."),
                W("Arroz", "Grano blanco usado en muchas comidas."),
                W("Lasagna", "Pasta en capas con salsa y relleno.")
            ),

            CreateCategory("Objetos",
                W("Celular", "Objeto que usás para llamar y chatear."),
                W("Computadora", "Objeto para trabajar, jugar o programar."),
                W("Teclado", "Tiene muchas teclas."),
                W("Mouse", "Se usa para mover el cursor."),
                W("Monitor", "Pantalla de la computadora."),
                W("Mesa", "Mueble con superficie plana."),
                W("Silla", "Objeto para sentarse."),
                W("Botella", "Sirve para guardar líquidos."),
                W("Vaso", "Se usa para tomar bebidas."),
                W("Plato", "Se usa para servir comida."),
                W("Cuchara", "Cubierto para sopa o postres."),
                W("Tenedor", "Cubierto con puntas."),
                W("Cuchillo", "Cubierto que corta."),
                W("Mochila", "Se lleva en la espalda."),
                W("Libro", "Objeto con páginas para leer."),
                W("Lápiz", "Sirve para escribir y se puede borrar."),
                W("Lapicera", "Sirve para escribir con tinta."),
                W("Cuaderno", "Tiene hojas para escribir."),
                W("Auriculares", "Sirven para escuchar audio."),
                W("Control remoto", "Cambia canales o maneja dispositivos."),
                W("Televisor", "Pantalla para ver series o películas."),
                W("Lámpara", "Objeto que da luz."),
                W("Cama", "Mueble para dormir."),
                W("Almohada", "Apoya la cabeza al dormir."),
                W("Reloj", "Sirve para ver la hora."),
                W("Billetera", "Guarda plata y tarjetas."),
                W("Llaves", "Sirven para abrir cerraduras."),
                W("Paraguas", "Protege de la lluvia."),
                W("Zapatilla", "Calzado cómodo/deportivo."),
                W("Campera", "Abrigo para el torso."),
                W("Gorra", "Se usa en la cabeza."),
                W("Espejo", "Refleja tu imagen."),
                W("Cepillo", "Sirve para peinar o limpiar."),
                W("Toalla", "Sirve para secarse."),
                W("Jabón", "Sirve para lavarse."),
                W("Shampoo", "Sirve para lavar el pelo."),
                W("Ventilador", "Mueve aire."),
                W("Heladera", "Mantiene comida fría."),
                W("Microondas", "Calienta comida rápido."),
                W("Horno", "Cocina con calor."),
                W("Sartén", "Sirve para freír o cocinar."),
                W("Olla", "Recipiente para cocinar."),
                W("Tijera", "Sirve para cortar papel u otros materiales."),
                W("Cinta", "Sirve para pegar cosas."),
                W("Caja", "Sirve para guardar objetos."),
                W("Bolsa", "Sirve para transportar cosas."),
                W("Pelota", "Objeto redondo para jugar."),
                W("Bicicleta", "Transporte de dos ruedas."),
                W("Cargador", "Sirve para cargar batería."),
                W("Pendrive", "Dispositivo pequeño para guardar archivos.")
            ),

            CreateCategory("Videojuegos",
                W("Minecraft", "Juego de bloques y crafteo."),
                W("Fortnite", "Battle royale con construcción."),
                W("Roblox", "Plataforma con juegos creados por usuarios."),
                W("Among Us", "Juego de tripulantes e impostores."),
                W("GTA", "Mundo abierto con autos y crimen."),
                W("The Sims", "Simulador de vida."),
                W("FIFA", "Juego de fútbol."),
                W("Call of Duty", "Shooter militar."),
                W("Valorant", "Shooter táctico con agentes."),
                W("League of Legends", "MOBA de campeones y líneas."),
                W("Counter Strike", "Shooter táctico de terroristas y antiterroristas."),
                W("Fall Guys", "Carreras locas con personajes gelatina."),
                W("Mario Kart", "Carreras con personajes de Nintendo."),
                W("Zelda", "Aventura de Link y la Trifuerza."),
                W("Pokémon", "Capturar criaturas y combatir."),
                W("Resident Evil", "Terror con zombies y corporaciones."),
                W("Silent Hill", "Terror psicológico con niebla."),
                W("Elden Ring", "Mundo abierto difícil de fantasía oscura."),
                W("God of War", "Kratos y mitología."),
                W("The Last of Us", "Supervivencia postapocalíptica con infectados.")
            ),

            CreateCategory("Animales",
                W("Perro", "Animal doméstico que ladra."),
                W("Gato", "Animal doméstico que maúlla."),
                W("León", "Felino conocido como rey de la selva."),
                W("Tigre", "Felino grande con rayas."),
                W("Elefante", "Animal enorme con trompa."),
                W("Jirafa", "Animal de cuello muy largo."),
                W("Mono", "Animal que trepa y es muy expresivo."),
                W("Caballo", "Animal usado para montar."),
                W("Vaca", "Animal que da leche."),
                W("Cerdo", "Animal de granja rosado."),
                W("Gallina", "Ave que pone huevos."),
                W("Pato", "Ave que nada y hace cuac."),
                W("Conejo", "Animal con orejas largas."),
                W("Oso", "Animal grande y peludo."),
                W("Lobo", "Animal parecido a un perro salvaje."),
                W("Zorro", "Animal astuto de cola grande."),
                W("Delfín", "Mamífero marino inteligente."),
                W("Tiburón", "Depredador marino con muchos dientes."),
                W("Pingüino", "Ave que no vuela y vive en zonas frías."),
                W("Cocodrilo", "Reptil grande con mandíbula fuerte.")
            ),

            CreateCategory("Películas",
                W("Titanic", "Película del barco que se hunde."),
                W("Avatar", "Película de seres azules en Pandora."),
                W("Toy Story", "Película de juguetes que cobran vida."),
                W("Shrek", "Película de un ogro verde."),
                W("Frozen", "Película de una reina con poderes de hielo."),
                W("Harry Potter", "Saga de magia en Hogwarts."),
                W("El Señor de los Anillos", "Saga del anillo único."),
                W("Star Wars", "Saga espacial con jedis y sables láser."),
                W("Jurassic Park", "Película de dinosaurios clonados."),
                W("Spider-Man", "Película del héroe arácnido."),
                W("Batman", "Película del héroe de Gotham."),
                W("Avengers", "Película de superhéroes reunidos."),
                W("Iron Man", "Película del héroe con armadura tecnológica."),
                W("Coco", "Película animada sobre familia y música."),
                W("Buscando a Nemo", "Película de un pez perdido."),
                W("Rápidos y Furiosos", "Saga de autos y familia."),
                W("Matrix", "Película de realidad simulada."),
                W("El Rey León", "Película de Simba."),
                W("Intensamente", "Película sobre emociones dentro de una mente."),
                W("Barbie", "Película basada en la muñeca famosa.")
            ),

            CreateCategory("Marcas",
                W("Nike", "Marca deportiva del logo swoosh."),
                W("Adidas", "Marca deportiva de tres tiras."),
                W("Apple", "Marca del iPhone."),
                W("Samsung", "Marca coreana de celulares y TVs."),
                W("Coca-Cola", "Gaseosa famosa de etiqueta roja."),
                W("Pepsi", "Gaseosa rival de Coca-Cola."),
                W("McDonald's", "Cadena de hamburguesas del payaso."),
                W("Burger King", "Cadena de hamburguesas del rey."),
                W("Netflix", "Plataforma de series y películas."),
                W("Disney", "Marca de Mickey Mouse."),
                W("Google", "Buscador más famoso de internet."),
                W("Microsoft", "Empresa de Windows y Xbox."),
                W("Amazon", "Tienda online y servicios cloud."),
                W("PlayStation", "Consola de Sony."),
                W("Xbox", "Consola de Microsoft."),
                W("Nintendo", "Empresa de Mario y Zelda."),
                W("Puma", "Marca deportiva con animal en el logo."),
                W("Gucci", "Marca italiana de lujo."),
                W("Zara", "Marca de ropa internacional."),
                W("Mercado Libre", "Marketplace muy usado en Latinoamérica.")
            ),

            CreateCategory("Profesiones",
                W("Médico", "Persona que atiende pacientes."),
                W("Abogado", "Profesional de leyes."),
                W("Profesor", "Persona que enseña."),
                W("Programador", "Persona que escribe código."),
                W("Diseñador", "Persona que crea piezas visuales."),
                W("Arquitecto", "Diseña edificios y espacios."),
                W("Ingeniero", "Resuelve problemas técnicos."),
                W("Cocinero", "Persona que prepara comida."),
                W("Policía", "Trabaja en seguridad pública."),
                W("Bombero", "Apaga incendios y rescata personas."),
                W("Veterinario", "Médico de animales."),
                W("Psicólogo", "Trabaja con la salud mental."),
                W("Contador", "Maneja números, balances e impuestos."),
                W("Periodista", "Informa noticias."),
                W("Actor", "Interpreta personajes."),
                W("Cantante", "Usa la voz como instrumento."),
                W("Electricista", "Trabaja con instalaciones eléctricas."),
                W("Plomero", "Arregla caños y pérdidas de agua."),
                W("Carpintero", "Trabaja con madera."),
                W("Peluquero", "Corta y arregla el pelo.")
            ),

            CreateCategory("Superhéroes",
                W("Spider-Man", "Héroe que lanza telarañas."),
                W("Batman", "Héroe de Gotham sin poderes."),
                W("Superman", "Héroe kryptoniano que vuela."),
                W("Wonder Woman", "Heroína amazona con lazo mágico."),
                W("Iron Man", "Héroe con armadura tecnológica."),
                W("Captain America", "Héroe con escudo."),
                W("Thor", "Dios del trueno con martillo."),
                W("Hulk", "Gigante verde muy fuerte."),
                W("Black Widow", "Espía de Marvel."),
                W("Doctor Strange", "Hechicero supremo."),
                W("Black Panther", "Rey de Wakanda."),
                W("Flash", "Héroe extremadamente rápido."),
                W("Aquaman", "Héroe relacionado con el océano."),
                W("Wolverine", "Mutante con garras."),
                W("Deadpool", "Antihéroe que rompe la cuarta pared."),
                W("Ant-Man", "Héroe que cambia de tamaño."),
                W("Captain Marvel", "Heroína cósmica de Marvel."),
                W("Green Lantern", "Héroe con anillo de poder."),
                W("Daredevil", "Héroe ciego con sentidos aumentados."),
                W("Venom", "Simbionte oscuro relacionado con Spider-Man.")
            )
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

    private WordData W(string word, string hint)
    {
        return new WordData
        {
            Word = word,
            Hint = hint
        };
    }
}
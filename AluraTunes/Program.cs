using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AluraTunes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //tipos de consulta
            //listar os generos rock
            var generos = new List<Genero> {
                new Genero() {Id=1,Nome="Rock" } ,
                new Genero() {Id=2,Nome="Reggae" } ,
                new Genero() {Id=3,Nome="Rock Progressivo" } ,
                new Genero() {Id=4,Nome="Punk Rock" } ,
                new Genero() {Id=5,Nome="Clássica" }
            };

            foreach (var genero in generos)
            {
                if (genero.Nome.Contains("Rock"))
                    Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
            }

            Console.WriteLine();

            //select * from generos
            //var query = from g in generos select g;
            var query = from g in generos where g.Nome.Contains("Rock") select g;

            foreach (var genero in query)
            {
                Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
            }

            // Linq = Language integrated query = consulta integrada a linguagem 

            //__________________________________________________________
            //__________________________________________________________
            // Lista de Musicas
            //LINQ JOINS
            var musicas = new List<Musica>()
             {
                 new Musica { Id = 1, Nome = "Sweet Child O'Mine", GeneroId = 1 },
                 new Musica { Id = 2, Nome = "I Shoot The Sheriff", GeneroId = 2},
                 new Musica { Id = 3, Nome = "Danúbio Azul", GeneroId = 5},
             };

            Console.WriteLine();
            //var musicaQuery = from m in musicas
            //                  select m;

            //criar um objeto anonimo
            var musicaQuery = from m in musicas
                              join g in generos on m.GeneroId equals g.Id
                              select new { m, g };

            //Elabore uma consulta Linq para trazer todas as músicas cujo GeneroId seja igual a 1.
            var musicaQuery2 = from m in musicas
                               join g in generos on m.GeneroId equals g.Id
                               where g.Id == 1
                               select new { m, g };

            foreach (var musica in musicaQuery)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", musica.m.Id, musica.m.Nome, musica.m.GeneroId, musica.g.Nome);
            }

            Console.WriteLine();
        
            //só vai fechar console se o usuario clicar em algo
            Console.ReadKey();
        }

        class Genero
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }

        class Musica
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public int GeneroId { get; set; }
        }

    }
}

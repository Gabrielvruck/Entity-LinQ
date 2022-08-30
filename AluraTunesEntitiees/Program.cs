using AluraTunesEntitiees.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AluraTunesEntitiees
{
    internal class Program
    {
        static void Main(string[] args)
        {

            using (var contexto = new AluraTunesEntities1())
            {
                //Como saber como é a consulta LINQ que está sendo traduzida para o sql?
                contexto.Database.Log = Console.WriteLine;

                var query = from g in contexto.Generoes
                            select g;


                foreach (var genero in query)
                {
                    Console.WriteLine("{0}\t{1}", genero.GeneroId, genero.Nome);
                }

                Console.WriteLine("usando join ____________");

                var faixaEgenero = from g in contexto.Generoes
                                   join f in contexto.Faixas
                                   on g.GeneroId equals f.GeneroId
                                   select new { f, g };

                faixaEgenero = faixaEgenero.Take(10);
                Console.WriteLine();

                foreach (var item in faixaEgenero)
                {
                    Console.WriteLine("{0}\t{1}", item.f.Nome, item.g.Nome);
                }
                Console.WriteLine();

                //________________________________________________
                //________________________________________________
                //Artista
                //filtrando artista
                var textoBusca = "Led";
                var queryArtista = from a in contexto.Artistas
                                   join alb in contexto.Albums on a.ArtistaId equals alb.ArtistaId
                                   where a.Nome.Contains(textoBusca)
                                   select new
                                   {
                                       NomeArtista = a.Nome,
                                       NomeAlbum = alb.Titulo

                                   };

                foreach (var artista in queryArtista)
                {
                    Console.WriteLine("{0}\t{1}", artista.NomeArtista, artista.NomeAlbum);
                }
                Console.WriteLine();
                //________________________________________________
                //________________________________________________
                //outra maneira de fazer buscar 

                var queryNovoConsulta = contexto.Artistas.Where(a => a.Nome.Contains(textoBusca));

                foreach (var artista in queryNovoConsulta)
                {
                    Console.WriteLine("{0}\t{1}", artista.ArtistaId, artista.Nome);
                }

                Console.WriteLine();
                //________________________________________________
                //________________________________________________
                //outra maneira de fazer buscar sem usar joins
                var querySemJoin = from alb in contexto.Albums
                                   where alb.Artista.Nome.Contains(textoBusca)
                                   select new
                                   {
                                       NomeArtista = alb.Artista.Nome,
                                       NomeAlbum = alb.Titulo
                                   };

                foreach (var album in querySemJoin)
                {
                    Console.WriteLine("{0}\t{1}", album.NomeArtista, album.NomeAlbum);
                }
                Console.WriteLine();

                //________________________________________________
                //________________________________________________
                //melhorando a busca

                GetFaixas(contexto, "Led Zeppelin", "");
                Console.WriteLine();
                GetFaixas(contexto, "Led Zeppelin", "Graffiti");
                Console.WriteLine();


                //________________________________________________
                //________________________________________________
                //Odernando elemento

                var buscaArtista = "";
                var buscaAlbum = "";
                var queryOrdenacao = from f in contexto.Faixas
                                     where f.Album.Artista.Nome.Contains(buscaArtista)
                                     && !(string.IsNullOrEmpty(buscaAlbum) ?
                                          f.Album.Titulo.Contains(buscaAlbum) : true)
                                     orderby f.Album.Titulo, f.Nome
                                     select f;

                foreach (var album in queryOrdenacao)
                {
                    Console.WriteLine("{0}\t{1}", album.Album.Titulo, album.Nome);
                }

                Console.WriteLine();


                //________________________________________________
                //________________________________________________
                //COUNT

                //var queryFaixa = from f in contexto.Faixas
                //            where f.Album.Artista.Nome == "Led Zeppelin"
                //            select f;

                //var quantidade = queryFaixa.Count();
                //Console.WriteLine("Led Zeppelin tem {0} músicas no banco de dados.", quantidade);

                var quantidade = contexto.Faixas
                .Count(f => f.Album.Artista.Nome == "Led Zeppelin");

                Console.WriteLine("O banco de dados tem {0} faixas de música.", quantidade);

                //________________________________________________
                //________________________________________________
                //Segunda maneira de fazer count sem usar from 
                Console.WriteLine();

                var quantidadeFaixas = contexto.Faixas.Count();
                Console.WriteLine("O banco de dados tem {0} faixas de música.", quantidadeFaixas);
                Console.WriteLine();

                //________________________________________________
                //________________________________________________
                //SUM

                var queryMulti = from inf in contexto.ItemNotaFiscals
                                 where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                                 select new { totalDoItem = inf.Quantidade * inf.PrecoUnitario };

                foreach (var inf in queryMulti)
                {
                    Console.WriteLine("{0}", inf.totalDoItem);
                }

                var totalDoArtista = queryMulti.Sum(x => x.totalDoItem);
                Console.WriteLine("Total do artista:R$ {0}", totalDoArtista);
                Console.WriteLine();
                //________________________________________________
                //________________________________________________
                //GROUPBY

                //var queryGroupBy = from inf in contexto.ItemNotaFiscals
                //                   where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                //                   group inf by inf.Faixa.Album into agrupado

                //                   orderby agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                //                   descending

                //                   select new
                //                   {
                //                       TituloDoAlbum = agrupado.Key.Titulo,
                //                       TotalPorAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                //                   };

                //REFATORANDO O CODIGO ACIMA
                var queryGroupBy = from inf in contexto.ItemNotaFiscals
                                   where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                                   group inf by inf.Faixa.Album into agrupado

                                   let vendasPorAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)

                                   orderby vendasPorAlbum
                                   descending

                                   select new
                                   {
                                       TituloDoAlbum = agrupado.Key.Titulo,
                                       TotalPorAlbum = vendasPorAlbum
                                   };

                //var artistaEqtdeAlbuns =
                //                        from alb in contexto.Albums
                //                        group alb by alb.Artista into agrupado
                //                        select new
                //                        {
                //                            Artista = agrupado.Key.Nome,
                //                            QuantidadeAlbuns = agrupado.Count()
                //                        };

                //foreach (var item in artistaEqtdeAlbuns)
                //{
                //    Console.WriteLine("{0}\t{1}", item.Artista, item.QuantidadeAlbuns);
                //}

                foreach (var agrupado in queryGroupBy)
                {

                    Console.WriteLine("{0}\t{1}",
                        agrupado.TituloDoAlbum.PadRight(40),
                       agrupado.TotalPorAlbum
                       );
                }

                //________________________________________________
                //________________________________________________
                //MIN MAX AVG
                Console.WriteLine();

                var maiorVenda = contexto.NotaFiscals.Max(nf => nf.Total);
                Console.WriteLine("A maior venda é de R$ {0}", maiorVenda);
                Console.WriteLine();

                var menorVenda = contexto.NotaFiscals.Min(nf => nf.Total);
                Console.WriteLine("A menor venda é de R$ {0}", menorVenda);
                Console.WriteLine();

                var vendaMedia = contexto.NotaFiscals.Average(nf => nf.Total);
                Console.WriteLine("A venda média é de R$ {0}", vendaMedia);
                Console.WriteLine();

                //Refatoracao do codigo acima

                var vendas = (from nf in contexto.NotaFiscals
                              group nf by 1 into agrupado
                              select new
                              {
                                  maiorVenda = agrupado.Max(nf => nf.Total),
                                  menorVenda = agrupado.Min(nf => nf.Total),
                                  vendaMedia = agrupado.Average(nf => nf.Total)
                              }).Single();

                Console.WriteLine("A maior venda é de R$ {0}", vendas.maiorVenda);
                Console.WriteLine("A menor venda é de R$ {0}", vendas.menorVenda);
                Console.WriteLine("A venda média é de R$ {0}", vendas.vendaMedia);
                Console.WriteLine();

                //________________________________________________
                //________________________________________________
                //EXTENSAO MEDIANA DA VENDA

                var queryVenda = from nf in contexto.NotaFiscals select nf.Total;

                decimal mediana = Mediana(queryVenda);
                Console.WriteLine("Mediana: {0}", mediana);
                Console.WriteLine();

                //________________________________________________
                //________________________________________________
                //nova funcao MEDIANA DA VENDA

                var vendaMediana = contexto.NotaFiscals.Mediana(nf => nf.Total);

                Console.Write("Mediana (com metodo de extesao): R$ {0}", vendaMediana);

            }


            //só vai fechar console se o usuario clicar em algo
            Console.ReadKey();
        }

        private static decimal Mediana(IQueryable<decimal> queryVenda)
        {
            var contagem = queryVenda.Count();
            var queryOrdenada = queryVenda.OrderBy(total => total);
            var elementoCentral_1 = queryOrdenada.Skip(contagem / 2).First();

            var elementoCentral_2 = queryOrdenada.Skip((contagem - 1) / 2).First();

            var mediana = (elementoCentral_1 + elementoCentral_2) / 2;
            return mediana;
        }

      

        private static void GetFaixas(AluraTunesEntities1 contexto, string buscaArtista, string buscaAlbum)
        {
            var query = from f in contexto.Faixas
                        where f.Album.Artista.Nome.Contains(buscaArtista)
                        && (!string.IsNullOrEmpty(buscaAlbum) ? f.Album.Titulo.Contains(buscaAlbum) : true)
                        orderby f.Album.Titulo, f.Nome
                        select f;

            foreach (var faixa in query)
            {
                Console.WriteLine("{0}\t{1}", faixa.Album.Titulo.PadRight(40), faixa.Nome);
            }
        }
    }
    static class LinqExtension
    {
        public static decimal Mediana<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var contagem = source.Count();

            var funcSeletor = selector.Compile();
            var queryOrdenada = source.Select(funcSeletor).OrderBy(total => total);

            var elementoCentral_1 = queryOrdenada.Skip(contagem / 2).First();
            var elementoCentral_2 = queryOrdenada.Skip((contagem - 1) / 2).First();

            var mediana = (elementoCentral_1 + elementoCentral_2) / 2;
            return mediana;
        }
    }
}

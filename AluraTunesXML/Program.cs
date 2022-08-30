using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AluraTunesXML
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ACESSANDO DADOS DO XML
            //biblioteca para xml
            XElement root = XElement.Load(@"Data\AluraTunes.xml");

            //var queryXML = from g in root.Element("Generos").Elements("Genero") select g;

            var queryXML = from g in root.Element("Generos").Elements("Genero")
                           join m in root.Element("Musicas").Elements("Musica")
                               on g.Element("GeneroId").Value equals m.Element("GeneroId").Value
                           select new
                           {
                               MusicaId = m.Element("MusicaId").Value,
                               Musica = m.Element("Nome").Value,
                               Genero = g.Element("Nome").Value
                           };

            foreach (var musicaEgenero in queryXML)
            {
                //Console.WriteLine("{0}\t{1}", genero.Element("GeneroId").Value, genero.Element("Nome").Value);
                Console.WriteLine("{0}\t{1}\t{2}",
                    musicaEgenero.MusicaId,
                    musicaEgenero.Musica.PadRight(20),
                    musicaEgenero.Genero);
            }

            //só vai fechar console se o usuario clicar em algo
            Console.ReadKey();
        }
    }
}

using System;
using System.IO;
using System.Linq;

namespace VideoGameSales
{
    class Program
    {
        static string delimiter = new string('-', 40);
        static void Main(string[] args)
        {
            //FileUtil.GenerateVideoGameSalesBinaryFile(FileUtil.CSVFilePath, FileUtil.BinaryFilePath);
            //IndexFiles(FileUtil.BinaryFilePath);
            //ExportWordIndex();

            ShowMenu();
            //Console.WriteLine(limiter);
            //ReadAllInvertedIndex(FileUtil.InvertedIndexFilePath);
        }

        static void ShowMenu()
        {
            int option;

            do
            {
                Console.Clear();
                Console.WriteLine("Selecione uma opção:");
                Console.WriteLine("1) Pesquisar estatísticas de vendas por nome do jogo");
                Console.WriteLine("2) Gerar arquivo de índice");
                Console.WriteLine("3) Importar dados");
                Console.WriteLine("4) Exportar dados");
                Console.WriteLine("0) Sair");

                bool parseSuccess = int.TryParse(Console.ReadLine(), out option);

                if (!parseSuccess || option < 0 || option > 4)
                {
                    option = -1;
                    Console.WriteLine("Opção invalida, tente novamente.");
                    Console.ReadKey();
                }
                else
                {
                    switch (option)
                    {
                        case 1: ShowSearch(); break;
                        default:
                            break;
                    }
                }

            } while (option != 0);
        }

        static void ShowSearch()
        {
            Console.Write("Digite o nome de um jogo: ");

            string name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var terms = StringUtil.GetGameNameWords(name);

                Console.WriteLine("\nRealizando a busca pelos termos relevantes [{0}]", string.Join(", ", terms));

                var results = RecordsSearch.SearchSalesRecordsByName(name);

                Console.WriteLine();

                if (!results.Any())
                {
                    Console.WriteLine("Nenhum resultado encontrado para os termos pesquisados.");
                }
                else
                {
                    Console.WriteLine("Foram encontrados {0} resultados para a pesquisa:", results.Count);
                }

                Console.WriteLine();

                foreach (var item in results)
                {
                    Console.WriteLine(delimiter);
                    Console.WriteLine("[{0}] {1} - {2} ({3}) - {4} - {5}", item.Rank, item.Name, item.Platform, item.Year, item.Genre, item.Publisher);
                    Console.WriteLine(delimiter);

                    Console.WriteLine("\nQuantidade de vendas:");

                    Console.WriteLine("\tAmérica do Norte:\t{0}M", item.NASales);
                    Console.WriteLine("\tEuropa: {0}M", item.EUSales);
                    Console.WriteLine("\tJapão: {0}M", item.JPSales);
                    Console.WriteLine("\tOutros: {0}M", item.OtherSales);
                    Console.WriteLine("\tMundial: {0}M", item.EUSales);

                    Console.WriteLine();
                }

                Console.WriteLine();

                Console.WriteLine("Fim dos resultados da pesquisa.");
            }
            else
            {
                Console.WriteLine("Não foi possível identificar os termos da pesquisa.");
            }

            Console.WriteLine();
            Console.WriteLine("Aperte uma tecla para continuar.");
            Console.ReadKey();
        }

        static void ExportWordIndex()
        {
            using (var csv = File.Open("words.csv", FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(csv))
            {
                var file = InvertedIndexRandomAccessFile.Open(FileUtil.InvertedIndexFilePath);
                try
                {

                    InvertedIndex record;

                    while ((record = file.Read()) != null)
                    {
                        writer.WriteLine("{0}|{1}", record.Word, string.Join(", ", record.Records));
                    }

                }
                catch (Exception)
                {
                }
                finally
                {
                    file.Close();
                }
            }

        }

        static void IndexFiles(string path)
        {
            FileUtil.Delete(FileUtil.InvertedIndexFilePath);
            FileUtil.Delete(FileUtil.WordIndexFilePath);

            var file = VideoGameSalesRandomAccessFile.Open(path);

            var start = DateTime.Now;
            int count = 0;

            VideoGameSalesRecord record;
            while ((record = file.Read()) != null)
            {
                count++;
                FileUtil.GenerateInvertedIndex(record.Name, count);
                Console.Write($"\r{count}/{file.TotalRecords} ({ count * 100 / file.TotalRecords }%) indexed");
            }
            Console.WriteLine();

            var end = DateTime.Now;
            var elapsed = end - start;

            Console.WriteLine("Indexed {0} records in {1}.", count, elapsed);

            file.Close();
        }
    }
}

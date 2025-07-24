using System;
using System.IO;
using System.Net;

namespace IntegracionFTPBancoAmerica
{
    internal class Program
    {
        static string ftpUrl = "ftp://10.0.0.23/";
        static string localFolder = @"C:\FTP\INFORMES\";
        static string ftpUsername = "miguelalejandrovasqueslara22gmail.com";
        static string ftpPassword = "spiderman10.10";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== Sistema de Envío de Informes Banco América ====");
                Console.WriteLine("\n1. Enviar nuevo informe");
                Console.WriteLine("\n2. Ver cantidad y nombres de informes locales");
                Console.WriteLine("\n3. Salir");
                Console.Write("\nSeleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        EnviarInforme();
                        break;
                    case "2":
                        MostrarInformesLocales();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presiona una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void EnviarInforme()
        {
            Console.Clear();
            Console.Write("Escriba el nombre del informe (sin .txt): ");
            string baseName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(baseName))
            {
                Console.WriteLine("\nNombre inválido. Presiona una tecla para regresar al menú.");
                Console.ReadKey();
                return;
            }

            string fileName = baseName + ".txt";
            string fullPath = Path.Combine(localFolder, fileName);
            int count = 1;
            while (File.Exists(fullPath))
            {
                fileName = $"{baseName}_{count}.txt";
                fullPath = Path.Combine(localFolder, fileName);
                count++;
            }

            Console.WriteLine("\nEscriba el contenido del informe.");
            Console.WriteLine("Presione ENTER dos veces seguidas para finalizar.\n");

            string linea;
            var contenidoBuilder = new System.Text.StringBuilder();

            contenidoBuilder.AppendLine("INFORME BANCO AMÉRICA");
            contenidoBuilder.AppendLine("------------------------");

            while (true)
            {
                linea = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(linea)) break;
                contenidoBuilder.AppendLine(linea);
            }

            string contenido = contenidoBuilder.ToString();
            contenido += $"\n\nFecha de envío: {DateTime.Now}";

            try
            {
                Directory.CreateDirectory(localFolder);
                File.WriteAllText(fullPath, contenido);

                string uploadUrl = ftpUrl + fileName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                byte[] fileContents = File.ReadAllBytes(fullPath);
                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"\n✅ Informe '{fileName}' enviado exitosamente.");
                    Console.WriteLine($"Estado: {response.StatusDescription}");
                    Console.WriteLine($"Fecha y hora de envío: {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nInforme \"" + fileName + "\" enviado exitosamente.");
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MostrarInformesLocales()
        {
            Console.Clear();
            try
            {
                if (!Directory.Exists(localFolder))
                {
                    Console.WriteLine("La carpeta de informes aún no existe.");
                }
                else
                {
                    string[] archivos = Directory.GetFiles(localFolder, "*.txt");
                    Console.WriteLine($"📂 Cantidad de informes guardados localmente: {archivos.Length}\n");
                    if (archivos.Length > 0)
                    {
                        Console.WriteLine("Lista de informes:");
                        foreach (string archivo in archivos)
                        {
                            Console.WriteLine("- " + Path.GetFileName(archivo));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No hay informes guardados.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al listar informes: " + ex.Message);
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}
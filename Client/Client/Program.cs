using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Client.DTOs;

namespace Client
{
    class Program
    {
        static readonly HttpClient client = new HttpClient(); 
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("\n\nWelcome to the demo of data exfiltration through FTP protocol");
            Console.WriteLine("Vitit the project repository at: https://github.com/anvpham/data-exfiltration-ftp\n\n");

            bool exit = false;
            while (exit == false)
            {
                Console.WriteLine("Enter the following numbers to perform an action");
                Console.WriteLine("1.List Directory\t2.Download a file\t3.Exit\n");

                Console.Write("Enter a number:");
                string userInput = Console.ReadLine();
                
                switch (userInput)
                {
                    case "1":
                    {
                        Console.WriteLine("\n\nEnter a directory path or nothing to list current directory\n");
                        Console.Write("Enter a directory path:");
                        string directoryPath = Console.ReadLine();
                        bool isSuccessful = await ListDirectoryAsync(directoryPath);

                        if (!isSuccessful)
                            Console.WriteLine("The directory path doesn't exist\n\n");
                        break;
                    }
                        
                    case "2":
                    {
                        Console.Write("\n\nEnter a file path to download:");
                        string filePath = Console.ReadLine();

                        bool isSuccessful = await GetFileAsync(filePath);
                        
                        if(isSuccessful)
                            Console.WriteLine($"Download {filePath} successfully!\n\n");
                        break;
                    }

                    case "3":
                    {
                        exit = true;
                        break;
                    }
                    
                    default:
                        Console.WriteLine("\n\nThe input number is not valid\n\n");
                        break;
                }
            }
        }

        static async Task<bool> ListDirectoryAsync(string path)
        {
            var jsonString = JsonSerializer.Serialize(new ListDirectory()
            {
                Path = path
            });

            HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:3000/api/listdirectory", httpContent);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;
            
            ListDirectoryResponse responseContent = JsonSerializer.Deserialize<ListDirectoryResponse>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            
            Console.WriteLine("\n\n" + responseContent.Result + "\n\n");
            return true;
        }
        
        static async Task<bool> GetFileAsync(string filePath)
        {
            var jsonString = JsonSerializer.Serialize(new GetFile()
            {
                FilePath = filePath
            });

            HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:3000/api/uploadfile", httpContent);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            return true;
        }
    }
}
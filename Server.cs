using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

class TCPServer
{
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 12345;

        TcpListener listener = new TcpListener(ipAddress, port);
        listener.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключений...");

        Queue<string> productQueue = new Queue<string>();

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Подключен клиент.");

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            try
            {
                while (true)
                {
                    string command = reader.ReadLine();

                    if (string.IsNullOrEmpty(command))
                    {
                        break;
                    }

                    if (command == "камера")
                    {
                        if (productQueue.Count < 5)
                        {
                            writer.WriteLine("Введите 'годный' или 'брак': ");
                            writer.Flush();

                            string productType = reader.ReadLine();

                            if (productType == "годный" || productType == "брак")
                            {
                                string product = "[" + productType[0] + "]";
                                productQueue.Enqueue(product);

                                Console.WriteLine($"Продукт {product} добавлен в очередь.");
                                writer.WriteLine($"Продукт {product} добавлен в очередь.");
                                writer.Flush();
                            }
                            else
                            {
                                writer.WriteLine("Неверный ввод. Допустимые значения: 'годный' или 'брак'.");
                                writer.Flush();
                            }
                        }
                        else
                        {
                            writer.WriteLine("Очередь полна. Нельзя добавить больше продуктов.");
                            writer.Flush();
                        }
                    }
                    else if (command == "толкатель")
                    {
                        if (productQueue.Count > 0)
                        {
                            string removedProduct = productQueue.Dequeue();
                            Console.WriteLine($"Продукт {removedProduct} удален из очереди.");
                            writer.WriteLine($"Продукт {removedProduct} удален из очереди.");
                            writer.Flush();
                        }
                        else
                        {
                            writer.WriteLine("Очередь пуста. Нельзя удалять элементы.");
                            writer.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}

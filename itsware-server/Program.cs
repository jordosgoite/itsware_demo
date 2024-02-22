using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class DeviceSocketService
{
    private TcpListener listener;
    private List<string> deviceIds; 
    public DeviceSocketService(int port, List<string> deviceIds)
    {
        listener = new TcpListener(IPAddress.Any, port);
        this.deviceIds = deviceIds;
    }

    public void Start(int port)
    {
        listener.Start();
        Console.WriteLine("Socket service listening on port {0}...", port);

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");

            HandleClient(client);
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                string? message = reader.ReadLine(); 

                if (message == "Connect")
                {
                    string response = GetDeviceListMessage();
                    writer.WriteLine(response);
                    writer.Flush();
                }
                else
                {
                    Console.WriteLine("Invalid message from client: {0}", message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error handling client: {0}", ex.Message);
        }
        finally
        {
            client.Close();
        }
    }

    private string GetDeviceListMessage()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('<');
        builder.Append(deviceIds.Count.ToString().Length); 
        builder.Append(deviceIds.Count);
        builder.Append(',');
        builder.Append(string.Join(",", deviceIds.ToArray())); 
        builder.Append('>'); 
        return builder.ToString();
    }

    public static void Main(string[] args)
    {
        List<string> deviceIds = new List<string> { "123", "124", "125", "126", "127" };

        DeviceSocketService service = new DeviceSocketService(40001, deviceIds);
        service.Start(40001);
    }
}
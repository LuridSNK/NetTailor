using System.Text.Json;
using NetTailor.Contracts;

public static class ResultWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };
    public static void WriteResult<TRequest, TResponse>(this HttpResult<TResponse> response)
    {
        if (response.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"<{typeof(TRequest)}, {typeof(TResponse)}>\nRESPONSE:");
            if (response.Value is Empty e)
            {
                Console.WriteLine(e.ToString());
            }
            else
            {
                if (response.Value is Stream s)
                {
                    using var reader = new StreamReader(s);
                    Console.WriteLine(reader.ReadToEnd());
                }
                else
                {
                    Console.WriteLine(JsonSerializer.Serialize(response.Value, SerializerOptions));
                }
            }
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR:");
            Console.WriteLine(response.Exception?.ToString());
            Console.ResetColor();
            throw response.Exception;
        }
    }
}
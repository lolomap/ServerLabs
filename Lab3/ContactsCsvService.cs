using CsvHelper;
using CsvHelper.Configuration;
using Lab3.Pages;
using System.Globalization;

namespace Lab3
{
    public interface ICsvSerializable;

    public interface ICsvService<T> where T : ICsvSerializable
    {
        Task SaveAsync(T record);
    }

    public class CsvService<T> : ICsvService<T> where T : ICsvSerializable
    {
        private readonly string _filePath = "contacts.csv";

        public async Task SaveAsync(T record)
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(_filePath)
            };

            using FileStream stream = File.Open(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new(stream);
            using CsvWriter csv = new(writer, config);

            if (config.HasHeaderRecord)
            {
                csv.WriteHeader<T>();
                await csv.NextRecordAsync();
            }

            csv.WriteRecord(record);
            await csv.NextRecordAsync();
        }
    }
}

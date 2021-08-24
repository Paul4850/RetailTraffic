using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailTraffic
{
    class Program
    {
        static void Main(string[] args)
        {

            var startTime = new DateTime(2020, 1, 10, 8, 0, 0);
            var endTime = new DateTime(2020, 1, 10, 22, 0, 0);

            DateTime startDay = new DateTime(2021, 1, 10, 8, 0, 0);
            DateTime endDay = new DateTime(2021, 8, 10, 22, 0, 0);

            int[] stores = new int[] { 1, 2, 3 };

            

            string subPath = "RetailTraffic"; // Your code goes here
            string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), subPath);

            bool exists = System.IO.Directory.Exists(dirPath);

            if (exists)
                System.IO.Directory.Delete(dirPath, true);
            
            System.IO.Directory.CreateDirectory(dirPath);

            stores.ToList().ForEach( s => 
                    WriteToFile(s, startDay, endDay, dirPath)
                );
            
        }

        static void WriteToFile(int storeId, DateTime startDate, DateTime endDate, string filePath)
        {
            int days = (int)(endDate - startDate).TotalDays;
            int registersNumber = 10;
            string fileName = storeId.ToString() + ".txt";
            int workHours = 14;

            var csv = new StringBuilder();

            csv.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                "regId",
                "Shopper ID",
                "Age",
                "Gender",
                "HasMask",
                "TimeAppeared",
                "TimeServiceStarted",
                "TimeServed"));

            Enumerable.Range(0, days).ToList().ForEach(
                day => {
                    Enumerable.Range(0, registersNumber).ToList().ForEach(
                        regId =>
                        {
                            var start = startDate.AddDays(day);
                            var end = startDate.AddDays(day).AddHours(workHours);
                            var line = new CheckoutLine(start, end);

                            line.GenerateQueue();
                            line.ProcessedShoppers.ForEach(
                                s =>
                                    csv.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                                    regId,
                                    s.ID,
                                    s.Age,
                                    s.Gender,
                                    s.HasMask,
                                    s.TimeAppeared,
                                    s.TimeServiceStarted,
                                    s.TimeServed))
                                );
                        });
                });

            File.WriteAllText(filePath + "\\" + fileName, csv.ToString());
        }
    }
}

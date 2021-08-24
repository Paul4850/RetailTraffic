using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailTraffic
{
    //    1) очередь больше 5ти человек не собиралась больше чем на 5 минут
    //2) возрастная \половая структура в зависимости от времени суток
    //3) простой: среднее время простоя кассы(нет людей в очереди)

    //StoreId
    //RegId
    //ShopperId
    //Datetime
    //EventType(appeared, left With sell, left without sell)
    //Age
    //Gender

    //params: Store, register, 
    //unique key: Store, register, shopperID
    //conditions: appearanceTime, timeInQueue,  

    public enum Status
    {
        Waiting,
        InService,
        Left,
        Served
    }
    public class Shopper
    {
        public Guid ID { get; set; }
        public int Age { get; set; }
        public bool HasMask { get; set; }
        public string Gender { get; set; }
        public DateTime TimeAppeared { get; set; }
        public DateTime TimeLeft { get; set; }
        public DateTime TimeServiceStarted { get; set; }
        public DateTime TimeServed { get; set; }
        public Status Status { get; set; }
    }
    public class CheckoutLine
    {
        Queue<Shopper> line;

        public List<Shopper> ProcessedShoppers { get; set; }
            

        DateTime start;
        DateTime end;
        int secondsPerAdd;
        int secondsServiceTime;
        int secondsBeforeNewService;
        int secondsPerAddNight = 600;

        DateTime currentTime;

        Random random = new Random();
        public CheckoutLine(DateTime start, DateTime end, int secondsPerAdd = 300, int secondsServiceTime = 290, int secondsBeforeNewService = 10)
        {
            ProcessedShoppers = new List<Shopper>();
            line = new Queue<Shopper>();
            this.start = start;
            this.end = end;
            this.secondsBeforeNewService = secondsBeforeNewService;
            this.secondsServiceTime = secondsServiceTime;
            this.secondsPerAdd = secondsPerAdd;
            currentTime = start;
        }

        public void GenerateQueue()
        {
            double totalSeconds = (end - start).TotalSeconds;
            int currentSeconds = 0;
            int secondsToAdd = secondsPerAdd;
            int secondsShopperInService = secondsServiceTime;
            int currentSecondsBeforeNewService = 0;
            int currentSecondsShopperInService = 0;
            Shopper shopperInService = null;
            double sigma = 60;
            int minServiceTime = 30;
            int maxServiceTime = 600;
            DateTime nightTime = end.AddHours(-1);

            while (currentSeconds < totalSeconds)
            {

                if (currentSeconds == secondsToAdd)
                {
                    line.Enqueue(new Shopper()
                    {
                        Age = GetAge(),
                        Gender = GetGender(),
                        HasMask = GetBool(),
                        ID = Guid.NewGuid(),
                        Status = Status.Waiting,
                        TimeAppeared = start.AddSeconds(currentSeconds)
                    });
                    secondsToAdd += Math.Min(Math.Max(GenerateNormal(secondsPerAdd, sigma), minServiceTime), maxServiceTime);
                    if (start.AddSeconds(currentSeconds) > nightTime)
                        secondsPerAdd = secondsPerAddNight;
                }
                if (currentSecondsBeforeNewService >= secondsBeforeNewService && shopperInService == null && line.Count > 0)
                {
                    shopperInService = line.Peek();
                    shopperInService.Status = Status.InService;
                    shopperInService.TimeServiceStarted = start.AddSeconds(currentSeconds);
                    currentSecondsBeforeNewService = 0;
                }

                if (currentSecondsShopperInService >= secondsShopperInService)
                {
                    shopperInService.TimeServed = start.AddSeconds(currentSeconds);
                    ProcessedShoppers.Add(shopperInService);
                    shopperInService.Status = Status.Served;
                    line.Dequeue();
                    currentSecondsShopperInService = 0;
                    currentSecondsBeforeNewService = 0;
                    shopperInService = null;
                    secondsShopperInService = Math.Min(Math.Max(GenerateNormal(secondsServiceTime, sigma), minServiceTime), maxServiceTime);
                }

                if (shopperInService == null)
                    currentSecondsBeforeNewService++;
                else
                    currentSecondsShopperInService++;

                //if (currentSeconds == se)
                currentSeconds++;
            }
        }

        private int GenerateNormal(double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return (int)randNormal;
        }

        private string GetGender()
        {
            return random.Next(0, 3) == 0 ? "Male" : "Female";
        }

        private bool GetBool()
        {
            return random.Next(0, 3) == 0 ? true : false;
        }

        int GetAge()
        {
            return random.Next(18, 70);
        }
    }
}

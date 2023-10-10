using Lab2RPBDIS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // получение строки подключения из конфигурации
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<RailwayTrafficContext>();
        var options = optionsBuilder.UseSqlServer(connectionString).Options;

        using (RailwayTrafficContext db = new RailwayTrafficContext(options))
        {
            RenegTrains(db);
            //Выполняем разные методы, содержащие операции выборки и изменения данных
            Console.WriteLine("====== Будет выполнена выборка данных (нажмите любую клавишу) ========");
            Console.ReadKey();
            Print("1. Выборка всех поездов (первые 5)", GetTrains(db).Take(5));
            
            string startSymbals = "a";
            Print($"2. Выборка поездов начинающихся на {startSymbals} (первые 5)", GetTrainsFilteredByNumber(db, startSymbals).Take(5));
            
            Console.WriteLine($"3. Максимальная длина пути у типов поездов");
            Dictionary<string, float> dict = CalculateMaxDistancePerTrainType(db);
            foreach (var kvp in dict)
            {
                Console.WriteLine(kvp.Key + ">>>" + kvp.Value);
            }
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("4. Вывод связанной инфомарции из Trains и TrainTypes");
            PrintTrainTypeNamesForTrains(db);
            Console.WriteLine();
            Console.ReadKey(); 

            int maxNameLen = 8;
            Console.WriteLine($"5. Вывод связанной инфомарции из Trains и TrainTypes c фильтрацией по TyoeName < {maxNameLen}");
            PrintTrainTypeNamesForTrainsFiltered(db,maxNameLen);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("====== Будет выполнена вставка данных (нажмите любую клавишу) ========");
            Console.ReadKey();
            Console.WriteLine("6. добавление записи");
            InsertRandomStopAndDisplay(db);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("7. добавление записи на строне многие");
            InsertRandomTrainAndDisplay(db);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("====== Будет выполнено удаление данных (нажмите любую клавишу) ========");
            Console.ReadKey();
            Console.WriteLine("8. удаление записи таблицы на строне 1");
            DeleteLastStop(db);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("9. удаление записи таблицы на строне многие");
            DeleteLastTrain(db);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("====== Будет выполнено обновление данных (нажмите любую клавишу) ========");
            Console.ReadKey();

            Console.WriteLine("10. обновление записи таблицы на строне многие");
            UpdateFirstTrain(db);
            Console.WriteLine();
            Console.ReadKey();
        }
    }

    public static void RenegTrains(RailwayTrafficContext db)
    {
        // Получаем все существующие StopId
        var stopIds = db.Stops.Select(s => s.StopId).ToList();

        // Получаем все поезда, у которых ArrivalStopId и DepartureStopId равны null
        var trainsWithNullStops = db.Trains.Where(t => t.ArrivalStopId == null && t.DepartureStopId == null).ToList();

        // Генерируем случайные значения для ArrivalStopId и DepartureStopId и обновляем их
        var random = new Random();
        foreach (var train in trainsWithNullStops)
        {
            // Выбираем случайное значение из списка всех StopId
            int randomStopId = stopIds[random.Next(stopIds.Count)];

            // Обновляем ArrivalStopId и DepartureStopId для поезда
            train.ArrivalStopId = randomStopId;
            train.DepartureStopId = randomStopId;
        }

        // Сохраняем изменения в базе данных
        db.SaveChanges();

        Console.WriteLine("Поездам были назначены случайные остановки.");
    }

    static void Print(string sqltext, IEnumerable items)
    {
        Console.WriteLine(sqltext);
        Console.WriteLine("Записи: ");
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
        Console.WriteLine();
        Console.ReadKey();
    }

    static IEnumerable<Train> GetTrains(RailwayTrafficContext context)
    {
        var trains = from train in context.Trains
                     select train;

        return trains;
    }

    static IEnumerable<Train> GetTrainsFilteredByNumber(RailwayTrafficContext context, string startSimbols)
    {
        var trains = from train in context.Trains
                     where train.TrainNumber != null && train.TrainNumber.StartsWith(startSimbols)
                     select train;

        return trains;
    }


    public static Dictionary<string, float> CalculateMaxDistancePerTrainType(RailwayTrafficContext db)
    {
        var maxDistancePerTrainType = from train in db.Trains
                                      where train.DistanceInKm.HasValue && train.TrainType != null && train.TrainType.TypeName != null
                                      group train by train.TrainType.TypeName into groupedTrains
                                      select new
                                      {
                                          TypeName = groupedTrains.Key,
                                          MaxDistance = groupedTrains.Max(train => train.DistanceInKm.Value)
                                      };

        return maxDistancePerTrainType.Take(5).ToDictionary(item => item.TypeName, item => item.MaxDistance);
    }

    public static void PrintTrainTypeNamesForTrains(RailwayTrafficContext db)
    {
        var trainData = (from train in db.Trains
                        join trainType in db.TrainTypes on train.TrainTypeId equals trainType.TrainTypeId
                        select new
                        {
                            TrainNumber = train.TrainNumber,
                            TypeName = trainType.TypeName
                        }).Take(5);

        foreach (var data in trainData)
        {
            Console.WriteLine($"Train TrainNumber: {data.TrainNumber}, Train Type Name: {data.TypeName}");
        }
    }

    public static void PrintTrainTypeNamesForTrainsFiltered(RailwayTrafficContext db, int maxNameLen)
    {
        var trainData = (from train in db.Trains
                         join trainType in db.TrainTypes on train.TrainTypeId equals trainType.TrainTypeId
                         where trainType.TypeName.Length <= maxNameLen
                         select new
                         {
                             TrainNumber = train.TrainNumber,
                             TypeName = trainType.TypeName
                         }).Take(5);

        foreach (var data in trainData)
        {
            Console.WriteLine($"Train TrainNumber: {data.TrainNumber}, Train Type Name: {data.TypeName}");
        }
    }

    public static void InsertRandomStopAndDisplay(RailwayTrafficContext db)
    {
        // случайное имя для Stop
        var random = new Random();
        string randomStopName = "Stop" + random.Next(1, 1000);
        // случайные значения для IsRailwayStation и HasWaitingRoom
        bool randomIsRailwayStation = random.Next(0, 2) == 1;
        bool randomHasWaitingRoom = random.Next(0, 2) == 1;

        var newStop = new Stop
        {
            StopName = randomStopName,
            IsRailwayStation = randomIsRailwayStation,
            HasWaitingRoom = randomHasWaitingRoom
        };
        db.Stops.Add(newStop);
        db.SaveChanges();

        Console.WriteLine("Добавлен тип поезда :");
        Console.WriteLine($"Stop ID: {newStop.StopId}");
        Console.WriteLine($"Stop Name: {newStop.StopName}");
        Console.WriteLine($"Is Railway Station: {newStop.IsRailwayStation}");
        Console.WriteLine($"Has Waiting Room: {newStop.HasWaitingRoom}");
    }

    public static void InsertRandomTrainAndDisplay(RailwayTrafficContext db)
    {
        var random = new Random();
        int randomTrainTypeId;
        int randomArrivalStopId;
        int randomDepartureStopId;

        // случайные значения для полей
        string randomTrainNumber = "C" + random.Next(1, 20);
        float randomDistanceInKm = (float)random.NextDouble() * 1000;
        TimeSpan? randomArrivalTime = TimeSpan.FromHours(random.Next(0, 24));
        TimeSpan? randomDepartureTime = TimeSpan.FromHours(random.Next(0, 24));
        bool randomIsBrandedTrain = random.Next(0, 2) == 1;


        do
        {
            // случайное существующие TrainTypeId
            randomTrainTypeId = random.Next(1, db.TrainTypes.Max(t => t.TrainTypeId) + 1);
        } while (db.TrainTypes.FirstOrDefault(t => t.TrainTypeId == randomTrainTypeId) == null);

        do
        {
            //  случайное существующие ArrivalStopId
            randomArrivalStopId = random.Next(1, db.Stops.Max(t => t.StopId));
        } while (db.Stops.FirstOrDefault(s => s.StopId == randomArrivalStopId) == null);

        do
        {
            //  случайное существующие DepartureStopId
            randomDepartureStopId = random.Next(1, db.Stops.Max(t => t.StopId));
        } while (db.Stops.FirstOrDefault(s => s.StopId == randomDepartureStopId) == null);

        var newTrain = new Train
        {
            TrainNumber = randomTrainNumber,
            TrainTypeId = randomTrainTypeId,
            ArrivalStopId = randomArrivalStopId,
            DepartureStopId = randomDepartureStopId,
            DistanceInKm = randomDistanceInKm,
            ArrivalTime = randomArrivalTime,
            DepartureTime = randomDepartureTime,
            IsBrandedTrain = randomIsBrandedTrain
        };
        db.Trains.Add(newTrain);
        db.SaveChanges();

        Console.WriteLine("Добавлен поезд :");
        Console.WriteLine($"Train ID: {newTrain.TrainId}");
        Console.WriteLine($"Train Number: {newTrain.TrainNumber}");
        Console.WriteLine($"Train Type ID: {newTrain.TrainTypeId}");
        Console.WriteLine($"Arrival Stop ID: {newTrain.ArrivalStopId}");
        Console.WriteLine($"Departure Stop ID: {newTrain.DepartureStopId}");
        Console.WriteLine($"Distance in Km: {newTrain.DistanceInKm}");
        Console.WriteLine($"Arrival Time: {newTrain.ArrivalTime}");
        Console.WriteLine($"Departure Time: {newTrain.DepartureTime}");
        Console.WriteLine($"Is Branded Train: {newTrain.IsBrandedTrain}");
    }

    public static void DeleteLastStop(RailwayTrafficContext db)
    {
        // Находим последнюю запись в таблице Stops
        var lastStop = db.Stops.OrderByDescending(s => s.StopId).FirstOrDefault();

        if (lastStop != null)
        {
            // Находим все поезда, у которых ArrivalStopId или DepartureStopId равны идентификатору последней остановки
            var trainsWithLastStopAsArrival = db.Trains.Where(t => t.ArrivalStopId == lastStop.StopId);
            var trainsWithLastStopAsDeparture = db.Trains.Where(t => t.DepartureStopId == lastStop.StopId);

            // Установка ArrivalStopId и DepartureStopId в null для найденных поездов
            foreach (var train in trainsWithLastStopAsArrival)
            {
                train.ArrivalStopId = null;
            }
            foreach (var train in trainsWithLastStopAsDeparture)
            {
                train.DepartureStopId = null;
            }

            // Удаление последней записи из таблицы Stops
            db.Stops.Remove(lastStop);
            db.SaveChanges();

            Console.WriteLine($"Последняя остановка с ID {lastStop.StopId} была успешно удалена.");
        }
        else
        {
            Console.WriteLine("В таблице Stops нет записей");
        }
    }

    public static void DeleteLastTrain(RailwayTrafficContext db)
    {
        // последний поезд в таблице Trains
        var lastTrain = db.Trains.OrderByDescending(t => t.TrainId).FirstOrDefault();

        if (lastTrain != null)
        {
            // Удаляение последнего поезда из таблицы Trains
            db.Trains.Remove(lastTrain);
            db.SaveChanges();

            Console.WriteLine($"Последний Train с ID {lastTrain.TrainId} был успешно удалён.");
        }
        else
        {
            Console.WriteLine("В таблице поездов нет записей для удаления.");
        }
    }

    public static void UpdateFirstTrain(RailwayTrafficContext db)
    {
        // первый поезд, где дистанция в км находится в диапазоне от 80 до 85
        var firstTrainInRange = db.Trains.FirstOrDefault(t => t.DistanceInKm >= 80 && t.DistanceInKm <= 85);

        if (firstTrainInRange != null)
        {
            
            Console.WriteLine($"Старое значение IsBrandedTrain>>> {firstTrainInRange.IsBrandedTrain} ");
            // Обновляем поле IsBrandedTrain на true
            firstTrainInRange.IsBrandedTrain = true;
            db.SaveChanges();
            Console.WriteLine($"Новое значение IsBrandedTrain>>> {firstTrainInRange.IsBrandedTrain} ");

            Console.WriteLine($"Поезд с ID  {firstTrainInRange.TrainId} обновлён успешно.");
        }
        else
        {
            Console.WriteLine("Поездов с дистанцией от 80 и до 85 км нет.");
        }
    }
}

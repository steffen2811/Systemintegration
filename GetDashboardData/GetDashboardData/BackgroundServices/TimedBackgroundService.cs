using GetDashboardData.Models;
using GetDashboardData.Models.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GetDashboardData.backend
{
    public class TimedBackgroundService : IHostedService, IDisposable
    {
        private static IConfiguration _config;

        private const string location = "Skive";
        private const string ftpUrl = "ftp://inverter.westeurope.cloudapp.azure.com";
        private const int fptRetryLimit = 5;
        private const int noOfForecastHours = 4;
        private const int powerColoum = 37;

        private static WeatherData? weatherData;
        private static InverterData? inverterData;
        private static RoomTempData? roomTempData;
        private Timer? _timer;
        private static bool failedToUpdateWeather;
        private static bool failedToUpdatePowerProduction;
        private static bool failedToUpdateRoomTemp;
        private static bool noPowerProductionLastHourFound;
        private static bool firstRun = true;
        private static int lastHourUpdateFtp;
        private static int lastMinUpdateWeb;
        private static int lastMinUpdateDb;
        private static System.Timers.Timer ftpDataTimer = new System.Timers.Timer(300000);
        private static System.Timers.Timer webDataTimer = new System.Timers.Timer(60000);
        private static System.Timers.Timer dbDataTimer = new System.Timers.Timer(60000);

        public TimedBackgroundService(IConfiguration config)
        {
            _config = config;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            webDataTimer.Elapsed += OnTimedRequestWeatherData;
            webDataTimer.AutoReset = true;
            webDataTimer.Stop();

            ftpDataTimer.Elapsed += OnTimedRequestFtpData;
            ftpDataTimer.AutoReset = true;
            ftpDataTimer.Stop();

            dbDataTimer.Elapsed += OnTimedRequestTempData;
            dbDataTimer.AutoReset = true;
            dbDataTimer.Stop();

            _timer = new Timer(Worker, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }
        private void Worker(object state)
        {
            if (firstRun) // Always run the task in first run
            {
                firstRun = false;
                GetDataInParallel();
            }

            int hour = int.Parse(DateTime.Now.ToString("HH"));
            int min = int.Parse(DateTime.Now.ToString("mm"));
            // Get Ftp data every hour when min = 00
            if (lastHourUpdateFtp != hour)
            {
                RequestFtpData();
            }

            // Update weather every 5 min
            if (((int.Parse(DateTime.Now.ToString("mm")) % 5) == 0) && (lastMinUpdateWeb != min))
            {
                lastMinUpdateWeb = int.Parse(DateTime.Now.ToString("mm"));
                RequestWeatherData();
            }

            // Update weather every 5 min
            if (((int.Parse(DateTime.Now.ToString("mm")) % 15) == 0) && (lastMinUpdateDb != min))
            {
                lastMinUpdateDb = int.Parse(DateTime.Now.ToString("mm"));
                RequestTemperature();
            }
            
            // Try again every 5 minut until the data is updated.
            if (failedToUpdatePowerProduction)
                ftpDataTimer.Start();
            else
                ftpDataTimer.Stop();
            
            // Try again every minut until the data is updated.
            if (failedToUpdateWeather)
                webDataTimer.Start();
            else
                webDataTimer.Stop();

            // Try again every minut until the data is updated.
            if (failedToUpdateRoomTemp)
                dbDataTimer.Start();
            else
                dbDataTimer.Stop();
        }

        private void GetDataInParallel()
        {
            Parallel.Invoke(() => RequestWeatherData(), () => RequestFtpData(), () => RequestTemperature());
        }

        private static void OnTimedRequestWeatherData(object state, System.Timers.ElapsedEventArgs e)
        {
            RequestWeatherData();
        }

        private static void RequestWeatherData()
        {
            var client = new ForecastServiceClient();
            var result = new Forecast();
            bool weatherDataAvaliable = true;

            try
            {
                result = client.GetForecastAsync(location, _config["Weather:ApiKey"]).Result.Body.GetForecastResult;
            }
            catch
            {
                weatherDataAvaliable = false;
                failedToUpdateWeather = true;
            }

            if (weatherDataAvaliable)
            {
                var currentConditions = result.location.currentConditions;
                var forecast = result.location.values;

                bool SunAvaliable = ((currentConditions.sunrise < DateTime.Now) && (currentConditions.sunset > DateTime.Now)) ? true : false;
                List<WeatherForecast> forecastForecast = new List<WeatherForecast>(noOfForecastHours);

                for (int i = 0; i < noOfForecastHours; i++)
                {
                    forecastForecast.Add(new WeatherForecast(forecast[i].datetimeStr, forecast[i].cloudcover, forecast[i].temp));
                }
                failedToUpdateWeather = false;
                weatherData = new WeatherData(result.location.address, currentConditions.cloudcover, currentConditions.temp, SunAvaliable, forecastForecast, DateTime.Now);
                lastMinUpdateWeb = int.Parse(DateTime.Now.ToString("mm"));
            }
        }

        private static void OnTimedRequestFtpData(object state, System.Timers.ElapsedEventArgs e)
        {
            RequestFtpData();
        }

        private static void RequestFtpData()
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
            FtpWebResponse? fptResponse = null;
            bool ftpConnected = true, startFound = false;
            int index = 0, firstProduction = 0, lastProduction = 0, ftpRetries = 0, firstProductionIndex = 0, lastProductionIndex = 0;
            noPowerProductionLastHourFound = false;

            ftpRequest.Credentials = new NetworkCredential(_config["ftp:Username"], _config["ftp:Password"]);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

            // Handle if no file is found for this hour. Retry again to check if the file is uploader later than expected.
            do
            {
                try
                {
                    fptResponse = (FtpWebResponse)ftpRequest.GetResponse();
                }
                catch
                {
                    failedToUpdatePowerProduction = true;
                    ftpConnected = false;
                }

                if (ftpConnected)
                {
                    var stream = fptResponse.GetResponseStream();
                    var reader = new StreamReader(stream);

                    List<string> directories = new List<string>();

                    string line = reader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        directories.Add(line);
                        line = reader.ReadLine();
                    }

                    reader.Close();
                    fptResponse.Close();

                    string date = DateTime.Now.AddYears(-1).ToString("yyMMddHH");

                    var matchingvalues = directories
                    .Where(stringToCheck => stringToCheck.Contains(date));

                    if (matchingvalues.Count() != 0)
                    {
                        // Get the object used to communicate with the server.
                        ftpRequest = (FtpWebRequest)WebRequest.Create(ftpUrl + "/" + matchingvalues.Last());
                        ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                        // This example assumes the FTP site uses anonymous logon.
                        ftpRequest.Credentials = new NetworkCredential(_config["ftp:Username"], _config["ftp:Password"]);

                        fptResponse = (FtpWebResponse)ftpRequest.GetResponse();

                        Stream responseStream = fptResponse.GetResponseStream();
                        reader = new StreamReader(responseStream);
                        var content = reader.ReadToEnd();

                        string[] lines = content.Split(
                            "\n",
                            StringSplitOptions.None
                        );

                        foreach (string csvLine in lines)
                        {
                            if (csvLine.Contains("[wr_ende]"))
                                break; // End found

                            if (firstProductionIndex != 0)
                                lastProductionIndex = index;
                            else
                            {
                                if (startFound)
                                    firstProductionIndex = index;
                                else
                                {
                                    if (csvLine.Contains("INTERVAL"))
                                        startFound = true;
                                }
                            }

                            index++;
                        }

                        string[] parts = lines[firstProductionIndex].Split(";");
                        try
                        {
                            firstProduction = int.Parse(parts[powerColoum]);
                        }
                        catch { failedToUpdatePowerProduction = true; }

                        parts = lines[lastProductionIndex].Split(";");
                        try
                        {
                            lastProduction = int.Parse(parts[powerColoum]);
                        }
                        catch { failedToUpdatePowerProduction = true; }

                        inverterData = new InverterData(true, (lastProduction - firstProduction), DateTime.Now);
                        lastHourUpdateFtp = int.Parse(DateTime.Now.ToString("HH"));
                        failedToUpdatePowerProduction = false;
                    }
                    else
                    {
                        noPowerProductionLastHourFound = true;
                        ftpRetries++;
                        Thread.Sleep(5000);
                    }
                }
            } while (noPowerProductionLastHourFound && (ftpRetries < fptRetryLimit));
        }

        private static void OnTimedRequestTempData(object state, System.Timers.ElapsedEventArgs e)
        {
            RequestTemperature();
        }

        private static void RequestTemperature()
        {
            var context = new indeklimaContext(_config);
            Temperatur? temperatur = context.Temperaturs.OrderByDescending(x => x.Dato).ThenByDescending(x => x.Tidspunkt)
              .FirstOrDefault();

            if (temperatur != null)
            {
                roomTempData = new RoomTempData(temperatur.Grader, temperatur.Dato + temperatur.Tidspunkt);
                failedToUpdateRoomTemp = false;
            }
            else
                failedToUpdateRoomTemp = true;
        }

        public static WeatherData GetDashboardDataFromBackend()
        {
            if (weatherData == null)
                return new WeatherData("", 0, 0,false, null, null);
            else
                return weatherData;
        }

        public static InverterData GetInverterDataFromBackend()
        {
            if (inverterData == null)
                return new InverterData(false, 0, null);
            else
                return inverterData;
        }
        
        public static RoomTempData GetTemperatureDataFromBackend()
        {
            if (roomTempData == null)
                return new RoomTempData(0, null);
            else
                return roomTempData;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
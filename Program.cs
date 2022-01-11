using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Hour_Control{
    class Program{
        #region Variables
        const double HOURS_TO_WORK = 160;
        static double targetHours = HOURS_TO_WORK;
        static double hours = 0;
        static double minutes = 0;
        static double numberOfDaysToExclude = 0;
        static bool includeWeekend = false;
        static bool includeToday = false;
        static string email;
        static string token;
        static bool useToggl = false;
        
        public static double Hours { get => hours; set => hours = value; }
        public static double Minutes { get => minutes; set => minutes = value; }
        #endregion

        static async Task Main(string[] args){
            if(args.Length <= 0){
                ShowHelp();
                return;
            }

            GetArguments(args);

            if(useToggl)
                await TogglApiHandler.GetInfoFromTogglAsync(token, email);

            HoursCalculator();
        }

        static void ShowHelp(){
            Console.WriteLine(
                "Hour control helps you to control better the hours you are going to work by day in the current month\n"+
                "Giving to you the amount of hours you need to work per day to reach your final hours in the month\n\n\n"+
                "Options:\n"+
                "-ho,  --hours            Number of hours you already worked this month   [integer]\n"+
                "-m,  --minutes           Number of minutes you already worked this month [integer]\n"+
                "-th, --target_hours      Number of target hours you need to work this month (this is 160h by default) [integer]\n"+
                "-iw  --include_weekend   Use this option if you work at weekends\n"+
                "-e   --exclude           Number of days to exclude from the month [integer]\n"+
                "-t   --today             Include the current day in the count\n\n"+
                "Getting data from toggl is possible using the option -tg or --toggl [options]\n"+
                "-tg  --toggl             Use toggl to get Data hours and minutes, this will override the hours and minutes\n"+
                "-tk  --token             Your token provide by toggl in the account info\n"+
                "-em  --email             Your email to get the hours from your account\n" 
            );
        }

        static void GetArguments(string[] args){
            for (int i = 0; i < args.Length; i++){
                if(args[i] == "-ho" || args[i] == "--hours")
                    hours = double.Parse(args[i + 1]);

                if(args[i] == "-m" || args[i] == "--minutes")
                    minutes = double.Parse(args[i + 1]);

                if(args[i] == "-th" || args[i] == "--target_hours")
                    targetHours = double.Parse(args[i + 1]);
                
                if(args[i] == "-iw" || args[i] == "--include_weekend")
                    includeWeekend = true;

                if(args[i] == "-e" || args[i] == "--exclude")
                    numberOfDaysToExclude = double.Parse(args[i + 1]);

                if(args[i] == "-t" || args[i] == "--today")
                    includeToday = true;
                
                if(args[i] == "-tg" || args[i] == "--toggl")
                    useToggl = true;

                if(args[i] == "-tk" || args[i] == "--token")
                    token = args[i + 1];
                
                if(args[i] == "-em" || args[i] == "--email")
                    email = args[i + 1];
            }
        }

        static void HoursCalculator(){
            double numberOfDays = DaysToWork();
            double workedMinutes = hours * 60 + minutes;
            double leftMinutes = targetHours * 60 - workedMinutes;
            double minutesPerDay = leftMinutes / numberOfDays;
            double hoursPerDay = minutesPerDay / 60;
            double leftMinutesPercentage = (double)(((decimal)hoursPerDay % 1) * 100);
            double leftMinutesPerDay = (leftMinutesPercentage * 60) / 100;

            Console.WriteLine("You worked until now " + hours.ToString("0") + "h:" + minutes + "m");
            Console.WriteLine("You need to work: " +
             hoursPerDay.ToString("0") + "h:" + Math.Round(leftMinutesPerDay) + "m per day during " + numberOfDays + " days");
        }
        
        static bool IsWeekend(string currentDay){
            return currentDay == "Saturday" || currentDay == "Sunday";
        }

        static double DaysToWork(){
            double numberOfDays = 0;
            
            foreach (var item in GetDates(DateTime.Now.Year, DateTime.Now.Month)){
                if(includeToday && item.Day == DateTime.Now.Day)
                    numberOfDays++;

                if(item.Day > DateTime.Now.Day){
                    if(!includeWeekend && IsWeekend(item.DayOfWeek.ToString()))
                        continue;

                    numberOfDays++;
                }
            }

            return numberOfDays;
        }

        public static List<DateTime> GetDates(int year, int month){
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                             .Select(day => new DateTime(year, month, day))
                             .ToList();
        }
    }
}

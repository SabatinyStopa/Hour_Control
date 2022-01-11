using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System;
using System.Collections.Generic;

namespace Hour_Control{
    public class TogglApiHandler{
        const string FINAL_PART = "T00%3A00%3A00%2B02%3A00";
        public static async Task GetInfoFromTogglAsync(string token, string email){
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, Url(token, email)); 
            var byteArray = Encoding.ASCII.GetBytes(token + ":" + "api_token");
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
            var response = await client.SendAsync(requestMessage);
            SetCurrentWorkedHours(await response.Content.ReadAsStringAsync());
        }

        static void SetCurrentWorkedHours(string allData){
            double hours = (TotalSecondsWorked(allData) / 60) / 60;
            double leftMinutesPercentage = (double)(((decimal)hours % 1) * 100);
            double realMinutes = (leftMinutesPercentage * 60) / 100;

            Program.Hours = (int)hours;
            Program.Minutes = (int)realMinutes;
        }

        static string Url(string token, string email){
            List<DateTime> month = Program.GetDates(DateTime.Now.Year, DateTime.Now.Month);
            
            string parsedMonth = ParsedDate(month[0].Month);
            string currentYear = month[0].Year.ToString();
            string startDate = currentYear + "-" + parsedMonth + "-" + ParsedDate(month[0].Day);
            string endDate = currentYear + "-" + parsedMonth + "-" + ParsedDate(month[month.Count - 1].Day);

            string url = "https://api.track.toggl.com/api/v8/time_entries?user_agent=" + email
                    + "&start_date=" + startDate + FINAL_PART + "&end_date=" + endDate + FINAL_PART;
            return url;
        }

        static double TotalSecondsWorked(string allData){
            char[] separators = new char[] { '{', '}', ':', ',', '"' };
            string[] splittedData = allData.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            double totalSecondsWorked = 0;

            for (int i = 0; i < splittedData.Length; i++){
                if(splittedData[i] == "duration")
                    totalSecondsWorked += double.Parse(splittedData[i + 1]);
            }

            return totalSecondsWorked;
        }

        static string ParsedDate(int date){
            return date > 10 ? date.ToString() : "0" + date.ToString();
        }
    }
}
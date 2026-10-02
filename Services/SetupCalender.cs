using System.Globalization;

namespace HairdresserApp.Services;

/* Class to setup calendar */
public class SetupCalender
{

    //Getting current week
    public int GetWeek()
    {
        //Calendar instance
        CultureInfo seCI = new CultureInfo("sv-SE");
        Calendar seCal = seCI.Calendar;

        //Properties required for getweekofyear method
        CalendarWeekRule seCWR = seCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek seFirstDOW = seCI.DateTimeFormat.FirstDayOfWeek;
        DateTime currentDate = DateTime.Now;

        return seCal.GetWeekOfYear(currentDate, seCWR, seFirstDOW);
    }

    //Getting days of given week
    public List<DateTime> GetWeekdays(DateTime date)
    {
        //Days for calendar
        var currentDay = date.DayOfWeek; //current weekday
        DateTime monday = new DateTime();

        //If currentDay == sat or sun count days til upcoming mon
        if(currentDay.ToString() == "Saturday" || currentDay.ToString() == "Sunday")
        {
            //Days til next monday
            int daysTilNextMonday = 7 - ((int)currentDay + 6) % 7;
            monday = date.AddDays(daysTilNextMonday); // Adding days til next monday
        }
        else
        {
            //Days since last monday
            int daysSinceMonday = ((int)currentDay + 6) % 7; // current day to int and counting remaining days when split by 7 
            monday = date.AddDays(-daysSinceMonday); // Subtracting days to get previous monday
        }

        List<DateTime> daysThisWeek = Enumerable.Range(0,5) //Sequence of integers
            .Select(d => monday.AddDays(d)) //Adding given integer to day
            .ToList(); // Making list of days

        return daysThisWeek;
    }
}
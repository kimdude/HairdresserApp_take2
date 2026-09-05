using System.Globalization;
using HairdresserApp.Data;
using HairdresserApp.Models;
using Microsoft.EntityFrameworkCore;
namespace HairdresserApp.Services;

/* Class to setup calendar */
public class SetupCalender
{

    //Getting current week
    public int GetWeek()
    {
        //Calendar instance
        CultureInfo myCI = new CultureInfo("sv-EU");
        Calendar myCal = myCI.Calendar;

        //Properties required for getweekofyear method
        CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
        DateTime currentDate = DateTime.Now;

        return myCal.GetWeekOfYear(currentDate, myCWR, myFirstDOW);
    }

    //Getting days of given week
    public List<DateTime> GetWeekdays(DateTime date)
    {
        //Days for calendar
        var currentDay = date.DayOfWeek; //current weekday
        DateTime monday = new DateTime();

        //If currentDay == sat or sun count days til upcoming mon
        if(currentDay.ToString() == "Saturday")
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

        var daysThisWeek = Enumerable.Range(0,5) //Sequence of integers
            .Select(d => monday.AddDays(d)) //Adding given integer to day
            .ToList(); // Making list of days

        return daysThisWeek;
    }
}
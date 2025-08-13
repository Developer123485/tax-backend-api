using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaxApp.BAL.Utilities
{
    public class Common
    {
        public static bool IsValidDecimal(decimal value)
        {
            return value % 1 == 0; // Check if the value has no remainder when divided by 1
        }

        public static int GetMonthDistance(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (endDate.Year - startDate.Year) + endDate.Month - startDate.Month;

            // Optional: If you want to count a partial month as a full one
            if (endDate.Day < startDate.Day)
            {
                monthsApart--;
            }
            if (monthsApart > 0)
            {
                return Math.Abs(monthsApart + 1);
            }
            return monthsApart; // Remove this if you want to keep direction
        }

        public static int GetFinancialYearStart(DateTime date)
        {
            string financialYear = GetFinancialYear(date, 4);
            return int.Parse(financialYear.Split('-')[0]);
        }
        public static string GetQuarter(DateTime date)
        {
            int month = date.Month;
            if (month >= 1 && month <= 3)
                return "Q4"; // Q4
            else if (month >= 4 && month <= 6)
                return "Q1"; // Q1
            else if (month >= 7 && month <= 9)
                return "Q2"; // Q2
            else
                return "Q3"; // Q3
        }

        public static int GetQuarterNumber(DateTime date)
        {
            int month = date.Month;
            if (month >= 1 && month <= 3)
                return 4; // Q4
            else if (month >= 4 && month <= 6)
                return 1; // Q1
            else if (month >= 7 && month <= 9)
                return 2; // Q2
            else
                return 3; // Q3
        }

        public static dynamic GetValidDateTime(string dateValue)
        {
            dynamic date = "";
            if (!String.IsNullOrEmpty(dateValue))
            {
                if (int.TryParse(dateValue, out int result))
                {
                    double serialDateOfDeposit = double.Parse(dateValue);
                    date = DateTime.FromOADate(serialDateOfDeposit);
                }
                else
                {
                    DateTime.TryParseExact(dateValue, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result1);
                    return result1;
                }
            }
            return date;
        }

        public static string GetFinancialYear(DateTime date, int financialYearStartMonth)
        {
            int year = date.Year;

            // If the month is before the financial year's start month (e.g., before April), it's the previous year.
            if (date.Month < financialYearStartMonth)
            {
                year--; // Financial year will be last year
            }

            // Format the financial year as "YYYY-YYYY" (e.g., "2024-2025")
            return $"{year}-{(year + 1).ToString().Substring(2)}";
        }

        public static int GetFinancialStartYear(DateTime date, int financialYearStartMonth)
        {
            int year = date.Year;

            // If the month is before the financial year's start month (e.g., before April), it's the previous year.
            if (date.Month < financialYearStartMonth)
            {
                year--; // Financial year will be last year
            }

            // Format the financial year as "YYYY-YYYY" (e.g., "2024-2025")
            return year;
        }

        public static string GetNature(string value)
        {
            var nature = "";
            if (value == "TDS-Non-Salary (26Q)")
                nature = "26Q";
            if (value == "TDS-Salary (24Q)")
                nature = "24Q";
            if (value == "TDS-Non-Resident (27Q)")
                nature = "27Q";
            if (value == "TCS (27EQ)")
                nature = "27EQ";
            return nature;
        }

        public static bool getLastDateOfMonth(DateTime date)
        {
            DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);

            // Check if the given date is the same as the last day of the month
            if (date.Date == lastDayOfMonth.Date)
            {
                return true;
            }
            return false;
        }
        public static bool IsValidPAN(string pan)
        {
            // Define the regex pattern for the PAN format
            string pattern = @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$";

            // Match the input string against the regex pattern
            Regex regex = new Regex(pattern);

            return regex.IsMatch(pan);
        }

        public static int GetStartYearFromFinancialYear(string financialYear)
        {
            if (string.IsNullOrWhiteSpace(financialYear) || !financialYear.Contains("-"))
                throw new ArgumentException("Invalid financial year format");

            string[] parts = financialYear.Split('-');
            if (int.TryParse(parts[0], out int startYear))
            {
                return startYear;
            }

            throw new ArgumentException("Invalid start year in financial year");
        }

        public static bool IsLastDayOfLastQuarter(DateTime givenDate)
        {
            // Get the last quarter's start date
            DateTime startOfLastQuarter = GetLastQuarterStartDate(givenDate);

            // Calculate the end of last quarter (last day of the quarter)
            DateTime endOfLastQuarter = startOfLastQuarter.AddMonths(3).AddDays(-1); // End of quarter is 3 months after start - 1 day

            // Check if the given date is the last day of the last quarter
            return givenDate.Date == endOfLastQuarter.Date;
        }

        public static DateTime GetLastQuarterStartDate(DateTime currentDate)
        {
            int month = currentDate.Month;
            int year = currentDate.Year;

            // Determine the start of the previous quarter
            if (month >= 1 && month <= 3)
            {
                month = 10; // The previous quarter is Q4 of the previous year (October - December)
                year -= 1;  // Adjust the year back by 1
            }
            else if (month >= 4 && month <= 6)
            {
                month = 1;  // Q1 (January - March)
            }
            else if (month >= 7 && month <= 9)
            {
                month = 4;  // Q2 (April - June)
            }
            else
            {
                month = 7;  // Q3 (July - September)
            }

            // Return the start date of the previous quarter
            return new DateTime(year, month, 1);
        }

        public static bool IsDepositWithinQuarter(DateTime depositDate)
        {
            // Get the start date of the current quarter
            DateTime startOfCurrentQuarter = GetQuarterStartDate(DateTime.Now);

            // Calculate the end date of the current quarter (3 months after the start)
            DateTime endOfCurrentQuarter = startOfCurrentQuarter.AddMonths(3).AddDays(-1); // Last day of the quarter

            // Check if the deposit date is within the current quarter
            return depositDate >= startOfCurrentQuarter && depositDate <= endOfCurrentQuarter;
        }

        public static DateTime GetQuarterStartDate(DateTime currentDate)
        {
            int month = currentDate.Month;
            int year = currentDate.Year;

            // Determine the start of the current quarter based on the month
            if (month >= 1 && month <= 3)
            {
                return new DateTime(year, 1, 1); // Q1: January 1st
            }
            else if (month >= 4 && month <= 6)
            {
                return new DateTime(year, 4, 1); // Q2: April 1st
            }
            else if (month >= 7 && month <= 9)
            {
                return new DateTime(year, 7, 1); // Q3: July 1st
            }
            else
            {
                return new DateTime(year, 10, 1); // Q4: October 1st
            }
        }

        public static DateTime GetQuarterStartDateByFY(string quarter, string financialYear)
        {
            // Split the financial year string "2024-25"
            var years = financialYear.Split('-');
            int startYear = int.Parse(years[0]);
            int endYear = (startYear + 1); // FY 2024-25 means year end is 2025

            return quarter.ToUpper() switch
            {
                "Q1" => new DateTime(startYear, 4, 1),
                "Q2" => new DateTime(startYear, 7, 1),
                "Q3" => new DateTime(startYear, 10, 1),
                "Q4" => new DateTime(endYear, 1, 1),
                _ => throw new ArgumentException("Invalid quarter. Must be Q1, Q2, Q3, or Q4.")
            };
        }

        public static DateTime GetQuarterEndDate(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1;         // Determine quarter
            int lastMonthOfQuarter = quarter * 3;           // Last month of quarter
            int lastDay = DateTime.DaysInMonth(date.Year, lastMonthOfQuarter); // Last day of that month
            return new DateTime(date.Year, lastMonthOfQuarter, lastDay); // Return full date
        }
    }
}

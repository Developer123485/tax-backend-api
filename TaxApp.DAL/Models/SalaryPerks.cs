using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class SalaryPerks
    {
        public int Id { get; set; }
        [NotMapped]
        public string? PanOfEmployee { get; set; }
        [NotMapped]
        public string? NameOfEmploye { get; set; }
        public decimal? AccommodationValue { get; set; }
        public decimal? AccommodationAmount { get; set; }
        public decimal? CarsValue { get; set; }
        public decimal? CarsAmount { get; set; }
        public decimal? SweeperValue { get; set; }
        public decimal? SweeperAmount { get; set; }
        public decimal? GasValue { get; set; }
        public decimal? GasAmount { get; set; }
        public decimal? InterestValue { get; set; }
        public decimal? InterestAmount { get; set; }
        public decimal? HolidayValue { get; set; }
        public decimal? HolidayAmount { get; set; }
        public decimal? FreeTravelValue { get; set; }
        public decimal? FreeTravelAmount { get; set; }
        public decimal? FreeMealsValue { get; set; }
        public decimal? FreeMealsAmount { get; set; }
        public decimal? FreeEducationValue { get; set; }
        public decimal? FreeEducationAmount { get; set; }
        public decimal? GiftsValue { get; set; }
        public decimal? GiftsAmount { get; set; }
        public decimal? CreditCardValue { get; set; }
        public decimal? CreditCardAmount { get; set; }
        public decimal? ClubValue { get; set; }
        public decimal? ClubAmount { get; set; }
        public decimal? UseOfMoveableValue { get; set; }
        public decimal? UseOfMoveableAmount { get; set; }
        public decimal? TransferOfAssetValue { get; set; }
        public decimal? TransferOfAssetAmount { get; set; }
        public decimal? ValueOfAnyOtherValue { get; set; }
        public decimal? ValueOfAnyOtherAmount { get; set; }
        public decimal? Stock16IACValue { get; set; }
        public decimal? Stock16IACAmount { get; set; }
        public decimal? StockAboveValue { get; set; }
        public decimal? StockAboveAmount { get; set; }
        public decimal? ContributionValue { get; set; }
        public decimal? ContributionAmount { get; set; }
        public decimal? AnnualValue { get; set; }
        public decimal? AnnualAmount { get; set; }
        public decimal? OtherValue { get; set; }
        public decimal? OtherAmount { get; set; }
        public int DeductorId { get; set; }
        public int? UserId { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual User? Users { get; set; }
    }
}

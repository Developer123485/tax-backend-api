using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Utilities;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Services
{
    public class FormValidationService : IFormValidationService
    {
        public readonly IDeductorService _deductorService;
        public readonly I24QValidationService _24ValidationService;
        public readonly I27QValidationService _27QValidationService;
        public readonly I27EQValidationService _27EQValidationService;
        public readonly I26QValidationService _26QValidationService;
        public ISalaryDetailService _salaryDetailService;
        public FormValidationService(IDeductorService deductorService, I24QValidationService validation24Service,
            I27QValidationService validation27QService, I27EQValidationService validation27EQService, I26QValidationService validation26QService)
        {
            _deductorService = deductorService;
            _24ValidationService = validation24Service;
            _26QValidationService = validation26QService;
            _27QValidationService = validation27QService;
            _27EQValidationService = validation27EQService;
        }

        public async Task<FileValidation> CheckDDOValidations(List<SaveDdoDetailsModel> deductors)
        {
            return null;
        }
        public async Task<FileValidation> CheckDeductorsValidations(List<DeductorSaveModel> deductors)
        {
            FileValidation models = new FileValidation();
            StringBuilder csvContent = new StringBuilder();
            int deductorIndex = 2;
            csvContent.AppendLine($"Invalid Deductors Details. Please correct the following errors:");
            foreach (var item in deductors)
            {
                var errorIndex = deductorIndex++;
                var regexItem = new Regex("^[a-zA-Z]*$");
                if (String.IsNullOrEmpty(item.DeductorName) || string.IsNullOrWhiteSpace(item.DeductorName) || regexItem.IsMatch(item.DeductorName))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Name is Required and only alphabets are allowed");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeductorTan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Tan is Required");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.DeductorTan) && item.DeductorTan.Length != 10)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Mention the 10 Character TAN of the deductor");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeductorPan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Pan is Required");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.DeductorPan) && item.DeductorPan.Length != 10)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Mention the 10 Character PAN of the deductor");
                    models.IsValidation = true;
                }
                if (!Common.IsValidPAN(item.DeductorPan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Invalid Deductor PAN card number");
                    models.IsValidation = true;
                }

                if (String.IsNullOrEmpty(item.DeductorBranch))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Branch is Required");
                    models.IsValidation = true;
                }
                if (!regexItem.IsMatch(item.DeductorBranch))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Alphabets are allowed for Deductor Branch");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeductorType))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Type is Required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.MinistryState) && (item.DeductorType == "S" || item.DeductorType == "E" || item.DeductorType == "H" || item.DeductorType == "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Ministry State is Required For Deductor Type E,S,H,N");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.MinistryState) && (item.DeductorType != "S" && item.DeductorType != "E" && item.DeductorType != "H" && item.DeductorType != "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Ministry State is not Required it is rquired for Deductor Type E,S,H,N");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.PaoCode) && (item.DeductorType == "A" || item.DeductorType == "S" || item.DeductorType == "D" || item.DeductorType == "E" || item.DeductorType == "G" || item.DeductorType == "H" || item.DeductorType == "L" || item.DeductorType == "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Pao Code is Required For Deductor Type A,S,D,E,G,H,L,N");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.PaoCode) && (item.DeductorType != "A" && item.DeductorType != "S" && item.DeductorType != "D" && item.DeductorType != "E" && item.DeductorType != "G" && item.DeductorType != "H" && item.DeductorType != "L" && item.DeductorType != "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Pao Code is Not Required It is only required for Deductor Type A,S,D,E,G,H,L,N");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DdoCode) && (item.DeductorType == "A" || item.DeductorType == "S" || item.DeductorType == "D" || item.DeductorType == "E" || item.DeductorType == "G" || item.DeductorType == "H" || item.DeductorType == "L" || item.DeductorType == "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - DDO Code is Required For Deductor Type A,S,D,E,G,H,L,N");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.DdoCode) && (item.DeductorType != "A" && item.DeductorType != "S" && item.DeductorType != "D" && item.DeductorType != "E" && item.DeductorType != "G" && item.DeductorType != "H" && item.DeductorType != "L" && item.DeductorType != "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Ddo Code is Not Required It is only required for Deductor Type A,S,D,E,G,H,L,N");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.MinistryName) && (item.DeductorType == "A" || item.DeductorType == "D" || item.DeductorType == "G"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Ministry Name is Required For Deductor Type A,D,G");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.MinistryName) && (item.DeductorType != "A" && item.DeductorType != "D" && item.DeductorType != "G" && item.DeductorType != "E" && item.DeductorType != "H" && item.DeductorType != "L" && item.DeductorType != "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Ministry Name is not Required It is only required for Deductor Type A,D,G,E,H,L,N");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.PaoRegistration) && (item.DeductorType != "A" && item.DeductorType != "S" && item.DeductorType != "D" && item.DeductorType != "E" && item.DeductorType != "G" && item.DeductorType != "H" && item.DeductorType != "L" && item.DeductorType != "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Pao Registration is not Required It is only required for Deductor Type A,S,D,E,G,H,L,N");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.DdoRegistration) && (item.DeductorType != "A" && item.DeductorType != "S" && item.DeductorType != "D" && item.DeductorType != "E" && item.DeductorType != "G" && item.DeductorType != "H" && item.DeductorType != "L" && item.DeductorType != "N"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Ddo Registration is not Required It is only required for Deductor Type A,S,D,E,G,H,L,N");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.IdentificationNumber) && (item.DeductorType == "A" || item.DeductorType == "S"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Identification Number is Required For Deductor Type A,S");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.IdentificationNumber) && item.IdentificationNumber.Length != 7)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Identification Number must be 7 digit");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.DeductorGstNo) && item.DeductorGstNo.Length != 15)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Gst No must be 15 digit");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeductorFlatNo))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Flat is Required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeductorState))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor State is Required or Invalid");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeductorPincode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Pin is Required");
                    models.IsValidation = true;
                }

                if (item.DeductorPincode.Length < 6 || item.DeductorPincode.Length > 6)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Mention the 6 Character Pincode of the deductor pin code");
                    models.IsValidation = true;
                }

                if (String.IsNullOrEmpty(item.DeductorEmailId))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Email Id is Required");
                    models.IsValidation = true;
                }

                bool regex = Regex.IsMatch(item.DeductorEmailId, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase); ;
                if (!regex)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Email is not valid");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsibleName))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Name is Required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsibleDesignation))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Designation is Required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsiblePan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Pan Id is Required");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.ResponsiblePan) && item.ResponsiblePan.Length != 10)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Pan number must be 10 digit");
                    models.IsValidation = true;
                }
                if (!Common.IsValidPAN(item.ResponsiblePan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Invalid Responsible PAN card number");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsibleFlatNo))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible FlatNo is Required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsibleState))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible State is Required or Invalid");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsiblePincode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Pincode is Required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.ResponsibleEmailId))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible EmailId is Required");
                    models.IsValidation = true;
                }
                bool regex2 = Regex.IsMatch(item.ResponsibleEmailId, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase); ;

                if (!regex2)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Email is not valid");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.DeductorMobile) && item.DeductorMobile.Length != 10)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Mention the 10 Character Mobile of the ResponsibleMobile");
                    models.IsValidation = true;
                }

                //if (!String.IsNullOrEmpty(GetDeductorByTan(item.DeductorTan)?.DeductorTan))
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - Tan Number already exist");
                //    models.IsValidation = true;
                //}

                if (String.IsNullOrEmpty(item.ResponsibleMobile))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Responsible Mobile is Required");
                    models.IsValidation = true;
                }

                if (!String.IsNullOrEmpty(item.ResponsibleMobile) && item.ResponsibleMobile.Length != 10)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Mention the 10 Character Mobile of the ResponsibleMobile");
                    models.IsValidation = true;
                }
            }
            models.CSVContent = csvContent;
            return models;
        }

        public async Task<FileValidation> CheckChallanAndDeducteeEntryValidations(List<Challan> challans, List<DeducteeEntry> deducteeDetails, List<SalaryDetail> salaryDetails, int catId, FormDashboardFilter model, string userId, bool isValidateReturn = false)
        {
            FileValidation models = new FileValidation();
            if (catId == 2)
            {
                models = await _26QValidationService.Check26QChallanValidation(challans, deducteeDetails, catId, model, userId, models, isValidateReturn);
            }
            if (catId == 4)
            {
                models = await _27QValidationService.Check27QChallanValidation(challans, deducteeDetails, catId, model, userId, models, isValidateReturn);
            }
            if (catId == 3)
            {
                models = await _27EQValidationService.Check27EQChallanValidation(challans, deducteeDetails, catId, model, userId, models, isValidateReturn);
            }
            if (catId == 1)
            {
                models = await _24ValidationService.Check24QChallanValidation(challans, deducteeDetails,salaryDetails, catId, model, userId, models, isValidateReturn);
            }
            return models;
        }
    }
}

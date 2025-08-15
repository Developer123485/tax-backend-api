using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using MySqlX.XDevAPI.Common;
using System.Data;
using System.Globalization;
using TaxAPI.Controllers;
using TaxApp.BAL;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;
using TaxApp.DAL.Models;
using OfficeOpenXml;

using static TaxApp.BAL.Models.EnumModel;
using NPOI.SS.UserModel;
using Sprache;
using TaxApp.BAL.Utilities;

namespace TaxAPI.Helpers
{
    public class UploadFile : IUploadFile
    {
        private IEnumService _enumService;
        public UploadFile(IEnumService enumService)
        {
            _enumService = enumService;
        }

        public async Task<DataTable> GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();

            using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
            {
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();
                foreach (string column in colFields)
                {
                    DataColumn datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }

                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    //Making empty value as null
                    for (int i = 0; i < fieldData.Length; i++)
                    {
                        if (fieldData[i] == "")
                        {
                            fieldData[i] = null;
                        }
                    }
                    csvData.Rows.Add(fieldData);
                }
            }
            return csvData;
        }

        public async Task<List<DeductorSaveModel>> UploadFileData(IFormFile file, string Path)
        {
            var deductors = new List<DeductorSaveModel>();
            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][5].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][5].ToString()))
                    {
                        string deduType = dt.Rows[i][9].ToString();
                        var deductor = new DeductorSaveModel()
                        {
                            ITDLogin = dt.Rows[i][0].ToString(),
                            ITDPassword = dt.Rows[i][1].ToString(),
                            TracesLogin = dt.Rows[i][2].ToString(),
                            TracesPassword = dt.Rows[i][3].ToString(),
                            DeductorCodeNo = dt.Rows[i][4].ToString(),
                            DeductorName = dt.Rows[i][5].ToString(),
                            DeductorTan = dt.Rows[i][6].ToString().Replace(" ", "").ToUpper(),
                            DeductorPan = dt.Rows[i][7].ToString().Replace(" ", "").ToUpper(),
                            DeductorBranch = dt.Rows[i][8].ToString(),
                            DeductorType = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? Helper.GetEnumMemberValueByDescription<DeductorType>(dt.Rows[i][9].ToString()) : null,
                            DeductorFlatNo = dt.Rows[i][10].ToString(),
                            DeductorBuildingName = dt.Rows[i][11].ToString(),
                            DeductorStreet = dt.Rows[i][12].ToString(),
                            DeductorArea = dt.Rows[i][13].ToString(),
                            DeductorDistrict = dt.Rows[i][14].ToString(),
                            DeductorState = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(dt.Rows[i][15].ToString()) : null,
                            DeductorPincode = dt.Rows[i][16].ToString(),
                            DeductorStdcode = dt.Rows[i][17].ToString(),
                            DeductorTelphone = dt.Rows[i][18].ToString(),
                            DeductorEmailId = dt.Rows[i][19].ToString(),
                            STDAlternate = dt.Rows[i][20].ToString(),
                            PhoneAlternate = dt.Rows[i][21].ToString(),
                            EmailAlternate = dt.Rows[i][22].ToString(),
                            ResponsibleName = dt.Rows[i][23].ToString(),
                            ResponsibleDesignation = dt.Rows[i][24].ToString(),
                            ResponsiblePan = dt.Rows[i][25].ToString(),
                            FatherName = dt.Rows[i][26].ToString(),
                            ResponsibleDOB = dt.Rows[i][27].ToString(),
                            DeductorMobile = dt.Rows[i][28].ToString(),
                            ResponsibleFlatNo = dt.Rows[i][29].ToString(),
                            ResponsibleBuildingName = dt.Rows[i][30].ToString(),
                            ResponsibleStreet = dt.Rows[i][31].ToString(),
                            ResponsibleArea = dt.Rows[i][32].ToString(),
                            ResponsibleDistrict = dt.Rows[i][33].ToString(),
                            ResponsibleState = !String.IsNullOrEmpty(dt.Rows[i][34].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(dt.Rows[i][34].ToString()) : null,
                            ResponsiblePincode = dt.Rows[i][35].ToString(),
                            ResponsibleEmailId = dt.Rows[i][36].ToString(),
                            ResponsibleMobile = dt.Rows[i][37].ToString(),
                            ResponsibleStdcode = dt.Rows[i][38].ToString(),
                            ResponsibleTelephone = dt.Rows[i][39].ToString(),
                            ResponsibleAlternateSTD = dt.Rows[i][40].ToString(),
                            ResponsibleAlternatePhone = dt.Rows[i][41].ToString(),
                            ResponsibleAlternateEmail = dt.Rows[i][42].ToString(),
                            GoodsAndServiceTax = dt.Rows[i][43].ToString(),
                            PaoCode = dt.Rows[i][44].ToString(),
                            PaoRegistration = dt.Rows[i][45].ToString(),
                            DdoCode = dt.Rows[i][46].ToString(),
                            DdoRegistration = dt.Rows[i][47].ToString(),
                            MinistryState = !String.IsNullOrEmpty(dt.Rows[i][48].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(dt.Rows[i][48].ToString()) : null,
                            MinistryName = !String.IsNullOrEmpty(dt.Rows[i][49].ToString()) ? Helper.GetEnumMemberValueByDescription<Ministry>(dt.Rows[i][49].ToString()) : null,
                            MinistryNameOther = dt.Rows[i][50].ToString(),
                            IdentificationNumber = dt.Rows[i][51].ToString()
                        };
                        deductors.Add(deductor);
                    }
                }
            }
            return deductors;
        }

        public async Task<List<DeducteeSaveModel>> UploadDeducteeFile(IFormFile file, string Path)
        {
            var deductees = new List<DeducteeSaveModel>();
            try
            {
                if (file.FileName != null)
                {
                    FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTable dt = dataset.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic date1 = "";
                        if (!String.IsNullOrEmpty(dt.Rows[i][21].ToString()))
                        {
                            if (int.TryParse(dt.Rows[i][21].ToString(), out int result))
                            {
                                double serialDateOfDeposit = double.Parse(dt.Rows[i][21].ToString());
                                date1 = DateTime.FromOADate(serialDateOfDeposit);
                            }
                            else
                            {
                                date1 = Convert.ToDateTime(dt.Rows[i][21].ToString());
                            }
                        }
                        string deduType = dt.Rows[i][9].ToString();
                        var deductee = new DeducteeSaveModel()
                        {
                            IdentificationNo = dt.Rows[i][0].ToString(),
                            Status = !String.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? Helper.GetEnumMemberValueByDescription<DeducteeCode27QAnd27EQ>(dt.Rows[i][1].ToString()) : null,
                            SurchargeApplicable = dt.Rows[i][2].ToString(),
                            ResidentialStatus = dt.Rows[i][3].ToString(),
                            Name = dt.Rows[i][4].ToString(),
                            PanNumber = dt.Rows[i][5].ToString().Replace(" ", "").ToUpper(),
                            PanRefNo = dt.Rows[i][6].ToString().Replace(" ", "").ToUpper(),
                            FlatNo = dt.Rows[i][7].ToString(),
                            BuildingName = dt.Rows[i][8].ToString(),
                            RoadStreet = dt.Rows[i][9].ToString(),
                            AreaLocality = dt.Rows[i][10].ToString(),
                            Town = dt.Rows[i][11].ToString(),
                            PostOffice = dt.Rows[i][12].ToString(),
                            Locality = dt.Rows[i][13].ToString(),
                            Pincode = dt.Rows[i][14].ToString(),
                            State = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(dt.Rows[i][15].ToString()) : null,
                            MobileNo = dt.Rows[i][16].ToString(),
                            STDCode = dt.Rows[i][17].ToString(),
                            PhoneNo = dt.Rows[i][18].ToString(),
                            PrinciplePlacesBusiness = dt.Rows[i][19].ToString(),
                            FirmName = dt.Rows[i][20].ToString(),
                            DOB = !String.IsNullOrEmpty(dt.Rows[i][21].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                            Transporter = dt.Rows[i][22].ToString(),
                            Email = dt.Rows[i][23].ToString(),
                            TinNo = dt.Rows[i][24].ToString(),
                            ZipCodeCase = dt.Rows[i][25].ToString(),
                            Country = !String.IsNullOrEmpty(dt.Rows[i][26].ToString()) ? Helper.GetEnumMemberValueByDescription<CountryCode>(dt.Rows[i][26].ToString()) : null,
                        };
                        deductees.Add(deductee);
                    }
                }
                return deductees;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EmployeeSaveModel>> UploadEmployeeFile(IFormFile file, string Path)
        {
            var employees = new List<EmployeeSaveModel>();
            try
            {
                if (file.FileName != null)
                {
                    FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTable dt = dataset.Tables[1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic date1 = "";
                        if (!String.IsNullOrEmpty(dt.Rows[i][12].ToString()))
                        {
                            if (int.TryParse(dt.Rows[i][12].ToString(), out int result))
                            {
                                double serialDateOfDeposit = double.Parse(dt.Rows[i][12].ToString());
                                date1 = DateTime.FromOADate(serialDateOfDeposit);
                            }
                            else
                            {
                                date1 = Convert.ToDateTime(dt.Rows[i][12].ToString());
                            }
                        }
                        var employee = new EmployeeSaveModel()
                        {
                            EmployeeRef = dt.Rows[i][0].ToString(),
                            Name = dt.Rows[i][1].ToString(),
                            FatherName = dt.Rows[i][2].ToString(),
                            PanNumber = dt.Rows[i][3].ToString(),
                            PanRefNo = dt.Rows[i][4].ToString(),
                            FlatNo = dt.Rows[i][5].ToString().Replace(" ", "").ToUpper(),
                            BuildingName = dt.Rows[i][6].ToString(),
                            RoadStreet = dt.Rows[i][7].ToString(),
                            AreaLocality = dt.Rows[i][8].ToString(),
                            Town = dt.Rows[i][9].ToString(),
                            Pincode = dt.Rows[i][10].ToString(),
                            State = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(dt.Rows[i][11].ToString()) : null,
                            DOB = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                            Sex = dt.Rows[i][13].ToString(),
                            Designation = dt.Rows[i][14].ToString(),
                            Email = dt.Rows[i][15].ToString(),
                            MobileNo = dt.Rows[i][16].ToString(),
                            SeniorCitizen = !String.IsNullOrEmpty(dt.Rows[i][17].ToString()) ? Helper.GetEnumMemberValueByDescription<EmployeeCategory>(dt.Rows[i][17].ToString()) : null,
                        };
                        employees.Add(employee);
                    }
                }
                return employees;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<FormTDSRatesSaveModel>> GetTdsRatesFileData(IFormFile file, string Path, int catId)
        {
            var tdsRatesList = new List<FormTDSRatesSaveModel>();
            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[0];
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][1].ToString()))
                    {
                        if (!String.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                        {

                            var tdsRate = new FormTDSRatesSaveModel()
                            {
                                SectionCode = dt.Rows[i][0].ToString() ?? "",
                                Description = dt.Rows[i][1].ToString() ?? "",
                                Nature = dt.Rows[i][2].ToString() ?? "",
                                DeducteeType = !String.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? dt.Rows[i][3].ToString() : "",
                                Pan = !String.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? dt.Rows[i][4].ToString() : "",
                                OptingForRegime = !String.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? dt.Rows[i][5].ToString() == "Yes" ? true : false : null,
                                AmountExceeding = !String.IsNullOrEmpty(dt.Rows[i][6].ToString()) ? Convert.ToDecimal(dt.Rows[i][6].ToString()) : 0,
                                AmountUpto = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? Convert.ToDecimal(dt.Rows[i][7].ToString()) : 0,
                                ApplicableFrom = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? Convert.ToDateTime(dt.Rows[i][8]) : null,
                                ApplicableTo = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? Convert.ToDateTime(dt.Rows[i][9]) : null,
                                ApplicableRate = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? Convert.ToDecimal(dt.Rows[i][10].ToString()) : 0,
                                TDSRate = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                SurchargeRate = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                HealthCessRate = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                Type = catId,
                            };
                            tdsRatesList.Add(tdsRate);
                        }
                    }
                }
            }
            return tdsRatesList;
        }

        public async Task<List<TaxDepositDueDateSaveModal>> GetTaxDepositData(IFormFile file, string Path)
        {
            var lists = new List<TaxDepositDueDateSaveModal>();
            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[0];
                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][1].ToString()))
                    {
                        if (!String.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                        {
                            var model = new TaxDepositDueDateSaveModal()
                            {
                                FormType = dt.Rows[i][0].ToString(),
                                DateOfDeductionFrom = !String.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? Convert.ToDateTime(dt.Rows[i][1].ToString()) : null,
                                DateOfDeductionTo = !String.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? Convert.ToDateTime(dt.Rows[i][2].ToString()) : null,
                                DepositByBookEntry = !String.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? dt.Rows[i][3].ToString() == "Yes" ? true : false : null,
                                DueDate = !String.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? Convert.ToDateTime(dt.Rows[i][4].ToString()) : null,
                                ExtendedDate = !String.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? Convert.ToDateTime(dt.Rows[i][5].ToString()) : null,
                                Notification = dt.Rows[i][6].ToString(),
                                FinancialYear = dt.Rows[i][7].ToString(),
                            };
                            lists.Add(model);
                        }
                    }
                }
            }
            return lists;
        }

        public async Task<List<ReturnFillingDueDatesSaveModel>> GetReturnFillingDueDateData(IFormFile file, string Path)
        {
            var lists = new List<ReturnFillingDueDatesSaveModel>();
            if (file.FileName != null)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[0];
                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][1].ToString()))
                    {
                        if (!String.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                        {
                            var model = new ReturnFillingDueDatesSaveModel()
                            {
                                FormType = dt.Rows[i][0].ToString(),
                                Quarter = dt.Rows[i][1].ToString(),
                                DueDates = !String.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? Convert.ToDateTime(dt.Rows[i][2].ToString()) : null,
                                ExtendedDate = !String.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? Convert.ToDateTime(dt.Rows[i][3].ToString()) : null,
                                Notification = dt.Rows[i][4].ToString(),
                                FinancialYear = dt.Rows[i][5].ToString(),
                            };
                            lists.Add(model);
                        }
                    }
                }
            }
            return lists;
        }

        public async Task<List<Challan>> GetChallanListFromExcel(IFormFile file, string Path, int catId)
        {
            var challans = new List<Challan>();
            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[1];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][1].ToString()))
                    {
                        if (!String.IsNullOrEmpty(dt.Rows[i][9].ToString()))
                        {
                            dynamic date = "";
                            if (!String.IsNullOrEmpty(dt.Rows[i][9].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][9].ToString(), out int result))
                                {
                                    double serialDateOfDeposit = double.Parse(dt.Rows[i][9].ToString());
                                    date = DateTime.FromOADate(serialDateOfDeposit);
                                }
                                else
                                {
                                    date = Convert.ToDateTime(dt.Rows[i][9].ToString());
                                }
                            }
                            var challan = new Challan()
                            {
                                SerialNo = !String.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? Convert.ToInt16(dt.Rows[i][0].ToString()) : 0,
                                TDSAmount = !String.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? Convert.ToDecimal(dt.Rows[i][1].ToString()) : 0,
                                SurchargeAmount = !String.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? Convert.ToDecimal(dt.Rows[i][2].ToString()) : 0,
                                HealthAndEducationCess = !String.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? Convert.ToDecimal(dt.Rows[i][3].ToString()) : 0,
                                InterestAmount = !String.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? Convert.ToDecimal(dt.Rows[i][4].ToString()) : 0,
                                Fee = !String.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? Convert.ToDecimal(dt.Rows[i][5].ToString()) : 0,
                                Others = !String.IsNullOrEmpty(dt.Rows[i][6].ToString()) ? Convert.ToDecimal(dt.Rows[i][6].ToString()) : 0,
                                TotalTaxDeposit = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? Convert.ToDecimal(dt.Rows[i][7].ToString()) : 0,
                                BSRCode = dt.Rows[i][8].ToString(),
                                DateOfDeposit = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? date.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                ChallanVoucherNo = dt.Rows[i][10].ToString(),
                                TDSDepositByBook = dt.Rows[i][11].ToString() == "Yes" ? "Y" : "N",
                                MinorHeadChallan = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Helper.GetEnumMemberValueByDescription<MinorCode26Q>(dt.Rows[i][12].ToString()) : null,
                                CategoryId = catId
                            };

                            challans.Add(challan);
                        }
                    }
                }
            }
            return challans;
        }

        public async Task<List<SaveDdoDetailsModel>> GetDDODetailsListFromExcel(IFormFile file, string Path, string type)
        {
            var ddoDetails = new List<SaveDdoDetailsModel>();
            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                System.Data.DataTable dt = dataset.Tables[1];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][0].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][0].ToString()))
                    {
                        dynamic date = "";
                        var ddoDetail = new SaveDdoDetailsModel()
                        {
                            Tan = dt.Rows[i][0].ToString(),
                            Name = dt.Rows[i][1].ToString(),
                            DdoRegNo = dt.Rows[i][2].ToString(),
                            DdoCode = dt.Rows[i][3].ToString(),
                            Address1 = dt.Rows[i][4].ToString(),
                            Address2 = dt.Rows[i][5].ToString(),
                            Address3 = dt.Rows[i][6].ToString(),
                            Address4 = dt.Rows[i][7].ToString(),
                            City = dt.Rows[i][8].ToString(),
                            State = Helper.GetEnumMemberValueByDescription<State>(dt.Rows[i][9].ToString()),
                            Pincode = dt.Rows[i][10].ToString(),
                            EmailID = dt.Rows[i][11].ToString()
                        };
                        if (type == "2")
                        {
                            ddoDetail.TaxAmount = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0;
                            ddoDetail.TotalTds = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0;
                            ddoDetail.Nature = dt.Rows[i][14].ToString();
                        }
                        ddoDetails.Add(ddoDetail);
                    }
                }
            }
            return ddoDetails;
        }

        public async Task<List<DeducteeEntry>> GetDeducteeEntryByChallanIdFromExcel(IFormFile file, string Path, int catId)
        {
            var DedcuteeEntrys = new List<DeducteeEntry>();
            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[2];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()) || !String.IsNullOrWhiteSpace(dt.Rows[i][1].ToString()))
                    {
                        if (catId == 2)
                        {
                            dynamic date = "";
                            dynamic date1 = "";
                            if (!String.IsNullOrEmpty(dt.Rows[i][8].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][8].ToString(), out int result))
                                {
                                    double serialDateOfDeduction = double.Parse(dt.Rows[i][8].ToString());
                                    date = DateTime.FromOADate(serialDateOfDeduction);
                                }
                                else
                                {
                                    date = Convert.ToDateTime(dt.Rows[i][8].ToString());
                                }
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][9].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][9].ToString(), out int results))
                                {
                                    double serialDateOfPayment = double.Parse(dt.Rows[i][9].ToString());
                                    date1 = DateTime.FromOADate(serialDateOfPayment);
                                }
                                else
                                {
                                    date1 = Convert.ToDateTime(dt.Rows[i][9].ToString());
                                }
                            }
                            var deductEntry = new DeducteeEntry()
                            {
                                SerialNo = Convert.ToInt32(dt.Rows[i][0].ToString()),
                                SectionCode = !String.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? Helper.GetEnumMemberValueByDescription<SectionCode26Q>(dt.Rows[i][1].ToString()) : null,
                                SectionCodeValue = dt.Rows[i][1].ToString(),
                                ReasonValue = dt.Rows[i][17].ToString(),
                                TypeOfRentPayment = dt.Rows[i][2].ToString(),
                                DeducteePanRef = dt.Rows[i][3].ToString(),
                                DeducteeRef = dt.Rows[i][4].ToString(),
                                DeducteeCode = Helper.GetEnumMemberValueByDescription<DeducteeCode26Q>(dt.Rows[i][5].ToString()),
                                PanOfDeductee = dt.Rows[i][6].ToString(),
                                NameOfDeductee = dt.Rows[i][7].ToString(),
                                DateOfPaymentCredit = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? date.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                DateOfDeduction = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                AmountPaidCredited = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? decimal.Parse(dt.Rows[i][10].ToString()) : 0,
                                TDS = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                Surcharge = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                HealthEducationCess = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                TotalTaxDeducted = !String.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? Convert.ToDecimal(dt.Rows[i][14].ToString()) : 0,
                                TotalTaxDeposited = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Convert.ToDecimal(dt.Rows[i][15].ToString()) : 0,
                                RateAtWhichTax = !String.IsNullOrEmpty(dt.Rows[i][16].ToString()) ? Convert.ToDecimal(dt.Rows[i][16].ToString()) : 0,
                                Reasons = Helper.GetEnumMemberValueByDescription<ReasonsCode26Q>(dt.Rows[i][17].ToString()),
                                CertificationNo = dt.Rows[i][18].ToString(),
                                FourNinteenA = !String.IsNullOrEmpty(dt.Rows[i][19].ToString()) ? Convert.ToDecimal(dt.Rows[i][19].ToString()) : null,
                                FourNinteenB = !String.IsNullOrEmpty(dt.Rows[i][20].ToString()) ? Convert.ToDecimal(dt.Rows[i][20].ToString()) : null,
                                FourNinteenC = !String.IsNullOrEmpty(dt.Rows[i][21].ToString()) ? Convert.ToDecimal(dt.Rows[i][21].ToString()) : null,
                                FourNinteenD = !String.IsNullOrEmpty(dt.Rows[i][22].ToString()) ? Convert.ToDecimal(dt.Rows[i][22].ToString()) : null,
                                FourNinteenE = !String.IsNullOrEmpty(dt.Rows[i][23].ToString()) ? Convert.ToDecimal(dt.Rows[i][23].ToString()) : null,
                                FourNinteenF = !String.IsNullOrEmpty(dt.Rows[i][24].ToString()) ? Convert.ToDecimal(dt.Rows[i][24].ToString()) : null,
                            };
                            DedcuteeEntrys.Add(deductEntry);
                        }
                        if (catId == 4)
                        {
                            dynamic date = "";
                            dynamic date1 = "";
                            if (!String.IsNullOrEmpty(dt.Rows[i][7].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][7].ToString(), out int result))
                                {
                                    double serialDateOfDeduction = double.Parse(dt.Rows[i][7].ToString());
                                    date = DateTime.FromOADate(serialDateOfDeduction);
                                }
                                else
                                {
                                    date = Convert.ToDateTime(dt.Rows[i][7].ToString());
                                }
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][8].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][8].ToString(), out int results))
                                {
                                    double serialDateOfPayment = double.Parse(dt.Rows[i][8].ToString());
                                    date1 = DateTime.FromOADate(serialDateOfPayment);
                                }
                                else
                                {
                                    date1 = Convert.ToDateTime(dt.Rows[i][8].ToString());
                                }
                            }
                            var deductEntry = new DeducteeEntry()
                            {
                                SerialNo = !String.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? Convert.ToInt32(dt.Rows[i][0].ToString()) : null,
                                SectionCode = Helper.GetEnumMemberValueByDescription<SectionCode27Q>(dt.Rows[i][1].ToString()),
                                SectionCodeValue = dt.Rows[i][1].ToString(),
                                ReasonValue = dt.Rows[i][17].ToString(),
                                DeducteePanRef = dt.Rows[i][2].ToString(),
                                DeducteeRef = dt.Rows[i][3].ToString(),
                                DeducteeCode = Helper.GetEnumMemberValueByDescription<DeducteeCode27QAnd27EQ>(dt.Rows[i][4].ToString()),
                                PanOfDeductee = dt.Rows[i][5].ToString(),
                                NameOfDeductee = dt.Rows[i][6].ToString(),
                                DateOfPaymentCredit = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? date.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                DateOfDeduction = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                AmountPaidCredited = dt.Rows[i][9].ToString() != null ? decimal.Parse(dt.Rows[i][9].ToString()) : 0,
                                TDS = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? Convert.ToDecimal(dt.Rows[i][10].ToString()) : 0,
                                Surcharge = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                HealthEducationCess = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                TotalTaxDeducted = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                TotalTaxDeposited = !String.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? Convert.ToDecimal(dt.Rows[i][14].ToString()) : 0,
                                RateAtWhichTax = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Convert.ToDecimal(dt.Rows[i][15].ToString()) : 0,
                                OptingForRegime = dt.Rows[i][16].ToString() == "Yes" ? "Y" : "N",
                                Reasons = Helper.GetEnumMemberValueByDescription<ReasonsCode27Q>(dt.Rows[i][17].ToString()),
                                CertificationNo = dt.Rows[i][18].ToString(),
                                GrossingUp = dt.Rows[i][19].ToString(),
                                TDSRateAct = Helper.GetEnumMemberValueByDescription<TDSRateCode>(dt.Rows[i][20].ToString()),
                                RemettanceCode = Helper.GetEnumMemberValueByDescription<NatureCode>(dt.Rows[i][21].ToString()),
                                Acknowledgement = dt.Rows[i][22].ToString(),
                                CountryCode = Helper.GetEnumMemberValueByDescription<CountryCode>(dt.Rows[i][23].ToString()),
                                Email = dt.Rows[i][24].ToString(),
                                ContactNo = dt.Rows[i][25].ToString(),
                                Address = dt.Rows[i][26].ToString(),
                                TaxIdentificationNo = dt.Rows[i][27].ToString(),
                                FourNinteenA = !String.IsNullOrEmpty(dt.Rows[i][28].ToString()) ? Convert.ToDecimal(dt.Rows[i][28].ToString()) : null,
                                FourNinteenB = !String.IsNullOrEmpty(dt.Rows[i][29].ToString()) ? Convert.ToDecimal(dt.Rows[i][29].ToString()) : null,
                                FourNinteenC = !String.IsNullOrEmpty(dt.Rows[i][30].ToString()) ? Convert.ToDecimal(dt.Rows[i][30].ToString()) : null,
                                FourNinteenD = !String.IsNullOrEmpty(dt.Rows[i][31].ToString()) ? Convert.ToDecimal(dt.Rows[i][31].ToString()) : null,
                                FourNinteenE = !String.IsNullOrEmpty(dt.Rows[i][32].ToString()) ? Convert.ToDecimal(dt.Rows[i][32].ToString()) : null,
                                FourNinteenF = !String.IsNullOrEmpty(dt.Rows[i][33].ToString()) ? Convert.ToDecimal(dt.Rows[i][33].ToString()) : null,
                            };
                            DedcuteeEntrys.Add(deductEntry);
                        }
                        if (catId == 3)
                        {
                            dynamic date = "";
                            dynamic date1 = "";
                            if (!String.IsNullOrEmpty(dt.Rows[i][7].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][7].ToString(), out int result))
                                {
                                    double serialDateOfDeduction = double.Parse(dt.Rows[i][7].ToString());
                                    date = DateTime.FromOADate(serialDateOfDeduction);
                                }
                                else
                                {
                                    date = Convert.ToDateTime(dt.Rows[i][7].ToString());
                                }
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][8].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][8].ToString(), out int results))
                                {
                                    double serialDateOfPayment = double.Parse(dt.Rows[i][8].ToString());
                                    date1 = DateTime.FromOADate(serialDateOfPayment);
                                }
                                else
                                {
                                    date1 = Convert.ToDateTime(dt.Rows[i][8].ToString());
                                }
                            }
                            var deductEntry = new DeducteeEntry()
                            {
                                SerialNo = !String.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? Convert.ToInt32(dt.Rows[i][0].ToString()) : null,
                                SectionCode = Helper.GetEnumMemberValueByDescription<SectionCode27EQ>(dt.Rows[i][1].ToString()),
                                SectionCodeValue = dt.Rows[i][1].ToString(),
                                ReasonValue = dt.Rows[i][18].ToString(),
                                DeducteePanRef = dt.Rows[i][2].ToString(),
                                DeducteeRef = dt.Rows[i][3].ToString(),
                                DeducteeCode = Helper.GetEnumMemberValueByDescription<DeducteeCode27QAnd27EQ>(dt.Rows[i][4].ToString()),
                                PanOfDeductee = dt.Rows[i][5].ToString(),
                                NameOfDeductee = dt.Rows[i][6].ToString(),
                                DateOfPaymentCredit = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? date.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                DateOfDeduction = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                TotalValueOfTheTransaction = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? Convert.ToDecimal(dt.Rows[i][9].ToString()) : 0,
                                AmountPaidCredited = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? Convert.ToDecimal(dt.Rows[i][10].ToString()) : 0,
                                TDS = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                Surcharge = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                HealthEducationCess = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                TotalTaxDeducted = !String.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? Convert.ToDecimal(dt.Rows[i][14].ToString()) : 0,
                                TotalTaxDeposited = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Convert.ToDecimal(dt.Rows[i][15].ToString()) : 0,
                                RateAtWhichTax = !String.IsNullOrEmpty(dt.Rows[i][16].ToString()) ? Convert.ToDecimal(dt.Rows[i][16].ToString()) : 0,
                                OptingForRegime = dt.Rows[i][17].ToString() == "Yes" ? "Y" : "N",
                                Reasons = Helper.GetEnumMemberValueByDescription<ReasonsCode27EQ>(dt.Rows[i][18].ToString()),
                                CertificationNo = dt.Rows[i][19].ToString(),
                                NoNResident = dt.Rows[i][20].ToString() == "Yes" ? "Y" : "N",
                                PermanentlyEstablished = dt.Rows[i][21].ToString(),
                                PaymentCovered = !String.IsNullOrEmpty(dt.Rows[i][22].ToString()) ? (dt.Rows[i][22].ToString() == "Yes" ? "Y" : "N"): "",
                                ChallanNumber = dt.Rows[i][23].ToString(),
                                ChallanDate = !String.IsNullOrEmpty(dt.Rows[i][24].ToString()) ? Convert.ToDateTime(dt.Rows[i][24].ToString()).ToString("dd/MM/yyyy").Replace("-", "/") : null,
                            };
                            DedcuteeEntrys.Add(deductEntry);
                        }
                        if (catId == 1)
                        {
                            dynamic date = "";
                            dynamic date1 = "";
                            if (!String.IsNullOrEmpty(dt.Rows[i][6].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][6].ToString(), out int result))
                                {
                                    double serialDateOfDeduction = double.Parse(dt.Rows[i][6].ToString());
                                    date = DateTime.FromOADate(serialDateOfDeduction);
                                }
                                else
                                {
                                    date = Convert.ToDateTime(dt.Rows[i][6].ToString());
                                }
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][7].ToString()))
                            {
                                if (int.TryParse(dt.Rows[i][7].ToString(), out int results))
                                {
                                    double serialDateOfPayment = double.Parse(dt.Rows[i][7].ToString());
                                    date1 = DateTime.FromOADate(serialDateOfPayment);
                                }
                                else
                                {
                                    date1 = Convert.ToDateTime(dt.Rows[i][7].ToString());
                                }
                            }
                            var deductEntry = new DeducteeEntry()
                            {
                                SerialNo = !String.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? Convert.ToInt32(dt.Rows[i][0].ToString()) : null,
                                SectionCode = Helper.GetEnumMemberValueByDescription<SectionCode24Q>(dt.Rows[i][1].ToString()),
                                SectionCodeValue = dt.Rows[i][1].ToString(),
                                ReasonValue = dt.Rows[i][14].ToString(),
                                DeducteePanRef = dt.Rows[i][2].ToString(),
                                DeducteeRef = dt.Rows[i][3].ToString(),
                                PanOfDeductee = dt.Rows[i][4].ToString(),
                                NameOfDeductee = dt.Rows[i][5].ToString(),
                                DateOfPaymentCredit = !String.IsNullOrEmpty(dt.Rows[i][6].ToString()) ? date.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                DateOfDeduction = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                AmountPaidCredited = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? decimal.Parse(dt.Rows[i][8].ToString()) : 0,
                                TDS = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? Convert.ToDecimal(dt.Rows[i][9].ToString()) : 0,
                                Surcharge = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? Convert.ToDecimal(dt.Rows[i][10].ToString()) : 0,
                                HealthEducationCess = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                TotalTaxDeducted = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                TotalTaxDeposited = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                Reasons = Helper.GetEnumMemberValueByDescription<ReasonsCode24Q>(dt.Rows[i][14].ToString()),
                                CertificationNo = dt.Rows[i][15].ToString(),
                            };
                            DedcuteeEntrys.Add(deductEntry);
                        }
                    }
                }
            }
            return DedcuteeEntrys;
        }

        public async Task<DeductorSaveModel> ReadTxtFile(string filePath)
        {
            var deductor = new DeductorSaveModel();
            var cateId = 0;
            deductor.Challans = new List<Challan>();
            deductor.DeducteeEntry = new List<DeducteeEntry>();
            deductor.SalaryDetail = new List<SalaryDetail>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1)) // skip header
            {
                var salaryRecord = new SalaryDetail();
                var originalLine = line.Replace("^", "*").ToString();
                var columns = originalLine.Split('*');
                if (columns != null && columns[1] == "BH")
                {
                    deductor.Form = columns[4].Trim();
                    deductor.FinancialYear = columns[16].Trim().Substring(0, 4) + "-" + columns[16].Trim().Substring(4, 2);
                    deductor.Quarter = columns[17].Trim();
                    deductor.DeductorTan = columns[12].Trim();
                    deductor.DeductorPan = columns[14].Trim();
                    deductor.DeductorName = columns[18].Trim();
                    deductor.DeductorBranch = columns[19].Trim();
                    deductor.DeductorFlatNo = columns[20].Trim();
                    deductor.DeductorBuildingName = columns[21].Trim();
                    deductor.DeductorStreet = columns[22].Trim();
                    deductor.DeductorArea = columns[23].Trim();
                    deductor.DeductorDistrict = columns[24].Trim();
                    deductor.DeductorState = columns[25].Trim();
                    deductor.DeductorPincode = columns[26].Trim();
                    deductor.DeductorEmailId = columns[27].Trim();
                    deductor.DeductorStdcode = columns[28].Trim();
                    deductor.DeductorTelphone = columns[29].Trim();
                    deductor.DeductorType = columns[31].Trim();
                    deductor.ResponsibleName = columns[32].Trim();
                    deductor.ResponsibleDesignation = columns[33].Trim();
                    deductor.ResponsibleFlatNo = columns[34].Trim();
                    deductor.ResponsibleBuildingName = columns[35].Trim();
                    deductor.ResponsibleStreet = columns[36].Trim();
                    deductor.ResponsibleArea = columns[37].Trim();
                    deductor.ResponsibleDistrict = columns[38].Trim();
                    deductor.ResponsibleState = columns[39].Trim();
                    deductor.ResponsiblePincode = columns[40].Trim();
                    deductor.ResponsibleEmailId = columns[41].Trim();
                    deductor.ResponsibleMobile = columns[42].Trim();
                    deductor.ResponsibleStdcode = columns[43].Trim();
                    deductor.ResponsibleTelephone = columns[44].Trim();
                    deductor.MinistryState = columns[53].Trim();
                    deductor.PaoCode = columns[54].Trim();
                    deductor.DdoCode = columns[55].Trim();
                    deductor.MinistryName = columns[56].Trim();
                    deductor.MinistryNameOther = columns[57].Trim();
                    deductor.ResponsiblePan = columns[58].Trim();
                    deductor.PaoRegistration = columns[59].Trim();
                    deductor.DdoRegistration = columns[60].Trim();
                    deductor.STDAlternate = columns[61].Trim();
                    deductor.PhoneAlternate = columns[62].Trim();
                    deductor.EmailAlternate = columns[63].Trim();
                    deductor.ResponsibleAlternateSTD = columns[64].Trim();
                    deductor.ResponsibleAlternatePhone = columns[65].Trim();
                    deductor.ResponsibleAlternateEmail = columns[66].Trim();
                    deductor.IdentificationNumber = columns[67].Trim();
                    deductor.DeductorGstNo = columns[68].Trim();
                }
                if (columns != null && columns[1] == "CD")
                {
                    string formatted = !String.IsNullOrEmpty(columns[17].Trim()) ? (columns[17].Trim().Substring(0, 2) + "/" + columns[17].Trim().Substring(2, 2) + "/" + columns[17].Trim().Substring(4, 4)) : null;
                    var record = new Challan()
                    {
                        SerialNo = Convert.ToInt16(columns[3].Trim()),
                        ChallanVoucherNo = columns[36].Trim() == "N" ? columns[11].Trim() : columns[13].Trim(),
                        BSRCode = columns[15].Trim(),
                        TDSAmount = Convert.ToDecimal(columns[21].Trim()),
                        DateOfDeposit = formatted,
                        SurchargeAmount = Convert.ToDecimal(columns[22].Trim()),
                        HealthAndEducationCess = Convert.ToDecimal(columns[23].Trim()),
                        InterestAmount = Convert.ToDecimal(columns[24].Trim()),
                        Others = Convert.ToDecimal(columns[25].Trim()),
                        TotalTaxDeposit = Convert.ToDecimal(columns[26].Trim()),
                        TDSDepositByBook = columns[36].Trim(),
                        Fee = Convert.ToDecimal(columns[38].Trim()),
                        MinorHeadChallan = columns[39].Trim(),
                    };
                    deductor.Challans.Add(record);
                }
                if (columns != null && columns[1] == "DD" && deductor.Form == "26Q")
                {
                    string formatted1 = !String.IsNullOrEmpty(columns[22].Trim()) ? (columns[22].Trim().Substring(0, 2) + "/" + columns[22].Trim().Substring(2, 2) + "/" + columns[22].Trim().Substring(4, 4)) : null;
                    string formatted2 = !String.IsNullOrEmpty(columns[23].Trim()) ? (columns[23].Trim().Substring(0, 2) + "/" + columns[23].Trim().Substring(2, 2) + "/" + columns[23].Trim().Substring(4, 4)) : null;
                    var record = new DeducteeEntry()
                    {
                        SerialNo = Convert.ToInt16(columns[3].Trim()),
                        DeducteeCode = columns[7].Trim(),
                        PanOfDeductee = columns[9].Trim(),
                        DeducteeRef = columns[11].Trim(),
                        NameOfDeductee = columns[12].Trim(),
                        TDS = !String.IsNullOrEmpty(columns[13].Trim()) ? Convert.ToDecimal(columns[13].Trim()) : 0,
                        Surcharge = !String.IsNullOrEmpty(columns[14].Trim()) ? Convert.ToDecimal(columns[14].Trim()) : 0,
                        HealthEducationCess = !String.IsNullOrEmpty(columns[15].Trim()) ? Convert.ToDecimal(columns[15].Trim()) : 0,
                        TotalTaxDeducted = !String.IsNullOrEmpty(columns[16].Trim()) ? Convert.ToDecimal(columns[16].Trim()) : 0,
                        TotalTaxDeposited = !String.IsNullOrEmpty(columns[18].Trim()) ? Convert.ToDecimal(columns[18].Trim()) : 0,
                        AmountPaidCredited = !String.IsNullOrEmpty(columns[21].Trim()) ? Convert.ToDecimal(columns[21].Trim()) : 0,
                        DateOfPaymentCredit = formatted1,
                        DateOfDeduction = formatted2,
                        RateAtWhichTax = !String.IsNullOrEmpty(columns[25].Trim()) ? Convert.ToDecimal(columns[25].Trim()) : null,
                        Reasons = columns[29].Trim(),
                        SectionCode = columns[32].Trim(),
                        CertificationNo = columns[33].Trim(),
                        FourNinteenA = !String.IsNullOrEmpty(columns[42].Trim()) ? Convert.ToDecimal(columns[42].Trim()) : null,
                        FourNinteenB = !String.IsNullOrEmpty(columns[43].Trim()) ? Convert.ToDecimal(columns[43].Trim()) : null,
                        FourNinteenC = !String.IsNullOrEmpty(columns[44].Trim()) ? Convert.ToDecimal(columns[44].Trim()) : null,
                        FourNinteenD = !String.IsNullOrEmpty(columns[45].Trim()) ? Convert.ToDecimal(columns[45].Trim()) : null,
                        FourNinteenE = !String.IsNullOrEmpty(columns[46].Trim()) ? Convert.ToDecimal(columns[46].Trim()) : null,
                        FourNinteenF = !String.IsNullOrEmpty(columns[47].Trim()) ? Convert.ToDecimal(columns[47].Trim()) : null,
                    };
                    deductor.DeducteeEntry.Add(record);
                }
                if (columns != null && columns[1] == "DD" && deductor.Form == "27Q")
                {
                    string formatted1 = !String.IsNullOrEmpty(columns[22].Trim()) ? (columns[22].Trim().Substring(0, 2) + "/" + columns[22].Trim().Substring(2, 2) + "/" + columns[22].Trim().Substring(4, 4)) : null;
                    string formatted2 = !String.IsNullOrEmpty(columns[23].Trim()) ? (columns[23].Trim().Substring(0, 2) + "/" + columns[23].Trim().Substring(2, 2) + "/" + columns[23].Trim().Substring(4, 4)) : null;
                    var record = new DeducteeEntry()
                    {
                        SerialNo = Convert.ToInt16(columns[3].Trim()),
                        DeducteeCode = columns[7].Trim(),
                        PanOfDeductee = columns[9].Trim(),
                        DeducteeRef = columns[11].Trim(),
                        NameOfDeductee = columns[12].Trim(),
                        TDS = !String.IsNullOrEmpty(columns[13].Trim()) ? Convert.ToDecimal(columns[13].Trim()) : 0,
                        Surcharge = !String.IsNullOrEmpty(columns[14].Trim()) ? Convert.ToDecimal(columns[14].Trim()) : 0,
                        HealthEducationCess = !String.IsNullOrEmpty(columns[15].Trim()) ? Convert.ToDecimal(columns[15].Trim()) : 0,
                        TotalTaxDeducted = !String.IsNullOrEmpty(columns[16].Trim()) ? Convert.ToDecimal(columns[16].Trim()) : 0,
                        TotalTaxDeposited = !String.IsNullOrEmpty(columns[18].Trim()) ? Convert.ToDecimal(columns[18].Trim()) : 0,
                        AmountPaidCredited = !String.IsNullOrEmpty(columns[21].Trim()) ? Convert.ToDecimal(columns[21].Trim()) : 0,
                        DateOfPaymentCredit = formatted1,
                        DateOfDeduction = formatted2,
                        RateAtWhichTax = !String.IsNullOrEmpty(columns[25].Trim()) ? Convert.ToDecimal(columns[25].Trim()) : null,
                        Reasons = columns[29].Trim(),
                        SectionCode = columns[32].Trim(),
                        CertificationNo = columns[33].Trim(),
                        TDSRateAct = columns[34].Trim(),
                        RemettanceCode = columns[35].Trim(),
                        Acknowledgement = columns[36].Trim(),
                        CountryCode = columns[37].Trim(),
                        Email = columns[38].Trim(),
                        ContactNo = columns[39].Trim(),
                        Address = columns[40].Trim(),
                        TaxIdentificationNo = columns[41].Trim(),
                        FourNinteenA = !String.IsNullOrEmpty(columns[42].Trim()) ? Convert.ToDecimal(columns[42].Trim()) : null,
                        FourNinteenB = !String.IsNullOrEmpty(columns[43].Trim()) ? Convert.ToDecimal(columns[43].Trim()) : null,
                        FourNinteenC = !String.IsNullOrEmpty(columns[44].Trim()) ? Convert.ToDecimal(columns[44].Trim()) : null,
                        FourNinteenD = !String.IsNullOrEmpty(columns[45].Trim()) ? Convert.ToDecimal(columns[45].Trim()) : null,
                        FourNinteenE = !String.IsNullOrEmpty(columns[46].Trim()) ? Convert.ToDecimal(columns[46].Trim()) : null,
                        FourNinteenF = !String.IsNullOrEmpty(columns[47].Trim()) ? Convert.ToDecimal(columns[47].Trim()) : null,
                        OptingForRegime = columns[48].Trim(),
                    };
                    deductor.DeducteeEntry.Add(record);
                }
                if (columns != null && columns[1] == "DD" && deductor.Form == "27EQ")
                {
                    string formatted1 = !String.IsNullOrEmpty(columns[22].Trim()) ? (columns[22].Trim().Substring(0, 2) + "/" + columns[22].Trim().Substring(2, 2) + "/" + columns[22].Trim().Substring(4, 4)) : null;
                    string formatted2 = !String.IsNullOrEmpty(columns[23].Trim()) ? (columns[23].Trim().Substring(0, 2) + "/" + columns[23].Trim().Substring(2, 2) + "/" + columns[23].Trim().Substring(4, 4)) : null;
                    string formatted3 = !String.IsNullOrEmpty(columns[35].Trim()) ? (columns[35].Trim().Substring(0, 2) + "/" + columns[35].Trim().Substring(2, 2) + "/" + columns[35].Trim().Substring(4, 4)) : null;
                    var record = new DeducteeEntry()
                    {
                        SerialNo = Convert.ToInt16(columns[3].Trim()),
                        DeducteeCode = columns[7].Trim(),
                        PanOfDeductee = columns[9].Trim(),
                        DeducteeRef = columns[11].Trim(),
                        NameOfDeductee = columns[12].Trim(),
                        TDS = !String.IsNullOrEmpty(columns[13].Trim()) ? Convert.ToDecimal(columns[13].Trim()) : 0,
                        Surcharge = !String.IsNullOrEmpty(columns[14].Trim()) ? Convert.ToDecimal(columns[14].Trim()) : 0,
                        HealthEducationCess = !String.IsNullOrEmpty(columns[15].Trim()) ? Convert.ToDecimal(columns[15].Trim()) : 0,
                        TotalTaxDeducted = !String.IsNullOrEmpty(columns[16].Trim()) ? Convert.ToDecimal(columns[16].Trim()) : 0,
                        TotalTaxDeposited = !String.IsNullOrEmpty(columns[18].Trim()) ? Convert.ToDecimal(columns[18].Trim()) : 0,
                        TotalValueOfTheTransaction = !String.IsNullOrEmpty(columns[20].Trim()) ? Convert.ToDecimal(columns[20].Trim()) : 0,
                        AmountPaidCredited = !String.IsNullOrEmpty(columns[21].Trim()) ? Convert.ToDecimal(columns[21].Trim()) : 0,
                        DateOfPaymentCredit = formatted1,
                        DateOfDeduction = formatted2,
                        RateAtWhichTax = !String.IsNullOrEmpty(columns[25].Trim()) ? Convert.ToDecimal(columns[25].Trim()) : null,
                        Reasons = columns[29].Trim(),
                        SectionCode = columns[32].Trim(),
                        CertificationNo = columns[33].Trim(),
                        PaymentCovered = columns[34].Trim(),
                        ChallanNumber = columns[35].Trim(),
                        ChallanDate = formatted3,
                        OptingForRegime = columns[48].Trim(),
                    };
                    deductor.DeducteeEntry.Add(record);
                }
                if (columns != null && columns[1] == "DD" && deductor.Form == "24Q")
                {
                    string formatted1 = !String.IsNullOrEmpty(columns[22].Trim()) ? (columns[22].Trim().Substring(0, 2) + "/" + columns[22].Trim().Substring(2, 2) + "/" + columns[22].Trim().Substring(4, 4)) : null;
                    string formatted2 = !String.IsNullOrEmpty(columns[23].Trim()) ? (columns[23].Trim().Substring(0, 2) + "/" + columns[23].Trim().Substring(2, 2) + "/" + columns[23].Trim().Substring(4, 4)) : null;
                    var record = new DeducteeEntry()
                    {
                        SerialNo = Convert.ToInt16(columns[3].Trim()),
                        DeducteeCode = columns[7].Trim(),
                        DeducteeRef = columns[6].Trim(),
                        PanOfDeductee = columns[9].Trim(),
                        DeducteePanRef = columns[11].Trim(),
                        NameOfDeductee = columns[12].Trim(),
                        TDS = !String.IsNullOrEmpty(columns[13].Trim()) ? Convert.ToDecimal(columns[13].Trim()) : 0,
                        Surcharge = !String.IsNullOrEmpty(columns[14].Trim()) ? Convert.ToDecimal(columns[14].Trim()) : 0,
                        HealthEducationCess = !String.IsNullOrEmpty(columns[15].Trim()) ? Convert.ToDecimal(columns[15].Trim()) : 0,
                        TotalTaxDeducted = !String.IsNullOrEmpty(columns[16].Trim()) ? Convert.ToDecimal(columns[16].Trim()) : 0,
                        TotalTaxDeposited = !String.IsNullOrEmpty(columns[18].Trim()) ? Convert.ToDecimal(columns[18].Trim()) : 0,
                        AmountPaidCredited = !String.IsNullOrEmpty(columns[21].Trim()) ? Convert.ToDecimal(columns[21].Trim()) : 0,
                        DateOfPaymentCredit = formatted1,
                        DateOfDeduction = formatted2,
                        Reasons = columns[29].Trim(),
                        SectionCode = columns[32].Trim(),
                        CertificationNo = columns[33].Trim(),
                    };
                    deductor.DeducteeEntry.Add(record);
                }
                if (deductor.Form == "24Q")
                {
                    cateId = 1;
                }
                if (deductor.Form == "26Q")
                {
                    cateId = 2;
                }
                if (deductor.Form == "27Q")
                {
                    cateId = 4;
                }
                if (deductor.Form == "27EQ")
                {
                    cateId = 3;
                }
                if (columns != null && columns[1] == "SD" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    string formatted1 = !String.IsNullOrEmpty(columns[10].Trim()) ? (columns[10].Trim().Substring(0, 2) + "/" + columns[10].Trim().Substring(2, 2) + "/" + columns[10].Trim().Substring(4, 4)) : null;
                    string formatted2 = !String.IsNullOrEmpty(columns[11].Trim()) ? (columns[11].Trim().Substring(0, 2) + "/" + columns[11].Trim().Substring(2, 2) + "/" + columns[11].Trim().Substring(4, 4)) : null;
                    string formatted3 = !String.IsNullOrEmpty(columns[60].Trim()) ? (columns[60].Trim().Substring(0, 2) + "/" + columns[60].Trim().Substring(2, 2) + "/" + columns[60].Trim().Substring(4, 4)) : null;
                    string formatted4 = !String.IsNullOrEmpty(columns[61].Trim()) ? (columns[61].Trim().Substring(0, 2) + "/" + columns[61].Trim().Substring(2, 2) + "/" + columns[61].Trim().Substring(4, 4)) : null;
                    salaryRecord.CategoryId = cateId;
                    salaryRecord.WheatherPensioner = "No";
                    salaryRecord.FinancialYear = deductor.FinancialYear;
                    salaryRecord.Quarter = deductor.Quarter;
                    salaryRecord.SerialNo = Convert.ToInt16(columns[3].Trim());
                    salaryRecord.PanOfEmployee = columns[6].Trim();
                    salaryRecord.EmployeeRef = columns[7].Trim();
                    salaryRecord.NameOfEmploye = columns[8].Trim();
                    salaryRecord.CategoryEmployee = columns[9].Trim();
                    salaryRecord.PeriodOfFromDate = formatted1;
                    salaryRecord.PeriodOfToDate = formatted2;
                    salaryRecord.TotalAmount = !String.IsNullOrEmpty(columns[12].Trim()) ? Convert.ToDecimal(columns[12].Trim()) : 0;
                    salaryRecord.GrossTotalDeduction = !String.IsNullOrEmpty(columns[15].Trim()) ? Convert.ToDecimal(columns[15].Trim()) : 0;
                    salaryRecord.IncomeChargeable = !String.IsNullOrEmpty(columns[16].Trim()) ? Convert.ToDecimal(columns[16].Trim()) : 0;
                    salaryRecord.IncomeOrLoss = !String.IsNullOrEmpty(columns[17].Trim()) ? Convert.ToDecimal(columns[17].Trim()) : 0;
                    salaryRecord.GrossTotalIncome = !String.IsNullOrEmpty(columns[18].Trim()) ? Convert.ToDecimal(columns[18].Trim()) : 0;
                    salaryRecord.GrossTotalDeductionUnderVIA = !String.IsNullOrEmpty(columns[21].Trim()) ? Convert.ToDecimal(columns[21].Trim()) : 0;
                    salaryRecord.TotalTaxableIncome = !String.IsNullOrEmpty(columns[22].Trim()) ? Convert.ToDecimal(columns[22].Trim()) : 0;
                    salaryRecord.IncomeTaxOnTotalIncome = !String.IsNullOrEmpty(columns[23].Trim()) ? Convert.ToDecimal(columns[23].Trim()) : 0;
                    salaryRecord.Surcharge = !String.IsNullOrEmpty(columns[24].Trim()) ? Convert.ToDecimal(columns[24].Trim()) : 0;
                    salaryRecord.HealthAndEducationCess = !String.IsNullOrEmpty(columns[24].Trim()) ? Convert.ToDecimal(columns[25].Trim()) : 0;
                    salaryRecord.IncomeTaxReliefUnderSection89 = !String.IsNullOrEmpty(columns[26].Trim()) ? Convert.ToDecimal(columns[26].Trim()) : 0;
                    salaryRecord.NetTaxPayable = !String.IsNullOrEmpty(columns[27].Trim()) ? Convert.ToDecimal(columns[27].Trim()) : 0;
                    salaryRecord.TotalTDS = !String.IsNullOrEmpty(columns[28].Trim()) ? Convert.ToDecimal(columns[28].Trim()) : 0;
                    salaryRecord.ShortfallExcess = !String.IsNullOrEmpty(columns[29].Trim()) ? Convert.ToDecimal(columns[29].Trim()) : 0;
                    salaryRecord.AggregateAmountOfDeductions = !String.IsNullOrEmpty(columns[30].Trim()) ? Convert.ToDecimal(columns[30].Trim()) : 0;
                    salaryRecord.TaxableAmount = !String.IsNullOrEmpty(columns[33].Trim()) ? Convert.ToDecimal(columns[33].Trim()) : 0;
                    salaryRecord.ReportedTaxableAmount = !String.IsNullOrEmpty(columns[34].Trim()) ? Convert.ToDecimal(columns[34].Trim()) : 0;
                    salaryRecord.TotalAmountofTaxDeducted = !String.IsNullOrEmpty(columns[35].Trim()) ? Convert.ToDecimal(columns[35].Trim()) : 0;
                    salaryRecord.ReportedAmountOfTax = !String.IsNullOrEmpty(columns[36].Trim()) ? Convert.ToDecimal(columns[36].Trim()) : 0;
                    salaryRecord.WheathertaxDeductedAt = !String.IsNullOrEmpty(columns[37].Trim()) ? columns[37].Trim() : "";
                    salaryRecord.WheatherRentPayment = columns[38].Trim();
                    salaryRecord.PanOfLandlord1 = columns[40].Trim();
                    salaryRecord.NameOfLandlord1 = columns[41].Trim();
                    salaryRecord.PanOfLandlord2 = columns[42].Trim();
                    salaryRecord.NameOfLandlord2 = columns[43].Trim();
                    salaryRecord.PanOfLandlord3 = columns[44].Trim();
                    salaryRecord.NameOfLandlord3 = columns[45].Trim();
                    salaryRecord.PanOfLandlord4 = columns[46].Trim();
                    salaryRecord.NameOfLandlord4 = columns[47].Trim();
                    salaryRecord.WheatherInterest = columns[48].Trim();
                    salaryRecord.PanOfLander1 = columns[50].Trim();
                    salaryRecord.NameOfLander1 = columns[51].Trim();
                    salaryRecord.PanOfLander2 = columns[52].Trim();
                    salaryRecord.NameOfLander2 = columns[53].Trim();
                    salaryRecord.PanOfLander3 = columns[54].Trim();
                    salaryRecord.NameOfLander3 = columns[55].Trim();
                    salaryRecord.PanOfLander4 = columns[56].Trim();
                    salaryRecord.NameOfLander4 = columns[57].Trim();
                    salaryRecord.WheatherContributions = columns[58].Trim();
                    salaryRecord.NameOfTheSuperanuation = columns[59].Trim();
                    salaryRecord.DateFromWhichtheEmployee = formatted3;
                    salaryRecord.DateToWhichtheEmployee = formatted4;
                    salaryRecord.TheAmountOfContribution = !String.IsNullOrEmpty(columns[62].Trim()) ? Convert.ToDecimal(columns[62].Trim()) : 0;
                    salaryRecord.TheAvarageRateOfDeduction = !String.IsNullOrEmpty(columns[63].Trim()) ? Convert.ToDecimal(columns[63].Trim()) : 0;
                    salaryRecord.TheAmountOfTaxDeduction = !String.IsNullOrEmpty(columns[64].Trim()) ? Convert.ToDecimal(columns[64].Trim()) : 0;
                    salaryRecord.GrossTotalIncomeCS = !String.IsNullOrEmpty(columns[65].Trim()) ? Convert.ToDecimal(columns[65].Trim()) : 0;
                    salaryRecord.GrossSalary = !String.IsNullOrEmpty(columns[66].Trim()) ? Convert.ToDecimal(columns[66].Trim()) : 0;
                    salaryRecord.ValueOfPerquisites = !String.IsNullOrEmpty(columns[67].Trim()) ? Convert.ToDecimal(columns[67].Trim()) : 0;
                    salaryRecord.ProfitsInLieuOf = !String.IsNullOrEmpty(columns[68].Trim()) ? Convert.ToDecimal(columns[68].Trim()) : 0;
                    salaryRecord.TravelConcession = !String.IsNullOrEmpty(columns[69].Trim()) ? Convert.ToDecimal(columns[69].Trim()) : 0;
                    salaryRecord.DeathCumRetirement = !String.IsNullOrEmpty(columns[70].Trim()) ? Convert.ToDecimal(columns[70].Trim()) : 0;
                    salaryRecord.ComputedValue = !String.IsNullOrEmpty(columns[71].Trim()) ? Convert.ToDecimal(columns[71].Trim()) : 0;
                    salaryRecord.CashEquivalent = !String.IsNullOrEmpty(columns[72].Trim()) ? Convert.ToDecimal(columns[72].Trim()) : 0;
                    salaryRecord.HouseRent = !String.IsNullOrEmpty(columns[73].Trim()) ? Convert.ToDecimal(columns[73].Trim()) : 0;
                    salaryRecord.AmountOfExemption = !String.IsNullOrEmpty(columns[74].Trim()) ? Convert.ToDecimal(columns[74].Trim()) : 0;
                    salaryRecord.TotalAmountOfExemption = !String.IsNullOrEmpty(columns[75].Trim()) ? Convert.ToDecimal(columns[75].Trim()) : 0;
                    salaryRecord.IncomeOtherSources = !String.IsNullOrEmpty(columns[76].Trim()) ? Convert.ToDecimal(columns[76].Trim()) : 0;
                    salaryRecord.Rebate87A = !String.IsNullOrEmpty(columns[77].Trim()) ? Convert.ToDecimal(columns[77].Trim()) : 0;
                    salaryRecord.NewRegime = !String.IsNullOrEmpty(columns[78].Trim()) ? (columns[78].Trim() == "Y" ? "N" : "Y") : "";
                    salaryRecord.OtherSpecial = !String.IsNullOrEmpty(columns[79].Trim()) ? Convert.ToDecimal(columns[79].Trim()) : 0;
                    salaryRecord.AmountReported = !String.IsNullOrEmpty(columns[80].Trim()) ? Convert.ToDecimal(columns[80].Trim()) : 0;
                    deductor.SalaryDetail.Add(salaryRecord);
                }

                if (columns != null && columns[1] == "S16" && columns[5] == "16(ia)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).StandardDeduction = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                }
                if (columns != null && columns[1] == "S16" && columns[3].Trim() == columns[3].Trim() && columns[5] == "16(ii)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).DeductionUSII = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                }
                if (columns != null && columns[1] == "S16" && columns[3].Trim() == columns[3].Trim() && columns[5] == "16(iii)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).DeductionUSIII = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "OTHERS" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionVIADeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionVIAGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionVIAQualifying = !String.IsNullOrEmpty(columns[8].Trim()) ? Convert.ToDecimal(columns[8].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80C" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80CCC" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCCDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCCGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80CCD(1)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCD1Deductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCD1Gross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80CCD(1B)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCD1BDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCD1BGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80CCD(2)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCD2Deductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCD2Gross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80D" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionDDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionDGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80E" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionEDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionEGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80G" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionGDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionGGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionGQualifying = !String.IsNullOrEmpty(columns[8].Trim()) ? Convert.ToDecimal(columns[8].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80TTA" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySection80TTADeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySection80TTAGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySection80TTAQualifying = !String.IsNullOrEmpty(columns[8].Trim()) ? Convert.ToDecimal(columns[8].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80CCH" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCDHDeductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCDHGross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "C6A" && columns[3].Trim() == columns[3].Trim() && columns[5] == "80CCH(1)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCDH2Deductiable = !String.IsNullOrEmpty(columns[6].Trim()) ? Convert.ToDecimal(columns[6].Trim()) : 0;
                    deductor.SalaryDetail.Find(p => p.SerialNo == Convert.ToInt16(columns[3].Trim())).EightySectionCCDH2Gross = !String.IsNullOrEmpty(columns[7].Trim()) ? Convert.ToDecimal(columns[7].Trim()) : 0;
                }
                if (columns != null && columns[1] == "94P" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                {
                    salaryRecord.CategoryId = cateId;
                    salaryRecord.WheatherPensioner = "Yes";
                    salaryRecord.FinancialYear = deductor.FinancialYear;
                    salaryRecord.Quarter = deductor.Quarter;
                    salaryRecord.PanOfEmployee = columns[5].Trim();
                    salaryRecord.NameOfEmploye = columns[6].Trim();
                    salaryRecord.CategoryEmployee = columns[7].Trim();
                    salaryRecord.NewRegime = (columns[8].Trim() == "Y" ? "N" : "Y");
                    salaryRecord.GrossSalary = !String.IsNullOrEmpty(columns[9].Trim()) ? Convert.ToDecimal(columns[9].Trim()) : 0;
                    salaryRecord.GrossTotalDeduction = !String.IsNullOrEmpty(columns[11].Trim()) ? Convert.ToDecimal(columns[11].Trim()) : 0;
                    salaryRecord.IncomeChargeable = !String.IsNullOrEmpty(columns[12].Trim()) ? Convert.ToDecimal(columns[12].Trim()) : 0;
                    salaryRecord.IncomeOtherSources = !String.IsNullOrEmpty(columns[13].Trim()) ? Convert.ToDecimal(columns[13].Trim()) : 0;
                    salaryRecord.GrossTotalIncome = !String.IsNullOrEmpty(columns[14].Trim()) ? Convert.ToDecimal(columns[14].Trim()) : 0;
                    salaryRecord.AggregateAmountOfDeductions = !String.IsNullOrEmpty(columns[16].Trim()) ? Convert.ToDecimal(columns[16].Trim()) : 0;
                    salaryRecord.GrossTotalDeductionUnderVIA = !String.IsNullOrEmpty(columns[18].Trim()) ? Convert.ToDecimal(columns[18].Trim()) : 0;
                    salaryRecord.TotalTaxableIncome = !String.IsNullOrEmpty(columns[19].Trim()) ? Convert.ToDecimal(columns[19].Trim()) : 0;
                    salaryRecord.IncomeTaxOnTotalIncome = !String.IsNullOrEmpty(columns[20].Trim()) ? Convert.ToDecimal(columns[20].Trim()) : 0;
                    salaryRecord.Rebate87A = !String.IsNullOrEmpty(columns[21].Trim()) ? Convert.ToDecimal(columns[21].Trim()) : 0;
                    salaryRecord.Surcharge = !String.IsNullOrEmpty(columns[22].Trim()) ? Convert.ToDecimal(columns[22].Trim()) : 0;
                    salaryRecord.HealthAndEducationCess = !String.IsNullOrEmpty(columns[23].Trim()) ? Convert.ToDecimal(columns[23].Trim()) : 0;
                    salaryRecord.TotalPayable = !String.IsNullOrEmpty(columns[24].Trim()) ? Convert.ToDecimal(columns[24].Trim()) : 0;
                    salaryRecord.IncomeTaxReliefUnderSection89 = !String.IsNullOrEmpty(columns[25].Trim()) ? Convert.ToDecimal(columns[25].Trim()) : 0;
                    salaryRecord.NetTaxPayable = !String.IsNullOrEmpty(columns[26].Trim()) ? Convert.ToDecimal(columns[26].Trim()) : 0;
                    foreach (var line1 in lines.Skip(1)) // skip header
                    {
                        var columns1 = line1.Replace("^", "*").ToString().Split('*');

                        if (columns1 != null && columns1[1] == "P16" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "16(ia)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.StandardDeduction = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P16" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "16(iii)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.DeductionUSIII = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns1[3].Trim() == columns[3].Trim() && columns1[5] == "80C" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80CCC" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCCGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCCDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80CCD(1)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCD1Gross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCD1Deductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80CCD(1B)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCD1BGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCD1BDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80CCD(2)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCD2Gross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCD2Deductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80CCH" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCDHGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCDHDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80CCH(1)" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCDH2Gross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCDH2Deductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80D" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionDGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionDDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80E" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionEGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionEDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80G" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionGGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionGQualifying = !String.IsNullOrEmpty(columns1[7].Trim()) ? Convert.ToDecimal(columns1[7].Trim()) : 0;
                            salaryRecord.EightySectionGDeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "80TTB" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySection80TTAGross = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySection80TTAQualifying = !String.IsNullOrEmpty(columns1[7].Trim()) ? Convert.ToDecimal(columns1[7].Trim()) : 0;
                            salaryRecord.EightySection80TTADeductiable = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                        if (columns1 != null && columns1[1] == "P6A" && columns[3].Trim() == columns1[3].Trim() && columns1[5] == "OTHERS" && deductor.Form == "24Q" && deductor.Quarter == "Q4")
                        {
                            salaryRecord.EightySectionCCDH2Deductiable = !String.IsNullOrEmpty(columns1[6].Trim()) ? Convert.ToDecimal(columns1[6].Trim()) : 0;
                            salaryRecord.EightySectionCCDH2Gross = !String.IsNullOrEmpty(columns1[7].Trim()) ? Convert.ToDecimal(columns1[7].Trim()) : 0;
                            salaryRecord.EightySectionCCDH2Gross = !String.IsNullOrEmpty(columns1[8].Trim()) ? Convert.ToDecimal(columns1[8].Trim()) : 0;
                        }
                    }
                    deductor.SalaryDetail.Add(salaryRecord);
                }
            }
            return deductor;
        }

        public async Task<List<Deductor>> GetCompanyDetail(IFormFile file, string Path, string type = null)
        {
            var companys = new List<Deductor>();

            if (file.FileName != null)
            {
                FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream)
    ;
                DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                DataTable dt = dataset.Tables[0];

                string text = (type != "1" && type != "2") ? dt.Rows[0][4].ToString(): dt.Rows[0][3].ToString();
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if (!String.IsNullOrEmpty(dt.Rows[1][0].ToString()))
                    {
                        var model = new Deductor()
                        {
                            DeductorTan = dt.Rows[i][0].ToString(),
                            DeductorPan = dt.Rows[i][1].ToString(),
                            DeductorName = dt.Rows[i][2].ToString(),
                            DeductorType = text
                        };
                        companys.Add(model);
                    }
                }
            }
            return companys;
        }

        public bool DeleteFiles(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return true;
        }

        public async Task<List<SalaryDetail>> GetUploadSalaryDeatil(IFormFile file, string Path, FormDashboardFilter model, int userId)
        {
            var salaryDetails = new List<SalaryDetail>();
            try
            {
                if (file.FileName != null)
                {
                    FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTable dt = dataset.Tables[3];
                    for (int i = 2; i < dt.Rows.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()))
                        {
                            var date = Common.GetValidDateTime(dt.Rows[i][5].ToString());
                            var date1 = Common.GetValidDateTime(dt.Rows[i][6].ToString());
                            var date2 = Common.GetValidDateTime(dt.Rows[i][7].ToString());
                            var date3 = Common.GetValidDateTime(dt.Rows[i][98].ToString());
                            var date4 = Common.GetValidDateTime(dt.Rows[i][99].ToString());
                            var salaryDetail = new SalaryDetail()
                            {
                                EmployeeRef = dt.Rows[i][0].ToString(),
                                PanOfEmployee = dt.Rows[i][1].ToString(),
                                NameOfEmploye = dt.Rows[i][2].ToString(),
                                Desitnation = dt.Rows[i][3].ToString(),
                                CategoryEmployee = Helper.GetEnumMemberValueByDescription<EmployeeCategory>(dt.Rows[i][4].ToString()),
                                DateOfBirth = !String.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? date.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                PeriodOfFromDate = !String.IsNullOrEmpty(dt.Rows[i][6].ToString()) ? date1.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                PeriodOfToDate = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? date2.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                NewRegime = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? (dt.Rows[i][8].ToString() == "Yes" ? "Y" : "N") : null,
                                GrossSalary = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? Convert.ToDecimal(dt.Rows[i][9].ToString()) : 0,
                                ValueOfPerquisites = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? Convert.ToDecimal(dt.Rows[i][10].ToString()) : 0,
                                ProfitsInLieuOf = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                TaxableAmount = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                ReportedTaxableAmount = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                TotalAmount = !String.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? Convert.ToDecimal(dt.Rows[i][14].ToString()) : 0,
                                TravelConcession = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Convert.ToDecimal(dt.Rows[i][15].ToString()) : 0,
                                DeathCumRetirement = !String.IsNullOrEmpty(dt.Rows[i][16].ToString()) ? Convert.ToDecimal(dt.Rows[i][16].ToString()) : 0,
                                ComputedValue = !String.IsNullOrEmpty(dt.Rows[i][17].ToString()) ? Convert.ToDecimal(dt.Rows[i][17].ToString()) : 0,
                                CashEquivalent = !String.IsNullOrEmpty(dt.Rows[i][18].ToString()) ? Convert.ToDecimal(dt.Rows[i][18].ToString()) : 0,
                                HouseRent = !String.IsNullOrEmpty(dt.Rows[i][19].ToString()) ? Convert.ToDecimal(dt.Rows[i][19].ToString()) : 0,
                                OtherSpecial = !String.IsNullOrEmpty(dt.Rows[i][20].ToString()) ? Convert.ToDecimal(dt.Rows[i][20].ToString()) : 0,
                                AmountOfExemption = !String.IsNullOrEmpty(dt.Rows[i][21].ToString()) ? Convert.ToDecimal(dt.Rows[i][21].ToString()) : 0,
                                TotalAmountOfExemption = !String.IsNullOrEmpty(dt.Rows[i][22].ToString()) ? Convert.ToDecimal(dt.Rows[i][22].ToString()) : 0,
                                StandardDeductionMannualEdit = dt.Rows[i][23].ToString(),
                                StandardDeduction = !String.IsNullOrEmpty(dt.Rows[i][24].ToString()) ? Convert.ToDecimal(dt.Rows[i][24].ToString()) : 0,
                                DeductionUSII = !String.IsNullOrEmpty(dt.Rows[i][25].ToString()) ? Convert.ToDecimal(dt.Rows[i][25].ToString()) : 0,
                                DeductionUSIII = !String.IsNullOrEmpty(dt.Rows[i][26].ToString()) ? Convert.ToDecimal(dt.Rows[i][26].ToString()) : 0,
                                GrossTotalDeduction = !String.IsNullOrEmpty(dt.Rows[i][27].ToString()) ? Convert.ToDecimal(dt.Rows[i][27].ToString()) : 0,
                                IncomeChargeable = !String.IsNullOrEmpty(dt.Rows[i][28].ToString()) ? Convert.ToDecimal(dt.Rows[i][28].ToString()) : 0,
                                IncomeOrLoss = !String.IsNullOrEmpty(dt.Rows[i][29].ToString()) ? Convert.ToDecimal(dt.Rows[i][29].ToString()) : 0,
                                IncomeOtherSources = !String.IsNullOrEmpty(dt.Rows[i][30].ToString()) ? Convert.ToDecimal(dt.Rows[i][30].ToString()) : 0,
                                GrossTotalIncome = !String.IsNullOrEmpty(dt.Rows[i][31].ToString()) ? Convert.ToDecimal(dt.Rows[i][31].ToString()) : 0,
                                EightySectionCGross = !String.IsNullOrEmpty(dt.Rows[i][32].ToString()) ? Convert.ToDecimal(dt.Rows[i][32].ToString()) : 0,
                                EightySectionCDeductiable = !String.IsNullOrEmpty(dt.Rows[i][33].ToString()) ? Convert.ToDecimal(dt.Rows[i][33].ToString()) : 0,
                                EightySectionCCCGross = !String.IsNullOrEmpty(dt.Rows[i][34].ToString()) ? Convert.ToDecimal(dt.Rows[i][34].ToString()) : 0,
                                EightySectionCCCDeductiable = !String.IsNullOrEmpty(dt.Rows[i][35].ToString()) ? Convert.ToDecimal(dt.Rows[i][35].ToString()) : 0,
                                EightySectionCCD1Gross = !String.IsNullOrEmpty(dt.Rows[i][36].ToString()) ? Convert.ToDecimal(dt.Rows[i][36].ToString()) : 0,
                                EightySectionCCD1Deductiable = !String.IsNullOrEmpty(dt.Rows[i][37].ToString()) ? Convert.ToDecimal(dt.Rows[i][37].ToString()) : 0,
                                AggregateAmountOfDeductions = !String.IsNullOrEmpty(dt.Rows[i][38].ToString()) ? Convert.ToDecimal(dt.Rows[i][38].ToString()) : 0,
                                EightySectionCCD1BGross = !String.IsNullOrEmpty(dt.Rows[i][39].ToString()) ? Convert.ToDecimal(dt.Rows[i][39].ToString()) : 0,
                                EightySectionCCD1BDeductiable = !String.IsNullOrEmpty(dt.Rows[i][40].ToString()) ? Convert.ToDecimal(dt.Rows[i][40].ToString()) : 0,
                                EightySectionCCD2Gross = !String.IsNullOrEmpty(dt.Rows[i][41].ToString()) ? Convert.ToDecimal(dt.Rows[i][41].ToString()) : 0,
                                EightySectionCCD2Deductiable = !String.IsNullOrEmpty(dt.Rows[i][42].ToString()) ? Convert.ToDecimal(dt.Rows[i][42].ToString()) : 0,
                                EightySectionCCDHGross = !String.IsNullOrEmpty(dt.Rows[i][43].ToString()) ? Convert.ToDecimal(dt.Rows[i][43].ToString()) : 0,
                                EightySectionCCDHDeductiable = !String.IsNullOrEmpty(dt.Rows[i][44].ToString()) ? Convert.ToDecimal(dt.Rows[i][44].ToString()) : 0,
                                EightySectionCCDH2Gross = !String.IsNullOrEmpty(dt.Rows[i][45].ToString()) ? Convert.ToDecimal(dt.Rows[i][45].ToString()) : 0,
                                EightySectionCCDH2Deductiable = !String.IsNullOrEmpty(dt.Rows[i][46].ToString()) ? Convert.ToDecimal(dt.Rows[i][46].ToString()) : 0,
                                EightySectionDGross = !String.IsNullOrEmpty(dt.Rows[i][47].ToString()) ? Convert.ToDecimal(dt.Rows[i][47].ToString()) : 0,
                                EightySectionDDeductiable = !String.IsNullOrEmpty(dt.Rows[i][48].ToString()) ? Convert.ToDecimal(dt.Rows[i][48].ToString()) : 0,
                                EightySectionEGross = !String.IsNullOrEmpty(dt.Rows[i][49].ToString()) ? Convert.ToDecimal(dt.Rows[i][49].ToString()) : 0,
                                EightySectionEDeductiable = !String.IsNullOrEmpty(dt.Rows[i][50].ToString()) ? Convert.ToDecimal(dt.Rows[i][50].ToString()) : 0,
                                EightySectionGGross = !String.IsNullOrEmpty(dt.Rows[i][51].ToString()) ? Convert.ToDecimal(dt.Rows[i][51].ToString()) : 0,
                                EightySectionGQualifying = !String.IsNullOrEmpty(dt.Rows[i][52].ToString()) ? Convert.ToDecimal(dt.Rows[i][52].ToString()) : 0,
                                EightySectionGDeductiable = !String.IsNullOrEmpty(dt.Rows[i][53].ToString()) ? Convert.ToDecimal(dt.Rows[i][53].ToString()) : 0,
                                EightySection80TTAGross = !String.IsNullOrEmpty(dt.Rows[i][54].ToString()) ? Convert.ToDecimal(dt.Rows[i][54].ToString()) : 0,
                                EightySection80TTAQualifying = !String.IsNullOrEmpty(dt.Rows[i][55].ToString()) ? Convert.ToDecimal(dt.Rows[i][55].ToString()) : 0,
                                EightySection80TTADeductiable = !String.IsNullOrEmpty(dt.Rows[i][56].ToString()) ? Convert.ToDecimal(dt.Rows[i][56].ToString()) : 0,
                                EightySectionVIAGross = !String.IsNullOrEmpty(dt.Rows[i][57].ToString()) ? Convert.ToDecimal(dt.Rows[i][57].ToString()) : 0,
                                EightySectionVIAQualifying = !String.IsNullOrEmpty(dt.Rows[i][58].ToString()) ? Convert.ToDecimal(dt.Rows[i][58].ToString()) : 0,
                                EightySectionVIADeductiable = !String.IsNullOrEmpty(dt.Rows[i][59].ToString()) ? Convert.ToDecimal(dt.Rows[i][59].ToString()) : 0,
                                GrossTotalDeductionUnderVIA = !String.IsNullOrEmpty(dt.Rows[i][60].ToString()) ? Convert.ToDecimal(dt.Rows[i][60].ToString()) : 0,
                                TotalTaxableIncome = !String.IsNullOrEmpty(dt.Rows[i][61].ToString()) ? Convert.ToDecimal(dt.Rows[i][61].ToString()) : 0,
                                IncomeTaxOnTotalIncomeMannualEdit = !String.IsNullOrEmpty(dt.Rows[i][62].ToString()) ? dt.Rows[i][62].ToString() : null,
                                IncomeTaxOnTotalIncome = !String.IsNullOrEmpty(dt.Rows[i][63].ToString()) ? Convert.ToDecimal(dt.Rows[i][63].ToString()) : 0,
                                Rebate87AMannualEdit = !String.IsNullOrEmpty(dt.Rows[i][64].ToString()) ? dt.Rows[i][64].ToString() : null,
                                Rebate87A = !String.IsNullOrEmpty(dt.Rows[i][65].ToString()) ? Convert.ToDecimal(dt.Rows[i][65].ToString()) : 0,
                                IncomeTaxOnTotalIncomeAfterRebate87A = !String.IsNullOrEmpty(dt.Rows[i][66].ToString()) ? Convert.ToDecimal(dt.Rows[i][66].ToString()) : 0,
                                Surcharge = !String.IsNullOrEmpty(dt.Rows[i][67].ToString()) ? Convert.ToDecimal(dt.Rows[i][67].ToString()) : 0,
                                HealthAndEducationCess = !String.IsNullOrEmpty(dt.Rows[i][68].ToString()) ? Convert.ToDecimal(dt.Rows[i][68].ToString()) : 0,
                                TotalPayable = !String.IsNullOrEmpty(dt.Rows[i][69].ToString()) ? Convert.ToDecimal(dt.Rows[i][69].ToString()) : 0,
                                IncomeTaxReliefUnderSection89 = !String.IsNullOrEmpty(dt.Rows[i][70].ToString()) ? Convert.ToDecimal(dt.Rows[i][70].ToString()) : 0,
                                NetTaxPayable = !String.IsNullOrEmpty(dt.Rows[i][71].ToString()) ? Convert.ToDecimal(dt.Rows[i][71].ToString()) : 0,
                                TotalAmountofTaxDeducted = !String.IsNullOrEmpty(dt.Rows[i][72].ToString()) ? Convert.ToDecimal(dt.Rows[i][72].ToString()) : 0,
                                ReportedAmountOfTax = !String.IsNullOrEmpty(dt.Rows[i][73].ToString()) ? Convert.ToDecimal(dt.Rows[i][73].ToString()) : 0,
                                AmountReported = !String.IsNullOrEmpty(dt.Rows[i][74].ToString()) ? Convert.ToDecimal(dt.Rows[i][74].ToString()) : 0,
                                TotalTDS = !String.IsNullOrEmpty(dt.Rows[i][75].ToString()) ? Convert.ToDecimal(dt.Rows[i][75].ToString()) : 0,
                                ShortfallExcess = !String.IsNullOrEmpty(dt.Rows[i][76].ToString()) ? Convert.ToDecimal(dt.Rows[i][76].ToString()) : 0,
                                WheathertaxDeductedAt = !String.IsNullOrEmpty(dt.Rows[i][77].ToString()) ? dt.Rows[i][77].ToString() == "Yes" ? "Y" : "N" : "N",
                                WheatherRentPayment = !String.IsNullOrEmpty(dt.Rows[i][78].ToString()) ? dt.Rows[i][78].ToString() == "Yes" ? "Y" : "N" : "N",
                                PanOfLandlord1 = dt.Rows[i][79].ToString(),
                                NameOfLandlord1 = dt.Rows[i][80].ToString(),
                                PanOfLandlord2 = dt.Rows[i][81].ToString(),
                                NameOfLandlord2 = dt.Rows[i][82].ToString(),
                                PanOfLandlord3 = dt.Rows[i][83].ToString(),
                                NameOfLandlord3 = dt.Rows[i][84].ToString(),
                                PanOfLandlord4 = dt.Rows[i][85].ToString(),
                                NameOfLandlord4 = dt.Rows[i][86].ToString(),
                                WheatherInterest = !String.IsNullOrEmpty(dt.Rows[i][87].ToString()) ? dt.Rows[i][87].ToString() == "Yes" ? "Y" : "N" : "N",
                                PanOfLander1 = dt.Rows[i][88].ToString(),
                                NameOfLander1 = dt.Rows[i][89].ToString(),
                                PanOfLander2 = dt.Rows[i][90].ToString(),
                                NameOfLander2 = dt.Rows[i][91].ToString(),
                                PanOfLander3 = dt.Rows[i][92].ToString(),
                                NameOfLander3 = dt.Rows[i][93].ToString(),
                                PanOfLander4 = dt.Rows[i][94].ToString(),
                                NameOfLander4 = dt.Rows[i][95].ToString(),
                                WheatherContributions = !String.IsNullOrEmpty(dt.Rows[i][96].ToString()) ? dt.Rows[i][96].ToString() == "Yes" ? "Y" : "N" : "N",
                                NameOfTheSuperanuation = dt.Rows[i][97].ToString(),
                                DateFromWhichtheEmployee = !String.IsNullOrEmpty(dt.Rows[i][98].ToString()) ? date3.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                DateToWhichtheEmployee = !String.IsNullOrEmpty(dt.Rows[i][99].ToString()) ? date4.ToString("dd/MM/yyyy").Replace("-", "/") : null,
                                TheAmountOfContribution = !String.IsNullOrEmpty(dt.Rows[i][100].ToString()) ? Convert.ToDecimal(dt.Rows[i][100].ToString()) : 0,
                                TheAvarageRateOfDeduction = !String.IsNullOrEmpty(dt.Rows[i][101].ToString()) ? Convert.ToDecimal(dt.Rows[i][101].ToString()) : 0,
                                TheAmountOfTaxDeduction = !String.IsNullOrEmpty(dt.Rows[i][102].ToString()) ? Convert.ToDecimal(dt.Rows[i][102].ToString()) : 0,
                                GrossTotalIncomeCS = !String.IsNullOrEmpty(dt.Rows[i][103].ToString()) ? Convert.ToDecimal(dt.Rows[i][103].ToString()) : 0,
                                WheatherPensioner = dt.Rows[i][104].ToString(),
                                CategoryId = model.CategoryId,
                                UserId = userId,
                                FinancialYear = model.FinancialYear,
                                Quarter = model.Quarter,
                                DeductorId = model.DeductorId,
                            };
                            salaryDetails.Add(salaryDetail);
                        }
                    }
                }
                return salaryDetails;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<SaveSalaryPerksModel>> GetUploadSalaryPerks(IFormFile file, string Path, int userId)
        {
            var salaryPerks = new List<SaveSalaryPerksModel>();
            try
            {
                if (file.FileName != null)
                {
                    FileStream stream = File.Open(Path, FileMode.OpenOrCreate);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    DataSet dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (IExcelDataReader tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTable dt = dataset.Tables[4];

                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(dt.Rows[i][1].ToString()))
                        {
                            var salaryDetail = new SaveSalaryPerksModel()
                            {
                                PanOfEmployee = dt.Rows[i][0].ToString(),
                                EmployeeName = dt.Rows[i][1].ToString(),
                                AccommodationValue = !String.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? Convert.ToDecimal(dt.Rows[i][2].ToString()) : 0,
                                AccommodationAmount = !String.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? Convert.ToDecimal(dt.Rows[i][3].ToString()) : 0,
                                CarsValue = !String.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? Convert.ToDecimal(dt.Rows[i][4].ToString()) : 0,
                                CarsAmount = !String.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? Convert.ToDecimal(dt.Rows[i][5].ToString()) : 0,
                                SweeperValue = !String.IsNullOrEmpty(dt.Rows[i][6].ToString()) ? Convert.ToDecimal(dt.Rows[i][6].ToString()) : 0,
                                SweeperAmount = !String.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? Convert.ToDecimal(dt.Rows[i][7].ToString()) : 0,
                                GasValue = !String.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? Convert.ToDecimal(dt.Rows[i][8].ToString()) : 0,
                                GasAmount = !String.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? Convert.ToDecimal(dt.Rows[i][9].ToString()) : 0,
                                InterestValue = !String.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? Convert.ToDecimal(dt.Rows[i][10].ToString()) : 0,
                                InterestAmount = !String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Convert.ToDecimal(dt.Rows[i][11].ToString()) : 0,
                                HolidayValue = !String.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? Convert.ToDecimal(dt.Rows[i][12].ToString()) : 0,
                                HolidayAmount = !String.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? Convert.ToDecimal(dt.Rows[i][13].ToString()) : 0,
                                FreeTravelValue = !String.IsNullOrEmpty(dt.Rows[i][14].ToString()) ? Convert.ToDecimal(dt.Rows[i][14].ToString()) : 0,
                                FreeTravelAmount = !String.IsNullOrEmpty(dt.Rows[i][15].ToString()) ? Convert.ToDecimal(dt.Rows[i][15].ToString()) : 0,
                                FreeMealsValue = !String.IsNullOrEmpty(dt.Rows[i][16].ToString()) ? Convert.ToDecimal(dt.Rows[i][16].ToString()) : 0,
                                FreeMealsAmount = !String.IsNullOrEmpty(dt.Rows[i][17].ToString()) ? Convert.ToDecimal(dt.Rows[i][17].ToString()) : 0,
                                FreeEducationValue = !String.IsNullOrEmpty(dt.Rows[i][18].ToString()) ? Convert.ToDecimal(dt.Rows[i][18].ToString()) : 0,
                                FreeEducationAmount = !String.IsNullOrEmpty(dt.Rows[i][19].ToString()) ? Convert.ToDecimal(dt.Rows[i][19].ToString()) : 0,
                                GiftsValue = !String.IsNullOrEmpty(dt.Rows[i][20].ToString()) ? Convert.ToDecimal(dt.Rows[i][20].ToString()) : 0,
                                GiftsAmount = !String.IsNullOrEmpty(dt.Rows[i][21].ToString()) ? Convert.ToDecimal(dt.Rows[i][21].ToString()) : 0,
                                CreditCardValue = !String.IsNullOrEmpty(dt.Rows[i][22].ToString()) ? Convert.ToDecimal(dt.Rows[i][22].ToString()) : 0,
                                CreditCardAmount = !String.IsNullOrEmpty(dt.Rows[i][23].ToString()) ? Convert.ToDecimal(dt.Rows[i][23].ToString()) : 0,
                                ClubValue = !String.IsNullOrEmpty(dt.Rows[i][24].ToString()) ? Convert.ToDecimal(dt.Rows[i][24].ToString()) : 0,
                                ClubAmount = !String.IsNullOrEmpty(dt.Rows[i][25].ToString()) ? Convert.ToDecimal(dt.Rows[i][25].ToString()) : 0,
                                UseOfMoveableValue = !String.IsNullOrEmpty(dt.Rows[i][26].ToString()) ? Convert.ToDecimal(dt.Rows[i][26].ToString()) : 0,
                                UseOfMoveableAmount = !String.IsNullOrEmpty(dt.Rows[i][27].ToString()) ? Convert.ToDecimal(dt.Rows[i][27].ToString()) : 0,
                                TransferOfAssetValue = !String.IsNullOrEmpty(dt.Rows[i][28].ToString()) ? Convert.ToDecimal(dt.Rows[i][28].ToString()) : 0,
                                TransferOfAssetAmount = !String.IsNullOrEmpty(dt.Rows[i][29].ToString()) ? Convert.ToDecimal(dt.Rows[i][29].ToString()) : 0,
                                ValueOfAnyOtherValue = !String.IsNullOrEmpty(dt.Rows[i][30].ToString()) ? Convert.ToDecimal(dt.Rows[i][30].ToString()) : 0,
                                ValueOfAnyOtherAmount = !String.IsNullOrEmpty(dt.Rows[i][31].ToString()) ? Convert.ToDecimal(dt.Rows[i][31].ToString()) : 0,
                                Stock16IACValue = !String.IsNullOrEmpty(dt.Rows[i][32].ToString()) ? Convert.ToDecimal(dt.Rows[i][32].ToString()) : 0,
                                Stock16IACAmount = !String.IsNullOrEmpty(dt.Rows[i][33].ToString()) ? Convert.ToDecimal(dt.Rows[i][33].ToString()) : 0,
                                StockAboveValue = !String.IsNullOrEmpty(dt.Rows[i][34].ToString()) ? Convert.ToDecimal(dt.Rows[i][34].ToString()) : 0,
                                StockAboveAmount = !String.IsNullOrEmpty(dt.Rows[i][35].ToString()) ? Convert.ToDecimal(dt.Rows[i][35].ToString()) : 0,
                                ContributionValue = !String.IsNullOrEmpty(dt.Rows[i][36].ToString()) ? Convert.ToDecimal(dt.Rows[i][36].ToString()) : 0,
                                ContributionAmount = !String.IsNullOrEmpty(dt.Rows[i][37].ToString()) ? Convert.ToDecimal(dt.Rows[i][37].ToString()) : 0,
                                AnnualValue = !String.IsNullOrEmpty(dt.Rows[i][38].ToString()) ? Convert.ToDecimal(dt.Rows[i][38].ToString()) : 0,
                                AnnualAmount = !String.IsNullOrEmpty(dt.Rows[i][39].ToString()) ? Convert.ToDecimal(dt.Rows[i][39].ToString()) : 0,
                                OtherValue = !String.IsNullOrEmpty(dt.Rows[i][40].ToString()) ? Convert.ToDecimal(dt.Rows[i][40].ToString()) : 0,
                                OtherAmount = !String.IsNullOrEmpty(dt.Rows[i][41].ToString()) ? Convert.ToDecimal(dt.Rows[i][41].ToString()) : 0,
                                UserId = userId,
                            };
                            salaryPerks.Add(salaryDetail);
                        }
                    }
                }
                return salaryPerks;
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}

using Aspose.Words;
using Autofac.Features.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Office.Interop.Word;
using Org.BouncyCastle.Utilities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;


namespace TaxApp.BAL.Services
{
    public class EnumServices : IEnumService
    {
        public string GetCategoryType(string value)
        {
            var data = "";
            switch (value)
            {
                case "Central Government":
                    data = "A";
                    break;
                case "State Government":
                    data = "S";
                    break;
                case "Statutory body (Central Govt.)":
                    data = "D";
                    break;
                case "Statutory body (State Govt.)":
                    data = "E";
                    break;
                case "Autonomous body (Central Govt.)":
                    data = "G";
                    break;
                case "Autonomous body (State Govt.)":
                    data = "H";
                    break;
                case "Local Authority (Central Govt.)":
                    data = "L";
                    break;
                case "Local Authority (State Govt.)":
                    data = "N";
                    break;
                case "Company":
                    data = "K";
                    break;
                case "Branch / Division of Company":
                    data = "M";
                    break;
                case "Association of Person (AOP)":
                    data = "P";
                    break;
                case "Association of Person (Trust)":
                    data = "T";
                    break;
                case "Artificial Juridical Person":
                    data = "J";
                    break;
                case "Body of Individuals":
                    data = "B";
                    break;
                case "Individual/HUF":
                    data = "Q";
                    break;
                case "Firm":
                    data = "F";
                    break;
            }
            return data;
        }
        public string GetSectionCode(string value)
        {
            var data = "";
            switch (value)
            {
                case "195-Payment to Non-residents":
                    data = "195";
                    break;
                case "194E-Non-resident Sportsman or Sports Assn.":
                    data = "94E";
                    break;
                case "196A-Foreign Company being unit holder in Mutual Fund":
                    data = "96A";
                    break;
                case "196B-Units held by Off-shore Fund":
                    data = "96B";
                    break;
                case "196C-Income from Foreign Currency Bonds":
                    data = "96C";
                    break;
                case "196D-Income of FII on Securities":
                    data = "96D";
                    break;
                case "194LB-Interest from infrastr. debt fund payable to non resident":
                    data = "4LB";
                    break;
                case "194LC(2)(i)/(ia)-Income under section 194LC(2)(i) and (ia)":
                    data = "LC1";
                    break;
                case "194LC(2)(ib)-Income under section 194LC(2)(ib)":
                    data = "LC2";
                    break;
                case "194LC(2)(ib)-Income under section 194LC(2)(ic)":
                    data = "LC3";
                    break;
                case "194LD-Interest on certain bonds and govt securities":
                    data = "4LD";
                    break;
                case "192A-Payment against EPF Scheme":
                    data = "2AA";
                    break;
                case "194LBA-Certain income from units of a business trust":
                    data = "4BA";
                    break;
                case "194LBB-Income on units of Investment Fund":
                    data = "LBB";
                    break;
                case "194LBC-Income in respect of investment in securitization trust":
                    data = "LBC";
                    break;
                case "194N-Payment of certain amounts in cash":
                    data = "94N";
                    break;
                case "194LBA(a)-Income referred to in section 10(23FC)(a) from units of a business trust":
                    data = "BA1";
                    break;
                case "194LBA(b)-Income referred to in section 10(23FC)(b) from units of a business trust":
                    data = "BA2";
                    break;
                case "194LBA(c)-Income referred to in section 10(23FCA) from units of a business trust":
                    data = "BA3";
                    break;
                case "194NF-Payment of certain amounts in cash to non-filers":
                    data = "4NF";
                    break;
                case "196D(1A)- Income of specified fund from securities referred to in section 115AD(1)(a)":
                    data = "6DA";
                    break;
                case "194B-Winnings from lottery or crossword puzzle, etc":
                    data = "94B";
                    break;
                case "194B-P-Winnings from  lottery or crossword puzzle, etc where consideration is made in kind or cash is not sufficient":
                    data = "4BP";
                    break;
                case "194BA-Winnings from online games":
                    data = "9BA";
                    break;
                case "194BA-P- Net Winnings from online games where the net winnings are made in kind or cash is not sufficient":
                    data = "4AP";
                    break;
                case "194BB-Winnings from horse race":
                    data = "4BB";
                    break;
                case "194NC-Payment of certain amounts in cash to co-operative societies not covered by first proviso":
                    data = "4NC";
                    break;
                case "194N-FT-Payment of certain amount in cash to non-filers being co-operative societies":
                    data = "9FT";
                    break;
                case "206CA- Alcholic liquor for human consumption & Tendu Leaves":
                    data = "A";
                    break;
                case "206CB- Timber optained under a forest lease":
                    data = "B";
                    break;
                case "206CC- Timber obtained under mode other than forest lease":
                    data = "C";
                    break;
                case "206CD- Any other forest product not being timber or tendu leave":
                    data = "D";
                    break;
                case "206CE- Scrap":
                    data = "E";
                    break;
                case "206CF- Parking Lot":
                    data = "F";
                    break;
                case "206CG- Toll Plaza":
                    data = "G";
                    break;
                case "206CH- Mining and quarring":
                    data = "H";
                    break;
                case "206CI- Tendu leaves":
                    data = "I";
                    break;
                case "206CJ- Minerals":
                    data = "J";
                    break;
                case "206CK- Bullion and Jewellery":
                    data = "K";
                    break;
                case "206CL- Sale of Motor vehicle":
                    data = "L";
                    break;
                case "206CM- Sale in cash of any goods (other than bullion/jewellery)":
                    data = "M";
                    break;
                case "206CN- Providing of any services (other than Ch-XVII-B)":
                    data = "N";
                    break;
                case "206CO- LRS - Overseas Tour Program Package":
                    data = "O";
                    break;
                case "206CP- LRS – Educational - Loan from Financial Institution":
                    data = "P";
                    break;
                case "206CQ- LRS – Other Purposes":
                    data = "Q";
                    break;
                case "206CR- Sale of Goods":
                    data = "R";
                    break;
                case "206CP- LRS – Education/Medical - Non Financial Institution":
                    data = "T";
                    break;
            }
            return data;
        }

        public int GetMinorCode(string value)
        {
            var data = 0;
            switch (value)
            {
                case "200-Payable by taxpayer":
                    data = 200;
                    break;
                case "400-Regular Assessment":
                    data = 400;
                    break;
                case "100-Advance Tax":
                    data = 100;
                    break;
            }
            return data;
        }
        public string GetDeducteeCode(string value)
        {
            var data = "0";
            switch (value)
            {
                case "01-Company":
                    data = "01";
                    break;
                case "02-Individual":
                    data = "02";
                    break;
                case "03-Hindu Undivided Family":
                    data = "03";
                    break;
                case "05-AOP Only Company Member":
                    data = "04";
                    break;
                case "04-AOP with No Company Member":
                    data = "05";
                    break;
                case "06-Co-operative Society":
                    data = "06";
                    break;
                case "07-Firm":
                    data = "07";
                    break;
                case "08-Body of individuals":
                    data = "08";
                    break;
                case "09-Artificial juridical person":
                    data = "09";
                    break;
                case "10-Others":
                    data = "10";
                    break;
            }
            return data;
        }
        public string GetReasonsCode(string value)
        {
            var data = "";
            switch (value)
            {
                case "A-Lower Deduction/ No deduction u/s 197":
                    data = "A";
                    break;
                case "B-No deduction u/s 197A":
                    data = "B";
                    break;
                case "C-Higher Rate (Valid PAN not available)":
                    data = "C";
                    break;
                case "S-Software Providers (Notification 21/2012)":
                    data = "N";
                    break;
                case "N-No deduction-clause(iii, iv or v)-Section 194N":
                    data = "O";
                    break;
                case "O-No deduction as per provisions of sub-section(2A) of section 194LBA":
                    data = "P";
                    break;
                case "P-No deduction-Notification-Section 197A(1F)":
                    data = "P";
                    break;
                case "M-No deduction/lower deduction on account of notification issued under second provison to section 194N":
                    data = "M";
                    break;
                case "G-No deduction is in view of clause(a) or clause(b) of sub-section(1D) of section 197A":
                    data = "G";
                    break;
                case "I-No deduction is in view of sub-section (2) of section 196D":
                    data = "I";
                    break;
                case "H-No deduction is in view of sub-section (1A) of section 196D":
                    data = "H";
                    break;
                case "J-Higer rate in case of non-filer u/s 206AB":
                    data = "J";
                    break;
                case "Y-Within threshold limit as per Income tax Act":
                    data = "Y";
                    break;
                case "A- Lower collection u/s 206C (9)":
                    data = "A";
                    break;
                case "B- Non collection u/s 206C (1A)":
                    data = "B";
                    break;
                case "C- Higher Rate (Valid PAN not available)":
                    data = "B";
                    break;
                case "D- Remittance is less than Rs. 7 lacs/ Collection Code 206CP":
                    data = "D";
                    break;
                case "E- TCS already collected":
                    data = "E";
                    break;
                case "F- TDS by Buyer / Sale to Govt. & others as specified":
                    data = "F";
                    break;
                case "G- TDS by Buyer on transaction":
                    data = "G";
                    break;
                case "H- Sale to Govt. & others as specified":
                    data = "H";
                    break;
                case "I- Higher rate in view of section 206CCA":
                    data = "I";
                    break;
            }
            return data;
        }

        public int GetNatureCode(string value)
        {
            var data = 0;
            switch (value)
            {
                case "Dividend":
                    data = 16;
                    break;
                case "Fees For Technical Services/ Fees For Included Services":
                    data = 21;
                    break;
                case "Interest Payment":
                    data = 27;
                    break;
                case "Investment Income":
                    data = 28;
                    break;
                case "Long Term Capital Gains (Others)":
                    data = 31;
                    break;
                case "Long Term Capital Gain U/S 115E in case of Non Resident Indian Citizen":
                    data = 66;
                    break;
                case "Long Term Capital Gain U/S 112(1)(C)(iii)":
                    data = 67;
                    break;
                case "Long Term Capital Gain U/S 112":
                    data = 68;
                    break;
                case "Long Term Capital Gain U/S 112A":
                    data = 69;
                    break;
                case "Royalty":
                    data = 49;
                    break;
                case "Short Term Capital Gains":
                    data = 52;
                    break;
                case "Short Term Capital Gains U/S 111A":
                    data = 70;
                    break;
                case "Other Income / Other (Not In The Nature Of Income)":
                    data = 99;
                    break;
            }
            return data;
        }

        public string GetTDSRateCode(string value)
        {
            var data = "";
            switch (value)
            {
                case "As per Income Tax Act":
                    data = "A";
                    break;
                case "As per DTAA":
                    data = "B";
                    break;
            }
            return data;
        }
        public int GetState(string value)
        {
            var data = 0;
            switch (value)
            {
                case "ANDAMAN AND NICOBAR ISLANDS":
                    data = 1;
                    break;
                case "ANDHRA PRADESH":
                    data = 2;
                    break;
                case "ARUNACHAL PRADESH":
                    data = 3;
                    break;
                case "ASSAM":
                    data = 4;
                    break;
                case "BIHAR":
                    data = 5;
                    break;
                case "CHANDIGARH":
                    data = 6;
                    break;
                case "DADRA & NAGAR HAVELI AND DAMAN & DIU":
                    data = 7;
                    break;
                case "DELHI":
                    data = 9;
                    break;
                case "GOA":
                    data = 10;
                    break;
                case "GUJARAT":
                    data = 11;
                    break;
                case "HARYANA":
                    data = 12;
                    break;
                case "HIMACHAL PRADESH":
                    data = 13;
                    break;
                case "JAMMU & KASHMIR":
                    data = 14;
                    break;
                case "KARNATAKA":
                    data = 15;
                    break;
                case "KERALA":
                    data = 16;
                    break;
                case "LAKSHWADEEP":
                    data = 17;
                    break;
                case "MADHYA PRADESH":
                    data = 18;
                    break;
                case "MAHARASHTRA":
                    data = 19;
                    break;
                case "MANIPUR":
                    data = 20;
                    break;
                case "MEGHALAYA":
                    data = 21;
                    break;
                case "MIZORAM":
                    data = 22;
                    break;
                case "NAGALAND":
                    data = 23;
                    break;
                case "ODISHA":
                    data = 24;
                    break;
                case "PONDICHERRY":
                    data = 25;
                    break;
                case "PUNJAB":
                    data = 26;
                    break;
                case "RAJASTHAN":
                    data = 27;
                    break;
                case "SIKKIM":
                    data = 28;
                    break;
                case "TAMIL NADU":
                    data = 29;
                    break;
                case "TRIPURA":
                    data = 30;
                    break;
                case "UTTAR PRADESH":
                    data = 31;
                    break;
                case "WEST BENGAL":
                    data = 32;
                    break;
                case "CHHATTISGARH":
                    data = 33;
                    break;
                case "UTTARAKHAND":
                    data = 34;
                    break;
                case "JHARKHAND":
                    data = 35;
                    break;
                case "TELANGANA":
                    data = 36;
                    break;
                case "LADAKH":
                    data = 37;
                    break;
            }
            return data;
        }
        public int GetCountryCode(string value)
        {
            var data = 0;
            switch (value)
            {
                case "AFGHANISTAN":
                    data = 01;
                    break;
                case "AKROTIRI":
                    data = 01;
                    break;
                case "ALBANIA":
                    data = 03;
                    break;
                case "ALGERIA":
                    data = 04;
                    break;
                case "AMERICAN SAMOA":
                    data = 05;
                    break;
                case "ANDORRA":
                    data = 06;
                    break;
                case "ANGOLA":
                    data = 07;
                    break;
                case "ANGUILLA":
                    data = 08;
                    break;
                case "ANTARCTICA":
                    data = 01;
                    break;
                case "ANTIGUA AND BARBUDA":
                    data = 03;
                    break;
                case "ARGENTINA":
                    data = 04;
                    break;
                case "ARMENIA":
                    data = 05;
                    break;
                case "ARUBA":
                    data = 06;
                    break;
                case "ASHMORE AND CARTIER ISLANDS":
                    data = 07;
                    break;
            }
            return data;

        }
        public int GetMinistry(string value)
        {
            var data = 0;
            switch (value)
            {
                case "Agriculture":
                    data = 1;
                    break;
                case "Atomic Energy":
                    data = 2;
                    break;
                case "Fertilizers":
                    data = 3;
                    break;
                case "Chemicals and Petrochemicals":
                    data = 4;
                    break;
                case "Civil Aviation and Tourism":
                    data = 05;
                    break;
                case "Coal":
                    data = 6;
                    break;
                case "Consumer Affairs, Food and Public Distribution":
                    data = 7;
                    break;
                case "Commerce and Textiles":
                    data = 8;
                    break;
                case "Environment and Forests and Ministry of Earth Science":
                    data = 9;
                    break;
                case "External Affairs and Overseas Indian Affairs":
                    data = 10;
                    break;
                case "Finance":
                    data = 11;
                    break;
                case "Central Board of Direct Taxes":
                    data = 12;
                    break;
                case "Central Board of Excise and Customs":
                    data = 13;
                    break;
                case "Contoller of Aid Accounts and Audit":
                    data = 14;
                    break;
                case "Central Pension Accounting Office":
                    data = 15;
                    break;
                case "Food Processing Industries":
                    data = 16;
                    break;
                case "Health and Family Welfare":
                    data = 17;
                    break;
                case "Home Affairs and Development of North Eastern Region":
                    data = 18;
                    break;
                case "Human Resource Development":
                    data = 19;
                    break;
                case "Industry":
                    data = 20;
                    break;
                case "Information and Broadcasting":
                    data = 21;
                    break;
                case "Telecommunication and Information Technology":
                    data = 22;
                    break;
                case "Labour":
                    data = 23;
                    break;
                case "Law and Justice and Company Affairs":
                    data = 24;
                    break;
                case "Personnel, Public Grievances and Pesions":
                    data = 25;
                    break;
                case "Petroleum and Natural Gas":
                    data = 26;
                    break;
                case "Plannning, Statistics and Programme Implementation":
                    data = 27;
                    break;
                case "Power":
                    data = 28;
                    break;
                case "New and Renewable Energy":
                    data = 29;
                    break;
                case "Rural Development and Panchayati Raj":
                    data = 30;
                    break;
                case "Science And Technology":
                    data = 31;
                    break;
                case "Space":
                    data = 32;
                    break;
                case "Steel":
                    data = 33;
                    break;
                case "Mines":
                    data = 34;
                    break;
                case "Social Justice and Empowerment":
                    data = 35;
                    break;
                case "Tribal Affairs":
                    data = 36;
                    break;
                case "D/o Commerce (Supply Division)":
                    data = 37;
                    break;
                case "Shipping and Road Transport and Highways":
                    data = 38;
                    break;
                case "Urban Development, Urban Employment and Poverty Alleviation":
                    data = 39;
                    break;
                case "Water Resources":
                    data = 40;
                    break;
                case "President's Secretariat":
                    data = 41;
                    break;
                case "Lok Sabha Secretariat":
                    data = 42;
                    break;
                case "Rajya Sabha secretariat":
                    data = 43;
                    break;
                case "Election Commission":
                    data = 44;
                    break;
                case "Ministry of Defence (Controller General of Defence Accounts)":
                    data = 45;
                    break;
                case "Ministry of Railways":
                    data = 46;
                    break;
                case "Department of Posts":
                    data = 47;
                    break;
                case "Department of Telecommunications":
                    data = 48;
                    break;
                case "Andaman and Nicobar Islands Administration":
                    data = 49;
                    break;
                case "Chandigarh Administration":
                    data = 50;
                    break;
                case "Dadra and Nagar Haveli":
                    data = 51;
                    break;
                case "Goa, Daman and Diu":
                    data = 52;
                    break;
                case "Lakshadweep":
                    data = 53;
                    break;
                case "Pondicherry Administration":
                    data = 54;
                    break;
                case "Pay and Accounts Officers (Audit)":
                    data = 55;
                    break;
                case "Ministry of Non-conventional energy sources":
                    data = 56;
                    break;
                case "Government Of NCT of Delhi":
                    data = 57;
                    break;
                case "Others":
                    data = 99;
                    break;
            }
            return data;
        }
    }
}

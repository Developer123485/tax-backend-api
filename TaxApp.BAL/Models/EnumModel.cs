using System.ComponentModel;
using System.Runtime.Serialization;

namespace TaxApp.BAL.Models
{
    public class EnumModel
    {
        public enum CountryCode
        {
            [EnumMember(Value = "01")]
            [Description("AFGHANISTAN")]
            AFGHANISTAN,
            [EnumMember(Value = "02")]
            [Description("AKROTIRI")]
            AKROTIRI,
            [EnumMember(Value = "03")]
            [Description("ALBANIA")]
            ALBANIA,
            [EnumMember(Value = "04")]
            [Description("ALGERIA")]
            ALGERIA,
            [Description("AMERICAN SAMOA")]
            [EnumMember(Value = "05")]
            AMERICANSAMOA,
            [EnumMember(Value = "06")]
            [Description("ANDORRA")]
            ANDORRA,
            [EnumMember(Value = "07")]
            [Description("ANGOLA")]
            ANGOLA,
            [EnumMember(Value = "08")]
            [Description("ANGUILLA")]
            ANGUILLA,
            [EnumMember(Value = "09")]
            [Description("ANTARCTICA")]
            ANTARCTICA,
            [Description("ANTIGUA AND BARBUDA")]
            [EnumMember(Value = "10")]
            ANTIGUAANDBARBUDA,
            [EnumMember(Value = "11")]
            [Description("ARGENTINA")]
            ARGENTINA,
            [EnumMember(Value = "12")]
            [Description("ARMENIA")]
            ARMENIA,
            [EnumMember(Value = "13")]
            [Description("ARUBA")]
            ARUBA,
            [Description("ASHMORE AND CARTIER ISLANDS")]
            [EnumMember(Value = "14")]
            ASHMOREANDCARTIERISLANDS,
            [EnumMember(Value = "15")]
            [Description("AUSTRALIA")]
            AUSTRALIA,
            [EnumMember(Value = "16")]
            [Description("AUSTRIA")]
            AUSTRIA,
            [EnumMember(Value = "17")]
            [Description("AZERBAIJAN")]
            AZERBAIJAN,
            [EnumMember(Value = "18")]
            [Description("BAHRAIN")]
            BAHRAIN,
            [Description("BAILIWICK OF GUERNSEY")]
            [EnumMember(Value = "19")]
            BAILIWICKOFGUERNSEY,
            [Description("BAILIWICK OF JERSEY")]
            [EnumMember(Value = "20")]
            BAILIWICKOFJERSEY = 20,
            [Description("BAKER ISLAND")]
            [EnumMember(Value = "21")]
            BAKERISLAND = 21,
            [EnumMember(Value = "22")]
            [Description("BANGLADESH")]
            BANGLADESH,
            [EnumMember(Value = "23")]
            [Description("BARBADOS")]
            BARBADOS,
            [EnumMember(Value = "24")]
            [Description("BELARUS")]
            BELARUS,
            [EnumMember(Value = "25")]
            [Description("BELGIUM")]
            BELGIUM = 25,
            [EnumMember(Value = "26")]
            [Description("BELIZE")]
            BELIZE = 26,
            [Description("BENIN PORTO")]
            [EnumMember(Value = "27")]
            BENINPORTO = 27,
            [EnumMember(Value = "28")]
            [Description("BERMUDA")]
            BERMUDA = 28,
            [EnumMember(Value = "29")]
            [Description("BHUTAN")]
            BHUTAN = 29,
            [EnumMember(Value = "30")]
            [Description("BOLIVIA")]
            BOLIVIA = 30,
            [Description("BOSNIAAND HERZEGOVINA")]
            [EnumMember(Value = "31")]
            BOSNIAANDHERZEGOVINA = 31,
            [EnumMember(Value = "32")]
            [Description("BOTSWANA")]
            BOTSWANA = 32,
            [Description("BOUVET ISLAND")]
            [EnumMember(Value = "33")]
            BOUVETISLAND = 33,
            [EnumMember(Value = "34")]
            [Description("BRAZIL")]
            BRAZIL = 34,
            [Description("BRITISH INDIAN OCEAN TERRITORY")]
            [EnumMember(Value = "35")]
            BRITISHINDIANOCEANTERRITORY = 35,
            [EnumMember(Value = "36")]
            [Description("BRUNEI")]
            BRUNEI = 36,
            [EnumMember(Value = "37")]
            [Description("BULGARIA")]
            BULGARIA = 37,
            [Description("BURKINA FASO")]
            [EnumMember(Value = "38")]
            BURKINAFASO = 38,
            [EnumMember(Value = "39")]
            [Description("BURUNDI")]
            BURUNDI = 39,
            [EnumMember(Value = "40")]
            [Description("CAMBODIA")]
            CAMBODIA = 40,
            [EnumMember(Value = "41")]
            [Description("CAMEROON")]
            CAMEROON = 41,
            [EnumMember(Value = "42")]
            [Description("CANADA")]
            CANADA = 42,
            [Description("CAPE VERDE")]
            [EnumMember(Value = "43")]
            CAPEVERDE = 43,
            [Description("CAYMAN ISLANDS")]
            [EnumMember(Value = "44")]
            CAYMANISLANDS = 44,
            [Description("CENTRAL AFRICAN REPUBLIC")]
            [EnumMember(Value = "45")]
            CENTRALAFRICANREPUBLIC = 45,
            [EnumMember(Value = "46")]
            [Description("CHAD")]
            CHAD = 46,
            [EnumMember(Value = "47")]
            [Description("CHILE")]
            CHILE = 47,
            [EnumMember(Value = "48")]
            [Description("CHINA")]
            CHINA = 48,
            [Description("CHRISTMAS ISLAND")]
            [EnumMember(Value = "49")]
            CHRISTMASISLAND = 49,
            [Description("CLIPPERTON ISLAND")]
            [EnumMember(Value = "50")]
            CLIPPERTONISLAND = 50,
            [Description("COCOS (KEELING) ISLANDS")]
            [EnumMember(Value = "51")]
            COCOSKEELINGISLANDS = 51,
            [EnumMember(Value = "52")]
            [Description("COLOMBIA")]
            COLOMBIA = 52,
            [Description("COMMONWEALTH OF PUERTO RICO")]
            [EnumMember(Value = "53")]
            COMMONWEALTHOFPUERTORICO = 53,
            [Description("COMMONWEALTH OF THE NORTHERN MARIANA ISLANDS")]
            [EnumMember(Value = "54")]
            COMMONWEALTHOFTHENORTHERNMARIANAISLANDS = 54,
            [Description("COMOROS")]
            [EnumMember(Value = "55")]
            COMOROS = 55,
            [Description("CONGO,DEMOCRATIC REPUBLIC OF THE")]
            [EnumMember(Value = "56")]
            CONGODEMOCRATICREPUBLICOFTHE = 56,
            [Description("CONGO,REPUBLIC OF THE")]
            [EnumMember(Value = "57")]
            CONGOREPUBLICOFTHE = 57,
            [Description("COOK ISLANDS")]
            [EnumMember(Value = "58")]
            COOKISLANDS = 58,
            [Description("CORAL SEA ISLANDS")]
            [EnumMember(Value = "59")]
            CORALSEAISLANDS = 59,
            [Description("CORAL SEA ISLANDS TERRITORY")]
            [EnumMember(Value = "60")]
            CORALSEAISLANDSTERRITORY = 60,
            [Description("COSTA RICA")]
            [EnumMember(Value = "61")]
            COSTARICA = 61,
            [Description("COTE D'IVOIRE")]
            [EnumMember(Value = "62")]
            COTEDIVOIRE = 62,
            [EnumMember(Value = "63")]
            [Description("CROATIA")]
            CROATIA = 63,
            [EnumMember(Value = "64")]
            [Description("CUBA")]
            CUBA = 64,
            [EnumMember(Value = "65")]
            [Description("CYPRUS")]
            CYPRUS = 65,
            [Description("CZECH REPUBLIC")]
            [EnumMember(Value = "66")]
            CZECHREPUBLIC = 66,
            [Description("DENMARK")]
            [EnumMember(Value = "67")]
            DENMARK = 67,
            [Description("DEPARTMENTAL COLLECTIVITY OF MAYOTTE")]
            [EnumMember(Value = "68")]
            DEPARTMENTALCOLLECTIVITYOFMAYOTTE = 68,
            [EnumMember(Value = "69")]
            [Description("DHEKELIA")]
            DHEKELIA = 69,
            [EnumMember(Value = "70")]
            [Description("DJIBOUTI")]
            DJIBOUTI = 70,
            [EnumMember(Value = "71")]
            [Description("DOMINICA")]
            DOMINICA = 71,
            [Description("DOMINICAN REPUBLIC")]
            [EnumMember(Value = "72")]
            DOMINICANREPUBLIC = 72,
            [Description("EAST TIMOR (TIMORLASTE)")]
            [EnumMember(Value = "73")]
            EASTTIMORTIMORLASTE = 73,
            [Description("ECUADOR")]
            [EnumMember(Value = "74")]
            ECUADOR = 74,
            [Description("EGYPT")]
            [EnumMember(Value = "75")]
            EGYPT = 75,
            [Description("ELSALVADOR")]
            [EnumMember(Value = "76")]
            ELSALVADOR = 76,
            [Description("EQUATORIAL GUINEA")]
            [EnumMember(Value = "77")]
            EQUATORIALGUINEA = 77,
            [Description("ERITREA")]
            [EnumMember(Value = "78")]
            ERITREA = 78,
            [Description("ESTONIA")]
            [EnumMember(Value = "79")]
            ESTONIA = 79,
            [Description("ETHIOPIA")]
            [EnumMember(Value = "80")]
            ETHIOPIA = 80,
            [Description("FALKLAND ISLANDS (ISLAS MALVINAS)")]
            [EnumMember(Value = "81")]
            FALKLANDISLANDSISLASMALVINAS = 81,
            [Description("FAROE ISLANDS")]
            [EnumMember(Value = "82")]
            FAROEISLANDS = 82,
            [Description("FIJI")]
            [EnumMember(Value = "83")]
            FIJI = 83,
            [Description("FINLAND")]
            [EnumMember(Value = "84")]
            FINLAND = 84,
            [Description("FRANCE")]
            [EnumMember(Value = "85")]
            FRANCE = 85,
            [Description("FRENCH GUIANA")]
            [EnumMember(Value = "86")]
            FRENCHGUIANA = 86,
            [Description("FRENCH POLYNESIA")]
            [EnumMember(Value = "87")]
            FRENCHPOLYNESIA = 87,
            [Description("FRENCH SOUTHERN ISLANDS")]
            [EnumMember(Value = "88")]
            FRENCHSOUTHERNISLANDS = 88,
            [Description("GABON")]
            [EnumMember(Value = "89")]
            GABON = 89,
            [Description("GEORGIA")]
            [EnumMember(Value = "90")]
            GEORGIA = 90,
            [Description("GERMANY")]
            [EnumMember(Value = "91")]
            GERMANY = 91,
            [Description("GEURNSEY")]
            [EnumMember(Value = "92")]
            GEURNSEY = 92,
            [Description("GHANA")]
            [EnumMember(Value = "93")]
            GHANA = 93,
            [Description("GIBRALTAR")]
            [EnumMember(Value = "94")]
            GIBRALTAR = 94,
            [Description("GREECE")]
            [EnumMember(Value = "95")]
            GREECE = 95,
            [Description("GREENLAND")]
            [EnumMember(Value = "96")]
            GREENLAND = 96,
            [Description("GRENADA")]
            [EnumMember(Value = "97")]
            GRENADA = 97,
            [Description("GUADELOUPE")]
            [EnumMember(Value = "98")]
            GUADELOUPE = 98,
            [Description("GUAM")]
            [EnumMember(Value = "100")]
            GUAM = 100,
            [Description("GUATEMALA")]
            [EnumMember(Value = "101")]
            GUATEMALA = 101,
            [Description("GUERNSEY")]
            [EnumMember(Value = "102")]
            GUERNSEY = 102,
            [Description("GUINEA")]
            [EnumMember(Value = "103")]
            GUINEA = 103,
            [Description("GUINEABISSAU")]
            [EnumMember(Value = "104")]
            GUINEABISSAU = 104,
            [Description("GUYANA")]
            [EnumMember(Value = "105")]
            GUYANA = 105,
            [Description("HAITI")]
            [EnumMember(Value = "106")]
            HAITI = 106,
            [Description("HEARD ISLAND AND MCDONALD ISLANDS")]
            [EnumMember(Value = "107")]
            HEARDISLANDANDMCDONALDISLANDS = 107,
            [Description("HONDURAS")]
            [EnumMember(Value = "108")]
            HONDURAS = 108,
            [Description("HONG KONG")]
            [EnumMember(Value = "109")]
            HONGKONG = 109,
            [Description("HOWLAND ISLAND")]
            [EnumMember(Value = "110")]
            HOWLANDISLAND = 110,
            [Description("HUNGARY")]
            [EnumMember(Value = "111")]
            HUNGARY = 111,
            [Description("ICELAND")]
            [EnumMember(Value = "112")]
            ICELAND = 112,
            [Description("INDIA")]
            [EnumMember(Value = "113")]
            INDIA = 113,
            [Description("INDONESIA")]
            [EnumMember(Value = "114")]
            INDONESIA = 114,
            [Description("IRAN")]
            [EnumMember(Value = "115")]
            IRAN = 115,
            [Description("IRAQ")]
            [EnumMember(Value = "116")]
            IRAQ = 116,
            [Description("IRELAND")]
            [EnumMember(Value = "117")]
            IRELAND = 117,
            [Description("ISLE OF MAN")]
            [EnumMember(Value = "118")]
            ISLEOFMAN = 118,
            [Description("ISRAEL")]
            [EnumMember(Value = "119")]
            ISRAEL = 119,
            [Description("ITALY")]
            [EnumMember(Value = "120")]
            ITALY = 120,
            [Description("JAMAICA")]
            [EnumMember(Value = "121")]
            JAMAICA = 121,
            [Description("JAN MAYEN")]
            [EnumMember(Value = "122")]
            JANMAYEN = 122,
            [Description("JAPAN")]
            [EnumMember(Value = "123")]
            JAPAN = 123,
            [Description("JARVIS ISLAND")]
            [EnumMember(Value = "124")]
            JARVISISLAND = 124,
            [Description("JERSEY")]
            [EnumMember(Value = "125")]
            JERSEY = 125,
            [Description("JOHNSTON ATOLL")]
            [EnumMember(Value = "126")]
            JOHNSTONATOLL = 126,
            [Description("JORDAN")]
            [EnumMember(Value = "127")]
            JORDAN = 127,
            [Description("KAZAKHSTAN")]
            [EnumMember(Value = "128")]
            KAZAKHSTAN = 128,
            [Description("KENYA")]
            [EnumMember(Value = "129")]
            KENYA = 129,
            [Description("KINGMAN REEF")]
            [EnumMember(Value = "130")]
            KINGMANREEF = 130,
            [Description("KIRIBATI")]
            [EnumMember(Value = "131")]
            KIRIBATI = 131,
            [Description("KOREA, NORTH")]
            [EnumMember(Value = "132")]
            KOREA = 132,
            [Description("KOREA, SOUTH")]
            [EnumMember(Value = "133")]
            KOREASOUTH = 133,
            [Description("KOSOVO")]
            [EnumMember(Value = "134")]
            KOSOVO = 134,
            [Description("KUWAIT")]
            [EnumMember(Value = "135")]
            KUWAIT = 135,
            [Description("KYRGYZSTAN")]
            [EnumMember(Value = "136")]
            KYRGYZSTAN = 136,
            [Description("LAOS")]
            [EnumMember(Value = "137")]
            LAOS = 137,
            [Description("LATVIA")]
            [EnumMember(Value = "138")]
            LATVIA = 138,
            [Description("LEBANON")]
            [EnumMember(Value = "139")]
            LEBANON = 139,
            [Description("LESOTHO")]
            [EnumMember(Value = "140")]
            LESOTHO = 140,
            [Description("LIBERIA")]
            [EnumMember(Value = "141")]
            LIBERIA = 141,
            [Description("LIBYA")]
            [EnumMember(Value = "142")]
            LIBYA = 142,
            [Description("LIECHTENSTEIN")]
            [EnumMember(Value = "143")]
            LIECHTENSTEIN = 143,
            [Description("LITHUANIA")]
            [EnumMember(Value = "144")]
            LITHUANIA = 144,
            [Description("LUXEMBOURG")]
            [EnumMember(Value = "145")]
            LUXEMBOURG = 145,
            [Description("MACAU")]
            [EnumMember(Value = "146")]
            MACAU = 146,
            [Description("MACEDONIA")]
            [EnumMember(Value = "147")]
            MACEDONIA = 147,
            [Description("MADAGASCAR")]
            [EnumMember(Value = "148")]
            MADAGASCAR = 148,
            [Description("MALAWI")]
            [EnumMember(Value = "149")]
            MALAWI = 149,
            [Description("MALAYSIA")]
            [EnumMember(Value = "150")]
            MALAYSIA = 150,
            [Description("MALAYSIA (LABUAN)")]
            [EnumMember(Value = "151")]
            MALAYSIALABUAN = 151,
            [Description("MALDIVES")]
            [EnumMember(Value = "152")]
            MALDIVES = 152,
            [Description("MALI")]
            [EnumMember(Value = "153")]
            MALI = 153,
            [Description("MALTA")]
            [EnumMember(Value = "154")]
            MALTA = 154,
            [Description("MARSHALLISLANDS")]
            [EnumMember(Value = "155")]
            MARSHALLISLANDS = 155,
            [Description("MARTINIQUE")]
            [EnumMember(Value = "156")]
            MARTINIQUE = 156,
            [Description("MAURITANIA")]
            [EnumMember(Value = "157")]
            MAURITANIA = 157,
            [Description("MAURITIUS")]
            [EnumMember(Value = "158")]
            MAURITIUS = 158,
            [Description("MAYOTTE")]
            [EnumMember(Value = "159")]
            MAYOTTE = 159,
            [Description("UNITED MEXICAN STATES")]
            [EnumMember(Value = "160")]
            UNITEDMEXICANSTATES = 160,
            [Description("MICRONESIA, FEDERATED STATES OF")]
            [EnumMember(Value = "161")]
            MICRONESIAFEDERATEDSTATESOF = 161,
            [Description("MIDWAY ISLANDS")]
            [EnumMember(Value = "162")]
            MIDWAYISLANDS = 162,
            [Description("MOLDOVA")]
            [EnumMember(Value = "163")]
            MOLDOVA = 163,
            [Description("MONACO")]
            [EnumMember(Value = "164")]
            MONACO = 164,
            [Description("MONGOLIA")]
            [EnumMember(Value = "165")]
            MONGOLIA = 165,
            [Description("MONTENEGRO")]
            [EnumMember(Value = "166")]
            MONTENEGRO = 166,
            [Description("MONTSERRAT")]
            [EnumMember(Value = "167")]
            MONTSERRAT = 167,
            [Description("MOROCCO")]
            [EnumMember(Value = "168")]
            MOROCCO = 168,
            [Description("MOZAMBIQUE")]
            [EnumMember(Value = "169")]
            MOZAMBIQUE = 169,
            [Description("MYANMAR (BURMA)")]
            [EnumMember(Value = "170")]
            MYANMARBURMA = 170,
            [Description("NAMIBIA")]
            [EnumMember(Value = "171")]
            NAMIBIA = 171,
            [Description("NAURU")]
            [EnumMember(Value = "172")]
            NAURU = 172,
            [Description("NAVASSA ISLAND")]
            [EnumMember(Value = "173")]
            NAVASSAISLAND = 173,
            [Description("NEPAL")]
            [EnumMember(Value = "174")]
            NEPAL = 174,
            [Description("NETHERLANDS")]
            [EnumMember(Value = "175")]
            NETHERLANDS = 175,
            [Description("NETHERLANDS ANTILLES")]
            [EnumMember(Value = "176")]
            NETHERLANDSANTILLES = 176,
            [Description("NEW CALEDONIA")]
            [EnumMember(Value = "177")]
            NEWCALEDONIA = 177,
            [Description("NEWZEALAND")]
            [EnumMember(Value = "178")]
            NEWZEALAND = 178,
            [Description("NICARAGUA")]
            [EnumMember(Value = "179")]
            NICARAGUA = 179,
            [Description("NIGER")]
            [EnumMember(Value = "180")]
            NIGER = 180,
            [Description("NIGERIA")]
            [EnumMember(Value = "181")]
            NIGERIA = 181,
            [Description("NIUE")]
            [EnumMember(Value = "182")]
            NIUE = 182,
            [Description("NORFOLK ISLAND")]
            [EnumMember(Value = "183")]
            NORFOLKISLAND = 183,
            [Description("NORTHERN MARIANA ISLANDS")]
            [EnumMember(Value = "184")]
            NORTHERNMARIANAISLANDS = 184,
            [Description("NORWAY")]
            [EnumMember(Value = "185")]
            NORWAY = 185,
            [Description("OMAN")]
            [EnumMember(Value = "186")]
            OMAN = 186,
            [Description("PAKISTAN")]
            [EnumMember(Value = "187")]
            PAKISTAN = 187,
            [Description("PALAU")]
            [EnumMember(Value = "188")]
            PALAU = 188,
            [Description("PALMYRA ATOLL")]
            [EnumMember(Value = "189")]
            PALMYRAATOLL = 189,
            [Description("PANAMA")]
            [EnumMember(Value = "190")]
            PANAMA = 190,
            [Description("PAPUA NEW GUINEA")]
            [EnumMember(Value = "191")]
            PAPUANEWGUINEA = 191,
            [Description("PARACEL ISLANDS")]
            [EnumMember(Value = "192")]
            PARACELISLANDS = 192,
            [Description("PARAGUAY")]
            [EnumMember(Value = "193")]
            PARAGUAY = 193,
            [Description("PERU")]
            [EnumMember(Value = "194")]
            PERU = 194,
            [Description("PHILIPPINES")]
            [EnumMember(Value = "195")]
            PHILIPPINES = 195,
            [Description("PITCAIRN ISLANDS")]
            [EnumMember(Value = "196")]
            PITCAIRNISLANDS = 196,
            [Description("PITCAIRN, HENDERSON, DUCIE, AND OENO ISLANDS")]
            [EnumMember(Value = "197")]
            PITCAIRNHENDERSONDUCIEANDOENOISLANDS = 197,
            [Description("POLAND")]
            [EnumMember(Value = "198")]
            POLAND = 198,
            [Description("PORTUGAL")]
            [EnumMember(Value = "199")]
            PORTUGAL = 199,
            [Description("PUERTO RICO")]
            [EnumMember(Value = "200")]
            PUERTORICO = 200,
            [Description("QATAR")]
            [EnumMember(Value = "201")]
            QATAR = 201,
            [Description("REUNION")]
            [EnumMember(Value = "202")]
            REUNION = 202,
            [Description("ROMANIA")]
            [EnumMember(Value = "203")]
            ROMANIA = 203,
            [Description("RUSSIA")]
            [EnumMember(Value = "204")]
            RUSSIA = 204,
            [Description("RWANDA")]
            [EnumMember(Value = "205")]
            RWANDA = 205,
            [Description("SAINT BARTHELEMY")]
            [EnumMember(Value = "206")]
            SAINTBARTHELEMY = 206,
            [Description("SAINT HELENA")]
            [EnumMember(Value = "207")]
            SAINTHELENA = 207,
            [Description("SAINT KITTS AND NEVIS")]
            [EnumMember(Value = "208")]
            SAINTKITTSANDNEVIS = 208,
            [Description("SAINT LUCIA")]
            [EnumMember(Value = "209")]
            SAINTLUCIA = 209,
            [Description("SAINT MARTIN")]
            [EnumMember(Value = "210")]
            SAINTMARTIN = 210,
            [Description("SAINT PIERRE AND MIQUELON")]
            [EnumMember(Value = "211")]
            SAINTPIERREANDMIQUELON = 211,
            [Description("SAINT VINCENT AND THE GRENADINES")]
            [EnumMember(Value = "212")]
            SAINTVINCENTANDTHEGRENADINES = 212,
            [Description("SAMOA")]
            [EnumMember(Value = "213")]
            SAMOA = 213,
            [Description("SANMARINO")]
            [EnumMember(Value = "214")]
            SANMARINO = 214,
            [Description("SAO TOME AND PRINCIPE")]
            [EnumMember(Value = "215")]
            SAOTOMEANDPRINCIPE = 215,
            [Description("SAUDI ARABIA")]
            [EnumMember(Value = "216")]
            SAUDIARABIA = 216,
            [Description("SENEGAL")]
            [EnumMember(Value = "217")]
            SENEGAL = 217,
            [Description("SERBIA")]
            [EnumMember(Value = "218")]
            SERBIA = 218,
            [Description("SEYCHELLES")]
            [EnumMember(Value = "219")]
            SEYCHELLES = 219,
            [Description("SIERRA LEONE")]
            [EnumMember(Value = "220")]
            SIERRALEONE = 220,
            [Description("SINGAPORE")]
            [EnumMember(Value = "221")]
            SINGAPORE = 221,
            [Description("SLOVAKIA")]
            [EnumMember(Value = "222")]
            SLOVAKIA = 222,
            [Description("SLOVENIA")]
            [EnumMember(Value = "223")]
            SLOVENIA = 223,
            [Description("SOLOMON ISLANDS")]
            [EnumMember(Value = "224")]
            SOLOMONISLANDS = 224,
            [Description("SOMALIA")]
            [EnumMember(Value = "225")]
            SOMALIA = 225,
            [Description("SOUTH AFRICA")]
            [EnumMember(Value = "226")]
            SOUTHAFRICA = 226,
            [Description("SOUTH GEORGIA AND SOUTH SANDWICH ISLANDS")]
            [EnumMember(Value = "227")]
            SOUTHGEORGIAANDSOUTHSANDWICHISLANDS = 227,
            [Description("SPRATLY ISLANDS")]
            [EnumMember(Value = "228")]
            SPRATLYISLANDS = 228,
            [Description("SPAIN")]
            [EnumMember(Value = "229")]
            SPAIN = 229,
            [Description("SRI LANKA")]
            [EnumMember(Value = "230")]
            SRILANKA = 230,
            [Description("ST. VINCENT & GRENADINES")]
            [EnumMember(Value = "231")]
            STVINCENTGRENADINES = 231,
            [Description("SUDAN")]
            [EnumMember(Value = "232")]
            SUDAN = 232,
            [Description("SURINAME")]
            [EnumMember(Value = "233")]
            SURINAME = 233,
            [Description("SVALBARD")]
            [EnumMember(Value = "234")]
            SVALBARD = 234,
            [Description("SWAZILAND")]
            [EnumMember(Value = "235")]
            SWAZILAND = 235,
            [Description("SWEDEN")]
            [EnumMember(Value = "236")]
            SWEDEN = 236,
            [Description("SWITZERLAND")]
            [EnumMember(Value = "237")]
            SWITZERLAND = 237,
            [Description("SYRIA")]
            [EnumMember(Value = "238")]
            SYRIA = 238,
            [Description("TAIWAN")]
            [EnumMember(Value = "239")]
            TAIWAN = 239,
            [Description("TAJIKISTAN")]
            [EnumMember(Value = "240")]
            TAJIKISTAN = 240,
            [Description("TANZANIA")]
            [EnumMember(Value = "241")]
            TANZANIA = 241,
            [Description("TERRITORIAL COLLECTIVITY OF ST. PIERRE & MIQUELON")]
            [EnumMember(Value = "242")]
            TERRITORIALCOLLECTIVITYOFSTPIERREMIQUELON = 242,
            [Description("TERRITORY OF AMERICAN SAMOA")]
            [EnumMember(Value = "243")]
            TERRITORYOFAMERICANSAMOA = 243,
            [Description("TERRITORY OF ASHMORE AND CARTIER ISLANDS")]
            [EnumMember(Value = "244")]
            TERRITORYOFASHMOREANDCARTIERISLANDS = 244,
            [Description("TERRITORY OF CHRISTMAS ISLAND")]
            [EnumMember(Value = "245")]
            TERRITORYOFCHRISTMASISLAND = 245,
            [Description("TERRITORY OF COCOS (KEELING) ISLANDS")]
            [EnumMember(Value = "246")]
            TERRITORYOFCOCOSKEELINGISLANDS = 246,
            [Description("TERRITORY OF GUAM")]
            [EnumMember(Value = "247")]
            TERRITORYOFGUAM = 247,
            [Description("TERRITORY OF HEARD ISLAND & MCDONALD ISLANDS")]
            [EnumMember(Value = "248")]
            TERRITORYOFHEARDISLANDMCDONALDISLANDS = 248,
            [Description("TERRITORY OF NORFOLK ISLAND")]
            [EnumMember(Value = "249")]
            TERRITORYOFNORFOLKISLAND = 249,
            [Description("THAILAND")]
            [EnumMember(Value = "250")]
            THAILAND = 250,
            [Description("THE BAHAMAS")]
            [EnumMember(Value = "251")]
            THEBAHAMAS = 251,
            [Description("THE GAMBIA")]
            [EnumMember(Value = "252")]
            THEGAMBIA = 252,
            [Description("TOGO")]
            [EnumMember(Value = "253")]
            TOGO = 253,
            [Description("TOKELAU")]
            [EnumMember(Value = "254")]
            TOKELAU = 254,
            [Description("TONGA")]
            [EnumMember(Value = "255")]
            TONGA = 255,
            [Description("TRINIDAD AND TOBAGO")]
            [EnumMember(Value = "256")]
            TRINIDADANDTOBAGO = 256,
            [Description("TUNISIA")]
            [EnumMember(Value = "257")]
            TUNISIA = 257,
            [Description("TURKEY")]
            [EnumMember(Value = "258")]
            TURKEY = 258,
            [Description("TURKMENISTAN")]
            [EnumMember(Value = "259")]
            TURKMENISTAN = 259,
            [Description("TURKS AND CAICOS ISLANDS")]
            [EnumMember(Value = "260")]
            TURKSANDCAICOSISLANDS = 260,
            [Description("TUVALU")]
            [EnumMember(Value = "261")]
            TUVALU = 261,
            [Description("UGANDA")]
            [EnumMember(Value = "262")]
            UGANDA = 262,
            [Description("UKRAINE")]
            [EnumMember(Value = "263")]
            UKRAINE = 263,
            [Description("UNITED ARAB EMIRATES")]
            [EnumMember(Value = "264")]
            UNITEDARABEMIRATES = 264,
            [Description("UNITED KINGDOM")]
            [EnumMember(Value = "265")]
            UNITEDKINGDOM = 265,
            [Description("UNITED STATES VIRGIN ISLANDS")]
            [EnumMember(Value = "266")]
            UNITEDSTATESVIRGINISLANDS = 266,
            [Description("UNITED STATES OF AMERICA")]
            [EnumMember(Value = "267")]
            UNITEDSTATESOFAMERICA = 267,
            [Description("URUGUAY")]
            [EnumMember(Value = "268")]
            URUGUAY = 268,
            [Description("UZBEKISTAN")]
            [EnumMember(Value = "269")]
            UZBEKISTAN = 269,
            [Description("VANUATU")]
            [EnumMember(Value = "270")]
            VANUATU = 270,
            [Description("VATICAN CITY (HOLYSEE)")]
            [EnumMember(Value = "271")]
            VATICANCITYHOLYSEE = 271,
            [Description("VENEZUELA")]
            [EnumMember(Value = "272")]
            VENEZUELA = 272,
            [Description("VIETNAM")]
            [EnumMember(Value = "273")]
            VIETNAM = 273,
            [Description("VIRGIN ISLANDS, BRITISH")]
            [EnumMember(Value = "274")]
            VIRGINISLANDSBRITISH = 274,
            [Description("VIRGIN ISLANDS, U.S.")]
            [EnumMember(Value = "275")]
            VIRGINISLANDSUS = 275,
            [Description("WAKE ISLAND")]
            [EnumMember(Value = "276")]
            WAKEISLAND = 276,
            [Description("WALLIS AND FUTUNA")]
            [EnumMember(Value = "277")]
            WALLISANDFUTUNA = 277,
            [Description("WESTERN SAHARA")]
            [EnumMember(Value = "278")]
            WESTERNSAHARA = 278,
            [Description("YEMEN")]
            [EnumMember(Value = "279")]
            YEMEN = 279,
            [Description("ZAMBIA")]
            [EnumMember(Value = "280")]
            ZAMBIA = 280,
            [Description("ZIMBABWE")]
            [EnumMember(Value = "281")]
            ZIMBABWE = 281,
            [Description("COMBODIA")]
            [EnumMember(Value = "282")]
            COMBODIA = 282,
            [Description("CONGO")]
            [EnumMember(Value = "283")]
            CONGO = 283,
            [Description("IVORY COAST")]
            [EnumMember(Value = "284")]
            IVORYCOAST = 284,
            [Description("WEST INDIES")]
            [EnumMember(Value = "285")]
            WESTINDIES = 285,
            [Description("BRITISH VIRGIN ISLANDS")]
            [EnumMember(Value = "286")]
            BRITISHVIRGINISLANDS = 286,
        }
        public enum DeductorType
        {
            [Description("Central Government")]
            [EnumMember(Value = "A")]
            CentralGovernment,
            [Description("State Government")]
            [EnumMember(Value = "S")]
            StateGovernment,
            [Description("Statutory body (Central Govt.)")]
            [EnumMember(Value = "D")]
            Statutorybody,
            [Description("Statutory body (State Govt.)")]
            [EnumMember(Value = "E")]
            StatutorybodyGovt,
            [Description("Autonomous body (Central Govt.)")]
            [EnumMember(Value = "G")]
            Autonomousbody,
            [Description("Autonomous body (State Govt.)")]
            [EnumMember(Value = "H")]
            AutonomousbodyStateGovt,
            [Description("Local Authority (Central Govt.)")]
            [EnumMember(Value = "L")]
            LocalAuthority,
            [Description("Local Authority (State Govt.)")]
            [EnumMember(Value = "N")]
            LocalAuthorityStateGovt,
            [Description("Company")]
            [EnumMember(Value = "K")]
            Company,
            [Description("Branch/Division of Company")]
            [EnumMember(Value = "M")]
            Branch,
            [Description("Association of Person (AOP)")]
            [EnumMember(Value = "P")]
            AOP,
            [Description("Association of Person (Trust)")]
            [EnumMember(Value = "T")]
            Trust,
            [Description("Artificial Juridical Person")]
            [EnumMember(Value = "J")]
            ArtificialJuridical,
            [Description("Body of Individuals")]
            [EnumMember(Value = "B")]
            Body,
            [Description("Individual/HUF")]
            [EnumMember(Value = "Q")]
            Individual,
            [Description("Firm")]
            [EnumMember(Value = "F")]
            Firm,
        }
        public enum State
        {
            [Description("ANDAMAN AND NICOBAR ISLANDS")]
            [EnumMember(Value = "1")]
            AndamanandNicobarIslands,
            [Description("ANDHRA PRADESH")]
            [EnumMember(Value = "2")]
            AndhraPradesh,
            [Description("ARUNACHAL PRADESH")]
            [EnumMember(Value = "3")]
            ArunachalPradesh,
            [Description("ASSAM")]
            [EnumMember(Value = "4")]
            Assam,
            [Description("BIHAR")]
            [EnumMember(Value = "5")]
            Bihar,
            [Description("CHANDIGARH")]
            [EnumMember(Value = "6")]
            Chandigarh,
            [Description("DADRA & NAGAR HAVELI AND DAMAN & DIU")]
            [EnumMember(Value = "7")]
            DadraAndNagarHaveli,
            [Description("DELHI")]
            [EnumMember(Value = "9")]
            Delhi,
            [Description("GOA")]
            [EnumMember(Value = "10")]
            Goa,
            [Description("GUJARAT")]
            [EnumMember(Value = "11")]
            Gujarat,
            [Description("HARYANA")]
            [EnumMember(Value = "12")]
            Haryana,
            [Description("HIMACHAL PRADESH")]
            [EnumMember(Value = "13")]
            HimachalPradesh,
            [Description("JAMMU & KASHMIR")]
            [EnumMember(Value = "14")]
            JammuKashmir,
            [Description("KARNATAKA")]
            [EnumMember(Value = "15")]
            Karnataka,
            [Description("KERALA")]
            [EnumMember(Value = "16")]
            Kerala,
            [Description("LAKSHWADEEP")]
            [EnumMember(Value = "17")]
            Lakshwadeep,
            [Description("MADHYA PRADESH")]
            [EnumMember(Value = "18")]
            MadhyaPradesh,
            [Description("MAHARASHTRA")]
            [EnumMember(Value = "19")]
            Maharashtra,
            [Description("MANIPUR")]
            [EnumMember(Value = "20")]
            Manipur,
            [Description("MEGHALAYA")]
            [EnumMember(Value = "21")]
            Meghalaya,
            [Description("MIZORAM")]
            [EnumMember(Value = "22")]
            Mizoram,
            [Description("NAGALAND")]
            [EnumMember(Value = "23")]
            Nagaland,
            [Description("ODISHA")]
            [EnumMember(Value = "24")]
            Odisha,
            [Description("PONDICHERRY")]
            [EnumMember(Value = "25")]
            Pondicherry,
            [Description("PUNJAB")]
            [EnumMember(Value = "26")]
            Punjab,
            [Description("RAJASTHAN")]
            [EnumMember(Value = "27")]
            Rajasthan,
            [Description("SIKKIM")]
            [EnumMember(Value = "28")]
            Sikkim,
            [Description("TAMIL NADU")]
            [EnumMember(Value = "29")]
            TamilNadu,
            [Description("TRIPURA")]
            [EnumMember(Value = "30")]
            Tripura,
            [Description("UTTAR PRADESH")]
            [EnumMember(Value = "31")]
            UttarPradesh,
            [Description("WEST BENGAL")]
            [EnumMember(Value = "32")]
            WestBengal,
            [Description("CHHATTISGARH")]
            [EnumMember(Value = "33")]
            Chhattisgarh,
            [Description("UTTARAKHAND")]
            [EnumMember(Value = "34")]
            Uttarakhand,
            [Description("JHARKHAND")]
            [EnumMember(Value = "35")]
            Jharkhand,
            [Description("TELANGANA")]
            [EnumMember(Value = "36")]
            Telangana,
            [Description("LADAKH")]
            [EnumMember(Value = "37")]
            Ladakh,
            [Description("OVERSEAS")]
            [EnumMember(Value = "38")]
            OVERSEAS
        }
        public enum Ministry
        {
            [Description("Agriculture")]
            [EnumMember(Value = "01")]
            Agriculture,
            [Description("Atomic Energy")]
            [EnumMember(Value = "02")]
            AtomicEnergy,
            [Description("Fertilizers")]
            [EnumMember(Value = "03")]
            Fertilizers,
            [Description("Chemicals and Petrochemicals")]
            [EnumMember(Value = "04")]
            ChemicalsandPetrochemicals,
            [Description("Civil Aviation and Tourism")]
            [EnumMember(Value = "05")]
            CivilAviationandTourism,
            [Description("Coal")]
            [EnumMember(Value = "06")]
            Coal = 6,
            [Description("Consumer Affairs, Food and Public Distribution")]
            [EnumMember(Value = "07")]
            ConsumerAffairsFoodandPublicDistribution,
            [Description("Commerce and Textiles")]
            [EnumMember(Value = "08")]
            CommerceandTextiles,
            [Description("Environment and Forests and Ministry of Earth Science")]
            [EnumMember(Value = "09")]
            EnvironmentandForestsandMinistryofEarthScience,
            [Description("External Affairs and Overseas Indian Affairs")]
            [EnumMember(Value = "10")]
            ExternalAffairsandOverseasIndianAffairs,
            [EnumMember(Value = "11")]
            [Description("Finance")]
            Finance,
            [Description("Central Board of Direct Taxes")]
            [EnumMember(Value = "12")]
            CentralBoardofDirectTaxes,
            [Description("Central Board of Excise and Customs")]
            [EnumMember(Value = "13")]
            CentralBoardofExciseandCustoms,
            [Description("Contoller of Aid Accounts and Audit")]
            [EnumMember(Value = "14")]
            ContollerofAidAccountsandAudit,
            [Description("Central Pension Accounting Office")]
            [EnumMember(Value = "15")]
            CentralPensionAccountingOffice = 15,
            [Description("Food Processing Industries")]
            [EnumMember(Value = "16")]
            FoodProcessingIndustries = 16,
            [Description("Health and Family Welfare")]
            [EnumMember(Value = "17")]
            HealthandFamilyWelfare = 17,
            [Description("Home Affairs and Development of North Eastern Region")]
            [EnumMember(Value = "18")]
            HomeAffairsandDevelopmentofNorthEasternRegion = 18,
            [Description("Human Resource Development")]
            [EnumMember(Value = "19")]
            HumanResourceDevelopment = 19,
            [Description("Industry")]
            [EnumMember(Value = "20")]
            Industry = 20,
            [Description("Information and Broadcasting")]
            [EnumMember(Value = "21")]
            InformationandBroadcasting = 21,
            [Description("Telecommunication and Information Technology")]
            [EnumMember(Value = "22")]
            TelecommunicationandInformationTechnology = 22,
            [Description("Labour")]
            [EnumMember(Value = "23")]
            Labour = 23,
            [Description("Law and Justice and Company Affairs")]
            [EnumMember(Value = "24")]
            LawandJusticeandCompanyAffairs = 24,
            [Description("Personnel, Public Grievances and Pesions")]
            [EnumMember(Value = "25")]
            PersonnelPublicGrievancesandPesions = 25,
            [Description("Petroleum and Natural Gas")]
            [EnumMember(Value = "26")]
            PetroleumandNaturalGas = 26,
            [Description("Plannning, Statistics and Programme Implementation")]
            [EnumMember(Value = "27")]
            PlannningStatisticsandProgrammeImplementation = 27,
            [Description("Power")]
            [EnumMember(Value = "28")]
            Power = 28,
            [Description("New and Renewable Energy")]
            [EnumMember(Value = "29")]
            NewandRenewableEnergy = 29,
            [Description("Rural Development and Panchayati Raj")]
            [EnumMember(Value = "30")]
            RuralDevelopmentandPanchayatiRaj = 30,
            [Description("Science And Technology")]
            [EnumMember(Value = "31")]
            ScienceAndTechnology = 31,
            [Description("Space")]
            [EnumMember(Value = "32")]
            Space = 32,
            [Description("Steel")]
            [EnumMember(Value = "33")]
            Steel = 33,
            [Description("Mines")]
            [EnumMember(Value = "34")]
            Mines = 34,
            [Description("Social Justice and Empowerment")]
            [EnumMember(Value = "35")]
            SocialJusticeandEmpowerment = 35,
            [Description("Tribal Affairs")]
            [EnumMember(Value = "36")]
            TribalAffairs = 36,
            [Description("D/o Commerce (Supply Division)")]
            [EnumMember(Value = "37")]
            DoCommerce = 37,
            [Description("Shipping and Road Transport and Highways")]
            [EnumMember(Value = "38")]
            ShippingandRoadTransportandHighways = 38,
            [Description("Urban Development, Urban Employment and Poverty Alleviation")]
            [EnumMember(Value = "39")]
            UrbanDevelopmentUrbanEmploymentandPovertyAlleviation = 39,
            [Description("Water Resources")]
            [EnumMember(Value = "40")]
            WaterResources = 40,
            [Description("President's Secretariat")]
            [EnumMember(Value = "41")]
            PresidentSecretariat = 41,
            [Description("Lok Sabha Secretariat")]
            [EnumMember(Value = "42")]
            LokSabhaSecretariat = 42,
            [Description("Rajya Sabha secretariat")]
            [EnumMember(Value = "43")]
            RajyaSabhasecretariat = 43,
            [Description("Election Commission")]
            [EnumMember(Value = "44")]
            ElectionCommission = 44,
            [Description("Ministry of Defence (Controller General of Defence Accounts)")]
            [EnumMember(Value = "45")]
            MinistryofDefence = 45,
            [Description("Ministry of Railways")]
            [EnumMember(Value = "46")]
            MinistryofRailways = 46,
            [Description("Department of Posts")]
            [EnumMember(Value = "47")]
            DepartmentofPosts = 47,
            [Description("Department of Telecommunications")]
            [EnumMember(Value = "48")]
            DepartmentofTelecommunications = 48,
            [Description("Andaman and Nicobar Islands Administration")]
            [EnumMember(Value = "49")]
            AndamanandNicobarIslandsAdministration = 49,
            [Description("Chandigarh Administration")]
            [EnumMember(Value = "50")]
            ChandigarhAdministration = 50,
            [Description("Dadra and Nagar Haveli")]
            [EnumMember(Value = "51")]
            DadraandNagarHaveli = 51,
            [Description("Goa, Daman and Diu")]
            [EnumMember(Value = "52")]
            GoaDamanandDiu = 52,
            [Description("Lakshadweep")]
            [EnumMember(Value = "53")]
            Lakshadweep = 53,
            [Description("Pondicherry Administration")]
            [EnumMember(Value = "54")]
            PondicherryAdministration = 54,
            [Description("Pay and Accounts Officers (Audit)")]
            [EnumMember(Value = "55")]
            PayandAccountsOfficers = 55,
            [Description("Ministry of Non-conventional energy sources")]
            [EnumMember(Value = "56")]
            MinistryofNonconventionalenergysources = 56,
            [Description("Government Of NCT of Delhi")]
            [EnumMember(Value = "57")]
            GovernmentOfNCTofDelhi = 57,
            [Description("Others")]
            [EnumMember(Value = "99")]
            Others = 99,
        }
        public enum SectionCode27Q
        {
            [Description("195-Payment to Non-residents")]
            [EnumMember(Value = "195")]
            Section1,
            [Description("194E-Non-resident Sportsman or Sports Assn.")]
            [EnumMember(Value = "94E")]
            Section2,
            [Description("196A-Foreign Company being unit holder in Mutual Fund")]
            [EnumMember(Value = "96A")]
            Section3,
            [Description("196B-Units held by Off-shore Fund")]
            [EnumMember(Value = "96B")]
            Section4,
            [Description("196C-Income from Foreign Currency Bonds")]
            [EnumMember(Value = "96C")]
            Section5,
            [Description("196D-Income of FII on Securities")]
            [EnumMember(Value = "96D")]
            Section6,
            [Description("194LB-Interest from infrastr. debt fund payable to non resident")]
            [EnumMember(Value = "4LB")]
            Section7,
            [Description("194LC(2)(i)/(ia)-Income under section 194LC(2)(i) and (ia)")]
            [EnumMember(Value = "LC1")]
            Section8,
            [Description("194LC(2)(ib)-Income under section 194LC(2)(ib)")]
            [EnumMember(Value = "LC2")]
            Section9,
            [Description("194LC(2)(ic)-Income under section 194LC(2)(ic)")]
            [EnumMember(Value = "LC3")]
            Section10,
            [Description("194LD-Interest on certain bonds and govt securities")]
            [EnumMember(Value = "4LD")]
            Section11,
            [Description("192A-Payment against EPF Scheme")]
            [EnumMember(Value = "2AA")]
            Section12,
            [Description("194LBB-Income on units of Investment Fund")]
            [EnumMember(Value = "LBB")]
            Section14,
            [Description("194LBC-Income in respect of investment in securitization trust")]
            [EnumMember(Value = "LBC")]
            Section15,
            [Description("194N-Payment of certain amounts in cash")]
            [EnumMember(Value = "94N")]
            Section16,
            [Description("194LBA(a)-Income referred to in section 10(23FC)(a) from units of a business trust")]
            [EnumMember(Value = "BA1")]
            Section17,
            [Description("194LBA(b)-Income referred to in section 10(23FC)(b) from units of a business trust")]
            [EnumMember(Value = "BA2")]
            Section18,
            [Description("194LBA(c)-Income referred to in section 10(23FCA) from units of a business trust")]
            [EnumMember(Value = "BA3")]
            Section19,
            [Description("194NF-Payment of certain amounts in cash to non-filers")]
            [EnumMember(Value = "4NF")]
            Section20,
            [Description("196D(1A)-Income of specified fund from securities referred to in section 115AD(1)(a)")]
            [EnumMember(Value = "6DA")]
            Section21,
            [Description("194B-Winnings from lottery or crossword puzzle, etc")]
            [EnumMember(Value = "94B")]
            Section22,
            [Description("194B-P-Winnings from  lottery or crossword puzzle, etc where consideration is made in kind or cash is not sufficient")]
            [EnumMember(Value = "4BP")]
            Section23,
            [Description("194BA-Winnings from online games")]
            [EnumMember(Value = "9BA")]
            Section24,
            [Description("194BAP-Net Winnings from online games-Kind/Cash not sufficient")]
            [EnumMember(Value = "4AP")]
            Section25,
            [Description("194BB-Winnings from horse race")]
            [EnumMember(Value = "4BB")]
            Section26,
            [Description("194NC-Payment of certain amounts in cash to co-operative societies not covered by first proviso")]
            [EnumMember(Value = "4NC")]
            Section27,
            [Description("194N-FT-Payment of certain amount in cash to non-filers being co-operative societies")]
            [EnumMember(Value = "9FT")]
            Section28,
            [Description("194T-Payment of salary, remuneration, commission, bonus or interest to a partner of the firm")]
            [EnumMember(Value = "94T")]
            Section29
        }
        public enum SectionCode27EQ
        {
            [Description("206CA-Alcholic liquor for human consumption & Tendu Leaves")]
            [EnumMember(Value = "A")]
            Section29,
            [Description("206CB-Timber optained under a forest lease")]
            [EnumMember(Value = "B")]
            Section30,
            [Description("206CC-Timber obtained under mode other than forest lease")]
            [EnumMember(Value = "C")]
            Section31,
            [Description("206CD-Any other forest product not being timber or tendu leave")]
            [EnumMember(Value = "D")]
            Section32,
            [Description("206CE-Scrap")]
            [EnumMember(Value = "E")]
            Section33,
            [Description("206CF-Parking Lot")]
            [EnumMember(Value = "F")]
            Section34,
            [Description("206CG-Toll Plaza")]
            [EnumMember(Value = "G")]
            Section35,
            [Description("206CH-Mining and quarring")]
            [EnumMember(Value = "H")]
            Section36,
            [Description("206CI-Tendu leaves")]
            [EnumMember(Value = "I")]
            Section37,
            [Description("206CJ-Minerals")]
            [EnumMember(Value = "J")]
            Section38,
            [Description("206CK-Bullion and Jewellery")]
            [EnumMember(Value = "K")]
            Section39,
            [Description("206CL-Sale of Motor vehicle")]
            [EnumMember(Value = "L")]
            Section40,
            [Description("206CM-Sale in cash of any goods (other than bullion/jewellery)")]
            [EnumMember(Value = "M")]
            Section41,
            [Description("206CN-Providing of any services (other than Ch-XVII-B)")]
            [EnumMember(Value = "N")]
            Section42,
            [Description("206CO-LRS-Overseas Tour Program Package")]
            [EnumMember(Value = "O")]
            Section43,
            [Description("206CP-LRS-Educational-Loan from Financial Institution")]
            [EnumMember(Value = "P")]
            Section44,
            [Description("206CQ-LRS-Other Purposes")]
            [EnumMember(Value = "Q")]
            Section45,
            [Description("206CR-Sale of Goods")]
            [EnumMember(Value = "R")]
            Section46,
            [Description("206CP-LRS-Education/Medical-Non Financial Institution")]
            [EnumMember(Value = "T")]
            Section47,
            [Description("206CMA-Sale of wrist watch")]
            [EnumMember(Value = "MA")]
            Section48,
            [Description("206CMB-Sale of art piece such as antiques, painting, sculpture")]
            [EnumMember(Value = "MB")]
            Section49,
            [Description("206CMC-Sale of collectibles such as coin, stamp")]
            [EnumMember(Value = "MC")]
            Section50,
            [Description("206CMD-Sale of yacht, rowing boat, canoe, helicopter")]
            [EnumMember(Value = "MD")]
            Section51,
            [Description("206CME-Sale of pair of sunglasses")]
            [EnumMember(Value = "ME")]
            Section52,
            [Description("206CMF-Sale of bag such as handbag, purse")]
            [EnumMember(Value = "MF")]
            Section53,
            [Description("206CMG-Sale of pair of shoes")]
            [EnumMember(Value = "MG")]
            Section54,
            [Description("206CMH-Sale of sportswear and equipment such as golf kit, ski-wear")]
            [EnumMember(Value = "MH")]
            Section55,
            [Description("206CMI-Sale of home theatre system")]
            [EnumMember(Value = "MI")]
            Section56,
            [Description("206CMJ-Sale of horse for horse racing in race clubs and horse for polo")]
            [EnumMember(Value = "MJ")]
            Section57,
        }
        public enum SectionCode24Q
        {
            [Description("192A-Payments made to Govt. employees other than Union Govt. employees")]
            [EnumMember(Value = "92A")]
            Section91,
            [Description("192B-Payments made to employees other than Govt. employees")]
            [EnumMember(Value = "92B")]
            Section92,
            [Description("192C-Payments made to Union Govt. employees")]
            [EnumMember(Value = "92C")]
            Section93,
            [Description("194P-Deduction of tax in case of specified senior citizens")]
            [EnumMember(Value = "94P")]
            Section94,
        }
        public enum SectionCode26Q
        {
            [Description("193-Interest on Securities")]
            [EnumMember(Value = "193")]
            Section48,
            [Description("194-Dividends")]
            [EnumMember(Value = "194")]
            Section49,
            [Description("194A-Interest other than interest on securities")]
            [EnumMember(Value = "94A")]
            Section50,
            [Description("194B-Winning from Lotteries or Crossword Puzzles")]
            [EnumMember(Value = "94B")]
            Section51,
            [Description("194BP-Winnings from  lottery or crossword puzzle, etc-Kind/Cash not sufficient")]
            [EnumMember(Value = "4BP")]
            Section52,
            [Description("194BB-Winning from Horse Race")]
            [EnumMember(Value = "4BB")]
            Section53,
            [Description("194C-Contractors/Sub Contractors")]
            [EnumMember(Value = "94C")]
            Section54,
            [Description("194D-Insurance Commission")]
            [EnumMember(Value = "94D")]
            Section55,
            [Description("194EE-National Savings Certificate")]
            [EnumMember(Value = "4EE")]
            Section56,
            [Description("194F-Equity-linked Savings Scheme")]
            [EnumMember(Value = "94F")]
            Section57,
            [Description("194G-Commission of sale of Lottery Tickets")]
            [EnumMember(Value = "94G")]
            Section58,
            [Description("194H-Commission & Brokerage")]
            [EnumMember(Value = "94H")]
            Section59,
            [Description("194I(a)-Rent for Plant & Machinery")]
            [EnumMember(Value = "4IA")]
            Section60,
            [Description("194I(b)-Rent for land, building & furniture")]
            [EnumMember(Value = "4IB")]
            Section61,
            [Description("194LA-Payment of Compensation on Acquisition")]
            [EnumMember(Value = "94L")]
            Section62,
            [Description("194DA-Payment in respect of life insurance policy")]
            [EnumMember(Value = "4DA")]
            Section63,
            [Description("194LBA-Certain income from units of a business trust")]
            [EnumMember(Value = "4BA")]
            Section64,
            [Description("192A-Payment against EPF Scheme")]
            [EnumMember(Value = "2AA")]
            Section65,
            [Description("194LBB-Income on units of Investment Fund")]
            [EnumMember(Value = "LBB")]
            Section66,
            [Description("194IA-TDS on sale of property")]
            [EnumMember(Value = "9IA")]
            Section67,
            [Description("194LBC-Income in respect of investment in securitization trust")]
            [EnumMember(Value = "LBC")]
            Section68,
            [Description("194IC-Payment under specified agreement")]
            [EnumMember(Value = "4IC")]
            Section69,
            [Description("194N-Payment of certain amounts in cash")]
            [EnumMember(Value = "94N")]
            Section70,
            [Description("194K-Payment of dividend by Mutual Funds")]
            [EnumMember(Value = "94K")]
            Section71,
            [Description("194J(a)-Fees for technical services, call centre, royalty for sale, distribution/exhibition of cinematographic films")]
            [EnumMember(Value = "4JA")]
            Section72,
            [Description("194J(b)-Fee for professional service or royalty etc")]
            [EnumMember(Value = "4JB")]
            Section73,
            [Description("194LBA(a)-Interest income from units of a business trust to a residential unit holder")]
            [EnumMember(Value = "BA1")]
            Section74,
            [Description("194LBA(b)-Dividend income from units of a business trust to a residential unit holder")]
            [EnumMember(Value = "BA2")]
            Section75,
            [Description("194NF-Payment of certain amounts in cash to non-filers")]
            [EnumMember(Value = "4NF")]
            Section76,
            [Description("194O-TDS on E-commerce transactions")]
            [EnumMember(Value = "94O")]
            Section77,
            [Description("194Q-Payment of certain sums for purchase of goods")]
            [EnumMember(Value = "94Q")]
            Section78,
            [Description("194R-Benefits or perquisites of business or profession")]
            [EnumMember(Value = "94R")]
            Section79,
            [Description("194RP-Benefits or perquisites of business or profession-Kind/Cash not sufficient")]
            [EnumMember(Value = "4RP")]
            Section80,
            [Description("194S-Transfer of virtual digital asset by persons other than specified persons")]
            [EnumMember(Value = "94S")]
            Section81,
            [Description("194SP-Transfer of virtual digital asset-Kind/Cash not sufficient")]
            [EnumMember(Value = "4SP")]
            Section82,
            [Description("194BA-Winnings from online games")]
            [EnumMember(Value = "9BA")]
            Section83,
            [Description("194BAP-Net Winnings from online games-Kind/Cash not sufficient")]
            [EnumMember(Value = "4AP")]
            Section84,
            [Description("194NC-Payment of certain amounts in cash to co-operative societies not covered by first proviso")]
            [EnumMember(Value = "4NC")]
            Section85,
            [Description("194NFT-Payment of certain amount in cash to non-filers being co-operative societies")]
            [EnumMember(Value = "9FT")]
            Section86,
            [Description("194B-Winnings from lottery or crossword puzzle, etc")]
            [EnumMember(Value = "94B")]
            Section88,
            [Description("194T-Payment of salary, remuneration, commission, bonus or interest to a partner of the firm")]
            [EnumMember(Value = "94T")]
            Section89
        }
        public enum TDSRateCode
        {
            [Description("As per Income Tax Act")]
            [EnumMember(Value = "A")]
            AsPerIncomeTaxAct,
            [Description("As per DTAA")]
            [EnumMember(Value = "B")]
            AsPerDTAA,
        }
        public enum EmployeeCategory
        {
            [Description("S-Senior Citizen")]
            [EnumMember(Value = "S")]
            SeniorCitizen,
            [Description("O-Super Senior Citizen")]
            [EnumMember(Value = "O")]
            SSCitizen,
            [Description("G-Others")]
            [EnumMember(Value = "G")]
            Others,
        }


        public enum MinorCode27Q
        {
            [Description("200-Payable by taxpayer")]
            [EnumMember(Value = "200")]
            AsPerIncomeTaxAct,
            [Description("400-Regular Assessment")]
            [EnumMember(Value = "400")]
            AsPerDTAA,
            [Description("100-Advance Tax")]
            [EnumMember(Value = "100")]
            AdvanceTax,
        }
        public enum MinorCode26Q
        {
            [Description("200-Payable by taxpayer")]
            [EnumMember(Value = "200")]
            AsPerIncomeTaxAct,
            [Description("400-Regular Assessment")]
            [EnumMember(Value = "400")]
            AsPerDTAA,
            [Description("100-Advance Tax")]
            [EnumMember(Value = "100")]
            AdvanceTax,
        }
        public enum MinorCode27EQ
        {
            [Description("200-Payable by taxpayer")]
            [EnumMember(Value = "200")]
            AsPerIncomeTaxAct = 200,
            [Description("400-Regular Assessment")]
            [EnumMember(Value = "400")]
            AsPerDTAA = 400,
        }
        public enum NatureCode
        {
            [Description("Dividend")]
            [EnumMember(Value = "16")]
            Dividend,
            [Description("Fees for Technical Services/Fees for Included Services")]
            [EnumMember(Value = "21")]
            FeesForTechnicalServices,
            [Description("Interest Payment")]
            [EnumMember(Value = "27")]
            InterestPayment,
            [Description("Investment Income")]
            [EnumMember(Value = "28")]
            InvestmentIncome,
            [Description("Long Term Capital Gains (Others)")]
            [EnumMember(Value = "31")]
            LongTermCapitalGainsOthers,
            [Description("Long Term Capital Gain u/s 115E in case of Non Resident Indian Citizen")]
            [EnumMember(Value = "66")]
            LongTermCapitalGain115,
            [Description("Long Term Capital Gain u/s 112(1)(c)(iii)")]
            [EnumMember(Value = "67")]
            LongTermCapitalGain112,
            [Description("Long Term Capital Gain u/s 112")]
            [EnumMember(Value = "68")]
            LongTermCapitalGainUS112,
            [Description("Long Term Capital Gain u/s 112A")]
            [EnumMember(Value = "69")]
            LongTermCapitalGainUS112A,
            [Description("Royalty")]
            [EnumMember(Value = "49")]
            Royalty,
            [Description("Short Term Capital Gains")]
            [EnumMember(Value = "52")]
            ShortTermCapitalGains,
            [Description("Short Term Capital Gains u/s 111A")]
            [EnumMember(Value = "70")]
            ShortTermCapitalGains111A,
            [Description("Other Income/Other (Not in the Nature of Income)")]
            [EnumMember(Value = "99")]
            OtherIncome,
        }

        public enum DeducteeCode27QAnd27EQ
        {
            [Description("01-Company")]
            [EnumMember(Value = "1")]
            Company,
            [Description("02-Individual")]
            [EnumMember(Value = "2")]
            Individual,
            [Description("03-Hindu Undivided Family")]
            [EnumMember(Value = "3")]
            HinduUndividedFamily,
            [Description("04-AOP with No Company Member")]
            [EnumMember(Value = "4")]
            AOPOnlyNoCompanyMember,
            [Description("05-AOP Only Company Member")]
            [EnumMember(Value = "5")]
            AOPOnlyCompanyMember,
            [Description("06-Co-operative Society")]
            [EnumMember(Value = "6")]
            CooperativeSociety,
            [Description("07-Firm")]
            [EnumMember(Value = "7")]
            Firm,
            [Description("08-Body of individuals")]
            [EnumMember(Value = "8")]
            BodyOfIndividuals,
            [Description("09-Artificial juridical person")]
            [EnumMember(Value = "9")]
            JuridicialPerson,
            [Description("10-Others")]
            [EnumMember(Value = "10")]
            Others,
        }
        public enum DeducteeCode26Q
        {
            [Description("01-Company")]
            [EnumMember(Value = "1")]
            Company,
            [Description("02-Non-Company")]
            [EnumMember(Value = "2")]
            Individual,
        }

        public enum DeducteeStatus
        {
            [Description("01-Company")]
            [EnumMember(Value = "1")]
            Company,
            [Description("02-Non-Company")]
            [EnumMember(Value = "2")]
            NonCompany,
            [Description("02-Individual")]
            [EnumMember(Value = "02")]
            Individual,
            [Description("03-Hindu Undivided Family")]
            [EnumMember(Value = "03")]
            HinduUndividedFamily,
            [Description("04-AOP with No Company Member")]
            [EnumMember(Value = "04")]
            AOPOnlyNoCompanyMember,
            [Description("05-AOP Only Company Member")]
            [EnumMember(Value = "05")]
            AOPOnlyCompanyMember,
            [Description("06-Co-operative Society")]
            [EnumMember(Value = "06")]
            CooperativeSociety,
            [Description("07-Firm")]
            [EnumMember(Value = "07")]
            Firm,
            [Description("08-Body of individuals")]
            [EnumMember(Value = "08")]
            BodyOfIndividuals,
            [Description("09-Artificial juridical person")]
            [EnumMember(Value = "09")]
            JuridicialPerson,
            [Description("10-Others")]
            [EnumMember(Value = "10")]
            Others,
        }

        public enum ReasonsCode27Q
        {
            [Description("A-No deduction/lower deduction u/s 197")]
            [EnumMember(Value = "A")]
            ReasonsCode1,
            [Description("B-No deduction u/s 197A")]
            [EnumMember(Value = "B")]
            ReasonsCode2,
            [Description("C-Higher Rate (Valid PAN not available)")]
            [EnumMember(Value = "C")]
            ReasonsCode3,
            [Description("S-Software Providers")]
            [EnumMember(Value = "S")]
            ReasonsCode4,
            [Description("N-No deduction u/s 194N clause(iii, iv or v)")]
            [EnumMember(Value = "N")]
            ReasonsCode5,
            [Description("O-No deduction u/s 194LBA(2A)")]
            [EnumMember(Value = "O")]
            ReasonsCode6,
            [Description("P-No deduction-Notification u/s 197A(1F)")]
            [EnumMember(Value = "P")]
            ReasonsCode7,
            [Description("M-No deduction/lower deduction-Notification u/s 194N second proviso")]
            [EnumMember(Value = "M")]
            ReasonsCode8,
            [Description("G-No deduction u/s 197A(1D)(a)/(b)")]
            [EnumMember(Value = "G")]
            ReasonsCode9,
            [Description("I-No deduction u/s 196D(2)")]
            [EnumMember(Value = "I")]
            ReasonsCode10,
            [Description("H-No deduction u/s 196D(1A)")]
            [EnumMember(Value = "H")]
            ReasonsCode11,
            [Description("J-Higer rate in case of non-filer u/s 206AB")]
            [EnumMember(Value = "J")]
            ReasonsCode12,
            [Description("Y-Within threshold limit as per Income tax Act")]
            [EnumMember(Value = "Y")]
            ReasonsCode13,

        }
        public enum ReasonsCode27EQ
        {
            [Description("A-Lower collection u/s 206C(9)")]
            [EnumMember(Value = "A")]
            ReasonsCode14,
            [Description("B-Non collection u/s 206C(1A)")]
            [EnumMember(Value = "B")]
            ReasonsCode15,
            [Description("C-Higher Rate (Valid PAN not available)")]
            [EnumMember(Value = "C")]
            ReasonsCode16,
            [Description("D-Remittance is less than Rs. 7 lacs/Collection Code 206CP")]
            [EnumMember(Value = "D")]
            ReasonsCode17,
            [Description("E-TCS already collected")]
            [EnumMember(Value = "E")]
            ReasonsCode18,
            [Description("F-TDS by Buyer/Sale to Govt. & others as specified")]
            [EnumMember(Value = "F")]
            ReasonsCode19,
            [Description("G-TDS by Buyer on transaction")]
            [EnumMember(Value = "G")]
            ReasonsCode20,
            [Description("H-Sale to Govt. & others as specified")]
            [EnumMember(Value = "H")]
            ReasonsCode21,
            [Description("I-Higher rate u/s 206CCA")]
            [EnumMember(Value = "I")]
            ReasonsCode22,
        }
        public enum ReasonsCode26Q
        {
            [Description("A-Lower Deduction/No deduction u/s 197")]
            [EnumMember(Value = "A")]
            ReasonsCode23,
            [Description("B-No deduction u/s 197A (15G/15H)")]
            [EnumMember(Value = "B")]
            ReasonsCode24,
            [Description("C-Higher Rate (Valid PAN not available)")]
            [EnumMember(Value = "C")]
            ReasonsCode25,
            [Description("T-Transporter")]
            [EnumMember(Value = "T")]
            ReasonsCode26,
            [Description("Y-Within threshold limit as per Income tax Act")]
            [EnumMember(Value = "Y")]
            ReasonsCode27,
            [Description("S-Software Providers/Section 194Q-No deduction")]
            [EnumMember(Value = "S")]
            ReasonsCode28,
            [Description("Z-No deduction u/s 197A(1F)")]
            [EnumMember(Value = "Z")]
            ReasonsCode29,
            [Description("R-Deduction on Interest Income-Senior Citizens")]
            [EnumMember(Value = "R")]
            ReasonsCode30,
            [Description("N-No deduction-clause(iii, iv or v)-Section 194N")]
            [EnumMember(Value = "N")]
            ReasonsCode31,
            [Description("D-No/Lower deduction on account of notification-Section 194A(5)")]
            [EnumMember(Value = "D")]
            ReasonsCode32,
            [Description("O-No deduction as per provisions Section 194LBA(2A)")]
            [EnumMember(Value = "O")]
            ReasonsCode33,
            [Description("M-No deduction/lower deduction on account of notification issued under second proviso to section 194N")]
            [EnumMember(Value = "M")]
            ReasonsCode34,
            [Description("E-No deduction on payment being made to a person")]
            [EnumMember(Value = "E")]
            ReasonsCode35,
            [Description("P-No deduction-payment of dividend made to a business trust")]
            [EnumMember(Value = "P")]
            ReasonsCode36,
            [Description("Q-No deduction-payment made to an entity referred to in Section 194A(3)(x)")]
            [EnumMember(Value = "Q")]
            ReasonsCode37,
            [Description("U-Higher rate in view of sec 206AB for non-filing of return")]
            [EnumMember(Value = "U")]
            ReasonsCode38,
        }

        public enum ReasonsCode24Q
        {
            [Description("A-Lower Deduction u/s 197")]
            [EnumMember(Value = "A")]
            ReasonsCode60,
            [Description("B-No deduction u/s 197")]
            [EnumMember(Value = "B")]
            ReasonsCode61,
            [Description("C-Higher Rate (Valid PAN not available)")]
            [EnumMember(Value = "C")]
            ReasonsCode62,
        }

        public enum Role
        {
            SuperAdmin = 1,
            Admin = 2
        }
    }
}

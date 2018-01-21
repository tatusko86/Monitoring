using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorCMD.Regedits;

namespace monitorCMD
{
    class Counterparty
    {
        public Counterparty(string valueHtml,string edrpou)
        {
            this.valueHtml = valueHtml;
            this.edrpou = edrpou;
        }
        public int id { get; set; }
        public string fullName { get; set; }
        public string smallName { get; set; }
        public string edrpou { get; set; }
        public string address { get; set; }
        public string director { get; set; }
        public string status { get; set; }
        public string qved { get; set; }
        public string contact { get; set; }

        //-----------------------------
        public List<Founder>founder = new List<Founder>();
        //---------------------------------------
        public string pdv { get; set; }
        public DateTime sud { get; set; }
        public string planPerevirki { get; set; }
        public string podatBorg { get; set; }
        public int viconavchiProvadgenya { get; set; }
        //---------------------------------------------
        public DateTime dalaLoad { get; set; }
        public DateTime dataUpdate { get; set; }
        string valueHtml = null;
        public bool flag;
        private string parser(string pathStart,string pathFin)
        {
            int indexStart = valueHtml.IndexOf(pathStart);
            if (indexStart == -1) { return ""; }
            int indexFinish = valueHtml.IndexOf(pathFin, indexStart);
            
            string result = valueHtml.Substring(indexStart,indexFinish-indexStart);
            result = result.Replace(pathStart, "");
            return result;
        }

        private void parsFounder()
        {
            string result = string.Empty;
            int indexStart = valueHtml.IndexOf("Власники");
            if (indexStart != -1)
            {
                int indexFinish = valueHtml.IndexOf("</h3>", indexStart);
                int indexlast = valueHtml.IndexOf("<h3>", indexFinish);
                result = valueHtml.Substring(indexFinish, indexlast - indexFinish);
                result = result.Replace("\n", "");
                result = result.Replace("<p>", " ");
                result = result.Replace("</b>", "");
                result = result.Replace("<b>", "");
                result = result.Replace("</h3>", " ");
                result = result.Replace("</p>", "*");
                result = result.Replace("     ", " ");
                result = result.Replace("    ", " ");
                result = result.Replace("   ", " ");
                result = result.Replace("  ", " ");
                result = result.Replace("  ", " ");
                string[] mass = result.Split('*');
                mass = mass.Where(x => x != "").ToArray();
                mass = mass.Where(x => x != " ").ToArray();
                foreach (string s in mass)
                {
                    int index = s.IndexOf("БЕНЕФ");
                    if (index == -1)
                    {
                        string[] m = s.Split(',');
                        Founder f = new Founder();
                        if (m.Length == 2)
                        {
                            f.name = m[0];
                            f.summ = m[1];
                            f.chast = "";
                            f.status = "Засновник";
                        }
                        else if (m.Length == 1)
                        {
                            f.name = m[0];
                            f.summ = "";
                            f.chast = "";
                            f.status = "Засновник";
                        }
                        else
                        {
                            f.name = m[0];
                            f.summ = m[1];
                            f.chast = m[2];
                            f.status = "Засновник";
                            founder.Add(f);
                        }

                    }
                    else
                    {
                        Founder f = new Founder();
                        f.status = "Бенефіціар";
                        f.name = s;
                        f.chast = "";
                        f.summ = "";
                        founder.Add(f);
                    }
                }
            }
           

        }
        private string parsBorgPDV(string start)
        {
            string result = string.Empty;
            int indexStart = valueHtml.IndexOf(start);
            if (indexStart == -1) { return ""; }
            int indexFinish = valueHtml.IndexOf("<p>", indexStart);
            int indexlast = valueHtml.IndexOf("</p>", indexFinish);
            result = valueHtml.Substring(indexFinish, indexlast - indexFinish);
            result = result.Replace("\n", "");
            result = result.Replace("<p>", " ");
            result = result.Replace("</b>", "");
            result = result.Replace("<b>", "");
            result = result.Replace("</h3>", " ");
            result = result.Replace("</p>", "*");
            result = result.Replace("     ", " ");
            result = result.Replace("    ", " ");
            result = result.Replace("   ", " ");
            result = result.Replace("  ", " ");
            result = result.Replace("  ", " ");
            return result;
            
        }
        public Counterparty chekInfo()
        {
            Counterparty con = null;
            fullName = parser("Повна назва:","<");
            smallName = parser("<h2>", "</h2>");
            address = parser("Адреса:", "<");
            director = parser("Директор:", "<");
            status = parser("Стан:", "<");
            qved = parser("Вид діяльністі:", "<");
            parsFounder();
            podatBorg = parsBorgPDV("<h3>Податковий борг</h3>"); 
            pdv = parsBorgPDV("<h3>Платник ПДВ</h3>");

            InBlank inblank = new InBlank();
            sud = inblank.getLostDate(edrpou);
            //sud = inblank.getCount(edrpou); // заполняем поля информацией с реестра судебных решений(количество судебных решений)/// Сейчас берем по количеству записей в реестре судебных решений
            //а надо сделать чтобы по дате!!! Пишем дату в БД и проверяем потом

            DBconnector dbcon = new DBconnector(); // add new info from OPENDATA
             Counterparty counterparty = new Counterparty("","");

            counterparty.fullName = fullName;
            counterparty.smallName = smallName;
            counterparty.address = address;
            counterparty.qved = qved;
            counterparty.director = director;
            counterparty.status = status;
            counterparty.pdv = pdv;
            counterparty.podatBorg = podatBorg;
            counterparty.sud = sud;
            counterparty.edrpou = edrpou;
            counterparty.founder = founder;

            flag = dbcon.setNewInformationsFromOpenData(counterparty);

            if (!flag)
            {
             con = getConterpartyInDB(edrpou);
            }

            return con;
        }
        private Counterparty getConterpartyInDB(string edrpou)
        {
            
            Counterparty con;

            DBconnector db = new DBconnector();
            con = db.getInfoOpendata(edrpou);

            return con;

        }
    }

    
}

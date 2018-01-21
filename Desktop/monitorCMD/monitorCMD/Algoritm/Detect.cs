using monitorCMD.Regedits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorCMD
{
    class Detect
    {
       

        string resultToEmail;
        public void mechingConterparty(Counterparty _new, Counterparty old)
        {
            
            bool flagNewLoadInfo = false;

            if (_new.fullName.Length != old.fullName.Length)
            {
                flagNewLoadInfo = true;

                

                resultToEmail = "\r\n\r\nВідбулася зміна назви:------> Була назва: " + old.fullName+"-------> Нова назва: "+_new.fullName;
            }

            if (_new.smallName != old.smallName)
            {
                flagNewLoadInfo = true;

                

                resultToEmail += "\r\n\r\nВідбулася зміна назви:------> Була назва: " + old.smallName + "-------> Нова назва: " + _new.smallName;
            }

            if (_new.address != old.address)
            {
                flagNewLoadInfo = true;

               

                resultToEmail += "\r\n\r\nВідбулася зміна адреси:------> Була адреса: " + old.address + "-------> Нова адреса: " + _new.address;
            }

            if (_new.qved != old.qved)
            {
                flagNewLoadInfo = true;

                

                resultToEmail += "\r\n\r\nВідбулася зміна основного КВЕД:------> Був квед: " + old.qved + "-------> Новий квед: " + _new.qved;
            }

            if (_new.director != old.director)
            {
                flagNewLoadInfo = true;

                resultToEmail += "\r\n\r\nВідбулася зміна директора:------> Колишній директор: " + old.director + "-------> Новий директор: " + _new.director;
            }
            if (_new.status != old.status)
            {
                flagNewLoadInfo = true;

                resultToEmail += "\r\n\r\nВідбулася зміна стану:------> Був стан: " + old.status + "-------> Новый стан: " + _new.status;
            }
            //-----------------------------
            if (_new.podatBorg != old.podatBorg)
            {
                if (old.podatBorg == "")
                {
                    resultToEmail += "\r\n\r\n Наявна інформація щодо Податкового боргу - " + _new.podatBorg;
                }
                else
                {
                    resultToEmail += "\r\n\r\nВідбулася зміна стану подоткового боргу:------> Був борг: " + old.podatBorg + "-------> Став борг: " + _new.podatBorg;
                }
                flagNewLoadInfo = true;
            }


            string[] massPDVnew = _new.pdv.Split(',');
            string[] massPDVold = old.pdv.Split(',');

            if (massPDVnew[0]!= massPDVold[0])
            {
                if (old.pdv=="")
                {
                    resultToEmail += "\r\n\r\n Наявна інформація щодо реестрації свідоцтва ПДВ - " + _new.pdv;
                }
                else
                {
                    resultToEmail += "\r\n\r\nВідбулася зміна стану реестрації свідоцтва ПДВ:------> Був : " + old.pdv + "-------> Став : " + _new.pdv;
                }
                flagNewLoadInfo = true;
            }

            if (_new.founder.Count != 0||old.founder.Count != 0)
            {
                if (_new.founder.Count != 0 & old.founder.Count == 0)
                {
                    string fonder = "Нові засновники:\r\n\r\n";
                    foreach (Founder s in _new.founder)
                    {

                        fonder += "\r\n" + s.name + " " + s.status + " " + s.summ + " " + s.chast;

                    }
                    resultToEmail += "\r\n\r\n" + fonder;
                    flagNewLoadInfo = true;
                }
                if (_new.founder.Count == 0 & old.founder.Count != 0)
                {
                    

                    
                }
                if (_new.founder.Count != 0 & old.founder.Count != 0)
                {
                    Validator val = new Validator();
                    string newFounders = "";
                    string oldFounders = "";

                    List<Founder> newFolder = val.getValidFouders(_new , old);
                    List<Founder> oldFounder = val.getValidFouders(old, _new);

                    if (newFolder.Count!=0)
                    {
                        string fonder = "\r\n\r\nНові засновники:";
                        foreach (Founder s in newFolder)
                        {

                            fonder += "\r\n" + s.name + " " + s.status + " " + s.summ + " " + s.chast;

                        }
                        resultToEmail += "\r\n\r\n" + fonder;
                        flagNewLoadInfo = true;
                    }

                    if (oldFounder.Count!=0)
                    {
                        string fonder = "\r\n\r\nЗасновники які вишли зі складу:";
                        foreach (Founder s in oldFounder)
                        {

                            fonder += "\r\n" + s.name + " " + s.status + " " + s.summ + " " + s.chast;

                        }
                        resultToEmail += "\r\n\r\n" + fonder;
                        flagNewLoadInfo = true;
                    }

                        //flagNewLoadInfo = true;
                }
            }

            if (_new.sud != old.sud)
            {
                
                string result = "\r\n\r\nНові судові рішення:\r\n\r\n" + bildSud(_new.agent, old.sud, old.edrpou);
                resultToEmail += "\r\n\r\n" + result;
                flagNewLoadInfo = true;
            }


            //-----------------------------
            if (flagNewLoadInfo == false)
            {
                DBconnector db = new DBconnector();
                db.setUpdateDataOpendata(_new.edrpou); // Обновляем дату обновления в таблице ОпенДата
                db.setUpdateDateEdrpou(_new.edrpou);   // Обновляем дату обновления в таблице Контрагент
            }
            else
            {
                DBconnector db = new DBconnector();
                Mail mail = new Mail();

                string[] mass = db.getMails(_new.edrpou).ToArray();// получаем все имейлы клиентов которые указали мониторинг по данному предприятию

                foreach(string s in mass)// отправляем имейлы
                {
                    mail.getMail(s,_new.smallName,resultToEmail, null, "counterparty.info@gmail.com", "Grom5657184");
                }

                db.setInformationsFromOpenDataAndFounders(_new); // подливкa новых данных в таблицу ОПЕНДАТА

                db.setUpdateDateEdrpou(_new.edrpou);// Обновляем дату обновления в таблице Контрагент
                //-------------------------------------------
            }

        }
        private string bildSud(Agent[] inBlank, DateTime data,string okpo)
        {
            Agent[] agent;
            try
            {
                agent = inBlank;
            }
            catch
            {
                agent = new Agent[1];
                return "Єдиний державний реєстр судових рішень тимчасово недучтупний, повторіть запит через декілька хвилин";
            }

            string result;
            if (agent[0].Blank == null)
            { result = "Судові рішення відсутні"; }
            else
            {
                result = "⚖️\r\nСтаном на <b>" + DateTime.Now + "</b> у судовому реєстрі за запитом <b>" + okpo + "</b> знайдено <b>" + agent[0].Blank.Length + "</b> документів \r\n\r\n\r\n";
                for (int i = 0; i < agent[0].Blank.Length; i++)
                {
                    string dd = data.ToString("dd.MM.yyyy");
                    if (dd == agent[0].Blank[i].Data) { break; }
                    try
                    {
                        result += "<b>" + agent[0].Blank[i].sud + "</b>\r\n" +
                             "Дата: " + agent[0].Blank[i].Data + "\r\n" +
                             "Тип: " + agent[0].Blank[i].TypeRisheniya + "\r\n" +
                             "Справа: " + agent[0].Blank[i].Number + "\r\n" +
                             "Форма: " + agent[0].Blank[i].Type + "\r\n" +
                             "<a href=\"http://www.reyestr.court.gov.ua" + agent[0].Blank[i].Href + "\">Відкрити " + agent[0].Blank[i].TypeRisheniya.Replace("Ухвала", "Ухвалу") + "</a>\r\n\r\n\r\n";
                    }
                    catch { break; }

                }

                result += "<a href=\"http://www.reyestr.court.gov.ua\">Переглянути всі записи за даним підприємством</a>";
            }

            return result;
        }
    }
}

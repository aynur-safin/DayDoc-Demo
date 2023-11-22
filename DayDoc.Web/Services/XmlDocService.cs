using DayDoc.Web.Data;
using DayDoc.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DayDoc.Web.Services
{
    public class XmlDocService
    {
        private static readonly string APP_DATA_DIR = "App_Data";

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public XmlDocService(AppDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetXmlFilePath(int docId, string fileName)
        {
            string xmlDir = Path.Combine(_webHostEnvironment.ContentRootPath, APP_DATA_DIR, "xml", "Akt", docId.ToString());
            Directory.CreateDirectory(xmlDir);
            return Path.Combine(xmlDir, fileName);
        }

        public async Task<List<XmlDoc>?> GetXmlDocList(int docId)
        {
            var xmlDocs = await _db.XmlDocs.AsNoTracking()
                .Where(x => x.DocId == docId)
                .OrderByDescending(x => x.DateAndTime)
                .ToListAsync();

            return xmlDocs;
        }

        public async Task<XmlDoc?> CreateXmlDoc(int docId)
        {
            var doc = await _db.Docs.AsNoTracking()
                .Include(m => m.OwnCompany)
#nullable disable
                    .ThenInclude(m => m.EdoCompany)
#nullable restore
                .Include(m => m.Contragent)
                .FirstOrDefaultAsync(m => m.Id == docId);

            //_ = doc ?? throw new ArgumentNullException(nameof(doc));
            if (doc == null)
            {
                return null;
            }

            /* OwnCompany */
            {
                _ = doc.OwnCompany ?? throw new ArgumentNullException(nameof(doc.OwnCompany));

                if (string.IsNullOrWhiteSpace(doc.OwnCompany.Name))
                    throw new ArgumentNullException(nameof(doc.OwnCompany.Name));

                if (string.IsNullOrWhiteSpace(doc.OwnCompany.INN))
                    throw new ArgumentNullException(nameof(doc.OwnCompany.INN));

                if (doc.OwnCompany.INN.Length != 12) // У ИП ИНН - 12 знаков                
                {
                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.KPP))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.KPP));
                }

                if (string.IsNullOrWhiteSpace(doc.OwnCompany.Address))
                    throw new ArgumentNullException(nameof(doc.OwnCompany.Address));

                if (string.IsNullOrWhiteSpace(doc.OwnCompany.OGRN))
                    throw new ArgumentNullException(nameof(doc.OwnCompany.OGRN));

                if (!doc.OwnCompany.OGRN_Date.HasValue)
                    throw new ArgumentNullException(nameof(doc.OwnCompany.OGRN_Date));

                if (string.IsNullOrWhiteSpace(doc.OwnCompany.EdoId))
                    throw new ArgumentNullException(nameof(doc.OwnCompany.EdoId));

                /* OwnCompany.Boss */
                {
                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.Signatory_FirstName))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.Signatory_FirstName));

                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.Signatory_MiddleName))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.Signatory_MiddleName));

                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.Signatory_LastName))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.Signatory_LastName));
                }

                /* OwnCompany.EdoCompany */
                {
                    _ = doc.OwnCompany.EdoCompany ?? throw new ArgumentNullException(nameof(doc.OwnCompany.EdoCompany));

                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.EdoCompany.Name))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.EdoCompany.Name));

                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.EdoCompany.INN))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.EdoCompany.INN));

                    if (string.IsNullOrWhiteSpace(doc.OwnCompany.EdoCompany.KPP))
                        throw new ArgumentNullException(nameof(doc.OwnCompany.EdoCompany.KPP));
                }
            }

            /* Contragent */
            {
                _ = doc.Contragent ?? throw new ArgumentNullException(nameof(doc.Contragent));

                if (string.IsNullOrWhiteSpace(doc.Contragent.Name))
                    throw new ArgumentNullException(nameof(doc.Contragent.Name));

                if (string.IsNullOrWhiteSpace(doc.Contragent.INN))
                    throw new ArgumentNullException(nameof(doc.Contragent.INN));

                /*
                if (string.IsNullOrWhiteSpace(doc.Contragent.KPP))
                    throw new ArgumentNullException(nameof(doc.Contragent.KPP));
                */

                if (string.IsNullOrWhiteSpace(doc.Contragent.Address))
                    throw new ArgumentNullException(nameof(doc.Contragent.Address));

                if (string.IsNullOrWhiteSpace(doc.Contragent.EdoId))
                    throw new ArgumentNullException(nameof(doc.Contragent.EdoId));

                if (doc.Contragent.EdoCompanyId == null)
                    throw new ArgumentNullException(nameof(doc.Contragent.EdoCompanyId));

                if(doc.Contragent.INN.Length == 12) // У ИП ИНН - 12 знаков
                {
                    if (string.IsNullOrWhiteSpace(doc.Contragent.Signatory_FirstName))
                        throw new ArgumentNullException(nameof(doc.Contragent.Signatory_FirstName));

                    if (string.IsNullOrWhiteSpace(doc.Contragent.Signatory_MiddleName))
                        throw new ArgumentNullException(nameof(doc.Contragent.Signatory_MiddleName));

                    if (string.IsNullOrWhiteSpace(doc.Contragent.Signatory_LastName))
                        throw new ArgumentNullException(nameof(doc.Contragent.Signatory_LastName));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(doc.Contragent.KPP))
                        throw new ArgumentNullException(nameof(doc.Contragent.KPP));
                }
            }

            if (string.IsNullOrEmpty(doc.WorkName))
                throw new ArgumentNullException(nameof(doc.WorkName));

            decimal docSum = decimal.Round(doc.Sum * 1.00m, 2);
            //docSum += 0.00m; // Устанавливаем Precision c двумя цифрами после запятой, для корректной сериализации в XML, иначе сериализуется без копеек

            const string verProg = "DayDoc 0.1";
            XmlSerializer xmlSerializer;
            object? xmlObject;
            string fileId = $"{doc.OwnCompany.EdoId}_{doc.Contragent.EdoId}_{doc.Date:yyyyMMdd}_{Guid.NewGuid():D}";

            if (doc.DocType == DocType.Akt)
            {
                xmlSerializer = new XmlSerializer(typeof(Objects.Xsd.Act.DP_REZRUISP.Файл));
                fileId = "DP_REZRUISP" + "_" + fileId;

                Objects.Xsd.Act.DP_REZRUISP.Файл xmlClass = new()
                {
                    ИдФайл = fileId,
                    ВерсПрог = verProg,
                    ВерсФорм = Objects.Xsd.Act.DP_REZRUISP.ФайлВерсФорм.Item502,
                    СвУчДокОбор = new Objects.Xsd.Act.DP_REZRUISP.ФайлСвУчДокОбор
                    {
                        ИдОтпр = doc.OwnCompany.EdoId,
                        ИдПол = doc.Contragent.EdoId,
                        СвОЭДОтпр = new Objects.Xsd.Act.DP_REZRUISP.ФайлСвУчДокОборСвОЭДОтпр
                        {
                            ИдЭДО = doc.OwnCompany.EdoId.Substring(0, 3),
                            НаимОрг = doc.OwnCompany.EdoCompany.Name,
                            ИННЮЛ = doc.OwnCompany.EdoCompany.INN
                        }
                    },
                    Документ = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокумент()
                    {
                        КНД = Objects.Xsd.Act.DP_REZRUISP.ФайлДокументКНД.Item1175012,
                        ДатаИнфИсп = doc.Date.ToString("dd.MM.yyyy"),
                        ВремИнфИсп = (new DateTime()).ToString("HH.mm.ss"), //doc.Date.ToString("HH.mm.ss"),
                        НаимЭконСубСост = doc.OwnCompany.Name,
                        СвДокПРУ = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУ
                        {
                            НаимДок = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУНаимДок
                            {
                                ПоФактХЖ = Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУНаимДокПоФактХЖ.ДокументопередачерезультатовработДокументобоказанииуслуг,
                                НаимДокОпр = "Акт приема-передачи"
                            },
                            ИдентДок = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУИдентДок
                            {
                                НомДокПРУ = doc.Num,
                                ДатаДокПРУ = doc.Date.ToString("dd.MM.yyyy")
                            },
                            ИспрДокПРУ = null, //new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУИспрДокПРУ { }
                            ДенИзм = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУДенИзм
                            {
                                КодОКВ = "643",
                                НаимОКВ = "Российский рубль"
                            },
                            СодФХЖ1 = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1
                            {
                                ЗагСодОпер =
                                    "Мы, нижеподписавшиеся, представитель ИСПОЛНИТЕЛЯ, с одной стороны и представитель ЗАКАЗЧИКА с другой стороны, " +
                                    "составили настоящий акт в том, что ИСПОЛНИТЕЛЬ выполнил, а ЗАКАЗЧИК принял следующие работы (услуги)",

                                Исполнитель = new Objects.Xsd.Act.DP_REZRUISP.УчастникТип
                                {
                                    ИдСв = new Objects.Xsd.Act.DP_REZRUISP.УчастникТипИдСв
                                    {
                                        /* !!! Варианты !!! */
                                        Item
                                        = doc.OwnCompany.INN.Length == 12 // У ИП ИНН - 12 знаков
                                        ? new Objects.Xsd.Act.DP_REZRUISP.СвИПТип
                                        {
                                            ИННФЛ = doc.OwnCompany.INN,
                                            ФИО = new Objects.Xsd.Act.DP_REZRUISP.ФИОТип
                                            {
                                                Фамилия = doc.OwnCompany.Signatory_LastName,
                                                Имя = doc.OwnCompany.Signatory_FirstName,
                                                Отчество = doc.OwnCompany.Signatory_MiddleName
                                            },
                                            СвГосРегИП = $"ОГРНИП: {doc.OwnCompany.OGRN} от {doc.OwnCompany.OGRN_Date?.ToString("dd.MM.yyyy")}", //"ОГРНИП: 123456789012345 от 21.02.2005 г.",
                                        }
                                        : new Objects.Xsd.Act.DP_REZRUISP.УчастникТипИдСвСвОрг
                                        {
                                            Item = new Objects.Xsd.Act.DP_REZRUISP.УчастникТипИдСвСвОргСвЮЛ
                                            {
                                                НаимОрг = doc.OwnCompany.Name,
                                                ИННЮЛ = doc.OwnCompany.INN,
                                                КПП = doc.OwnCompany.KPP
                                            }
                                        }
                                    },
                                    Адрес = new Objects.Xsd.Act.DP_REZRUISP.АдресТип
                                    {
                                        /* !!! Варианты !!! */
                                        //Item = new Objects.Xsd.Act.DP_REZRUISP.АдрРФТип
                                        //{
                                        //    Индекс = "",
                                        //    КодРегион = "", // https://www.consultant.ru/document/cons_doc_LAW_181028/0660ba06bb726586e23236f2be0d5a5713820cc8/#dst101160
                                        //    Город = "",
                                        //    Улица = "",
                                        //    Дом = "",
                                        //    Кварт = ""
                                        //}                                                
                                        Item = new Objects.Xsd.Act.DP_REZRUISP.АдрИнфТип
                                        {
                                            КодСтр = "643", // 643 = Россия, https://normativ.kontur.ru/document?moduleId=1&documentId=25234#l0
                                            АдрТекст = doc.OwnCompany.Address
                                        }
                                    },
                                },
                                Заказчик = new Objects.Xsd.Act.DP_REZRUISP.УчастникТип
                                {
                                    ИдСв = new Objects.Xsd.Act.DP_REZRUISP.УчастникТипИдСв
                                    {
                                        /* !!! Варианты !!! */
                                        Item
                                        = doc.Contragent.INN.Length == 12 // У ИП ИНН - 12 знаков
                                        ? new Objects.Xsd.Act.DP_REZRUISP.СвИПТип
                                        {
                                            ИННФЛ = doc.Contragent.INN,
                                            ФИО = new Objects.Xsd.Act.DP_REZRUISP.ФИОТип
                                            {
                                                Фамилия = doc.Contragent.Signatory_LastName,
                                                Имя = doc.Contragent.Signatory_FirstName,
                                                Отчество = doc.Contragent.Signatory_MiddleName,
                                            }
                                            // , СвГосРегИП = $"ОГРНИП: {doc.Contragent.OGRN} от {doc.Contragent.OGRN_Date?.ToString("dd.MM.yyyy")}", //"ОГРНИП: 123456789012345 от 21.02.2005 г.",
                                        }
                                        : new Objects.Xsd.Act.DP_REZRUISP.УчастникТипИдСвСвОрг
                                        {
                                            Item = new Objects.Xsd.Act.DP_REZRUISP.УчастникТипИдСвСвОргСвЮЛ
                                            {
                                                НаимОрг = doc.Contragent.Name,
                                                ИННЮЛ = doc.Contragent.INN,
                                                КПП = doc.Contragent.KPP
                                            }
                                        } 
                                    },
                                    Адрес = new Objects.Xsd.Act.DP_REZRUISP.АдресТип
                                    {
                                        /* !!! Варианты !!! */
                                        //Item = new Objects.Xsd.Act.DP_REZRUISP.АдрРФТип
                                        //{
                                        //    Индекс = "",
                                        //    КодРегион = "",
                                        //    Город = "",
                                        //    Улица = "",
                                        //    Дом = ""
                                        //}
                                        Item = new Objects.Xsd.Act.DP_REZRUISP.АдрИнфТип
                                        {
                                            КодСтр = "643", // 643 = Россия, https://normativ.kontur.ru/document?moduleId=1&documentId=25234#l0
                                            АдрТекст = doc.Contragent.Address
                                        }
                                    },
                                },
                                Основание = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1Основание[]
                                {
                                            new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1Основание
                                            {
                                                НаимОсн = !string.IsNullOrWhiteSpace(doc.Dogovor_Num) ? "Договор/Счёт" : "Без документа-основания",
                                                НомОсн = !string.IsNullOrWhiteSpace(doc.Dogovor_Num) ? doc.Dogovor_Num : null,
                                                ДатаОсн = !string.IsNullOrWhiteSpace(doc.Dogovor_Num) ? doc.Dogovor_Date?.ToString("dd.MM.yyyy") : null,
                                                //ДопСвОсн = ""
                                            }
                                },
                                ОписРабот = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1ОписРабот[]
                                {
                                            new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1ОписРабот
                                            {
                                                СтБезНДСИт = docSum, СтБезНДСИтSpecified = true,
                                                СумНДСИт = 0.00m, СумНДСИтSpecified = true,
                                                СтУчНДСИт = docSum,
                                                Работа = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1ОписРаботРабота[]
                                                {
                                                    new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1ОписРаботРабота
                                                    {
                                                        Номер = "1",
                                                        НаимРабот = doc.WorkName,
                                                        //Описание = doc.Description,
                                                        НаимЕдИзм = "шт",
                                                        ОКЕИ = "796",
                                                        Цена = docSum, ЦенаSpecified = true,
                                                        Количество = 1, КоличествоSpecified = true,
                                                        СтоимБезНДС = docSum, СтоимБезНДСSpecified = true,
                                                        НалСт = Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1ОписРаботРаботаНалСт.безНДС, НалСтSpecified = true,
                                                        СумНДС = 0.00m, СумНДСSpecified = true,
                                                        СтоимУчНДС = docSum, СтоимУчНДСSpecified = true,
                                                        //ИнфПолеОписРабот = new Objects.Xsd.Act.DP_REZRUISP.ТекстИнфТип[]
                                                        //{
                                                        //    new Objects.Xsd.Act.DP_REZRUISP.ТекстИнфТип
                                                        //    {
                                                        //        Идентиф = "552 стр",
                                                        //        Значен = "Информационное поле строки"
                                                        //    }
                                                        //}                                                    
                                                    }
                                                }
                                            }
                                },
                                //ИнфПолФХЖ1 = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСвДокПРУСодФХЖ1ИнфПолФХЖ1
                                //{
                                //    ТекстИнф = new Objects.Xsd.Act.DP_REZRUISP.ТекстИнфТип[]
                                //    {
                                //        new Objects.Xsd.Act.DP_REZRUISP.ТекстИнфТип
                                //        {
                                //            Идентиф = "552",
                                //            Значен = "Информационное поле документа"
                                //        }
                                //    }
                                //}
                            }
                        },
                        СодФХЖ2 = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументСодФХЖ2
                        {
                            СодОпер = "Результаты работ переданы (услуги оказаны)"
                        },
                        Подписант = new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументПодписант[]
                        {
                                    /*
                                     * Полномочия подписанта: рекомендации по заполнению
                                     * https://support.kontur.ru/pages/viewpage.action?pageId=83856418
                                     * */
                                    new Objects.Xsd.Act.DP_REZRUISP.ФайлДокументПодписант
                                    {
                                        /*
                                            Принимает значение: 1 | 2 | 3, где:
                                            1 - лицо, совершившее сделку, операцию;
                                            2 - лицо, совершившее сделку, операцию и ответственное за ее оформление;
                                            3 - лицо, ответственное за оформление свершившегося события                                     
                                        */
                                        ОблПолн = Objects.Xsd.Act.DP_REZRUISP.ФайлДокументПодписантОблПолн.Item2, /* 2 */

                                        /*
                                            Принимает значение: 1 | 2 | 3 | 4, где:
                                            1 - Работник организации - исполнителя работ (услуг);
                                            2 - Работник организации - составителя информации исполнителя;
                                            3 - Работник иной уполномоченной организации;
                                            4 - Уполномоченное физическое лицо (в том числе индивидуальный предприниматель)
                                        */
                                        Статус = Objects.Xsd.Act.DP_REZRUISP.ФайлДокументПодписантСтатус.Item4, /* 4 */
                                        ОснПолнПодп = doc.OwnCompany.OGRN, /* для ИП - ОГРНИП */
                                        /* !!! Варианты !!! */
                                        Item = new Objects.Xsd.Act.DP_REZRUISP.СвИПТип
                                        {
                                            ИННФЛ = doc.OwnCompany.INN,
                                            ФИО = new Objects.Xsd.Act.DP_REZRUISP.ФИОТип
                                            {
                                                Фамилия = doc.OwnCompany.Signatory_LastName,
                                                Имя = doc.OwnCompany.Signatory_FirstName,
                                                Отчество = doc.OwnCompany.Signatory_MiddleName
                                            },
                                            СвГосРегИП = $"ОГРНИП: {doc.OwnCompany.OGRN} от {doc.OwnCompany.OGRN_Date?.ToString("dd.MM.yyyy")}", //"ОГРНИП: 123456789012345 от 21.02.2005 г.",
                                        }
                                    }
                        }
                    }
                };
                xmlObject = xmlClass;
            }
            else //if(doc.DocType == DocType.UPD)
            {
                xmlSerializer = new XmlSerializer(typeof(Objects.Xsd.Upd.ON_NSCHFDOPPR.Файл));
                fileId = "ON_NSCHFDOPPR" + "_" + fileId;

                Objects.Xsd.Upd.ON_NSCHFDOPPR.Файл xmlClass = new()
                {
                    ИдФайл = fileId,
                    ВерсПрог = verProg,
                    ВерсФорм = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлВерсФорм.Item501,
                    СвУчДокОбор = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлСвУчДокОбор
                    {
                        ИдОтпр = doc.OwnCompany.EdoId,
                        ИдПол = doc.Contragent.EdoId,
                        СвОЭДОтпр = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлСвУчДокОборСвОЭДОтпр
                        {
                            ИдЭДО = doc.OwnCompany.EdoId.Substring(0, 3),
                            НаимОрг = doc.OwnCompany.EdoCompany.Name,
                            ИННЮЛ = doc.OwnCompany.EdoCompany.INN
                        }
                    },
                    Документ = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокумент()
                    {
                        КНД = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументКНД.Item1115131,
                        Функция = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументФункция.ДОП,
                        ПоФактХЖ = "Документ об отгрузке товаров (выполнении работ), передаче имущественных прав (документ об оказании услуг)",
                        НаимДокОпр = "Акт передачи прав на использование ПО", // "Акт приема-передачи прав на использование ПО", "Акт на передачу прав",
                        ДатаИнфПр = doc.Date.ToString("dd.MM.yyyy"),
                        ВремИнфПр = (new DateTime()).ToString("HH.mm.ss"), //doc.Date.ToString("HH.mm.ss"),
                        НаимЭконСубСост = doc.OwnCompany.Name,
                        СвСчФакт = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФакт
                        {
                            НомерСчФ = doc.Num,
                            ДатаСчФ = doc.Date.ToString("dd.MM.yyyy"),
                            КодОКВ = "643",
                            //НаимОКВ = "Российский рубль",
                            //ИспрСчФ = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактИспрСчФ { },
                            СвПрод = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТип[]
                            {
                                        new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТип
                                        {
                                            //ОКПО = "0062753258",
                                            //СтруктПодр = "Основной склад",
                                            //ИнфДляУчаст = "",
                                            КраткНазв = doc.OwnCompany.Name,
                                            ИдСв = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТипИдСв
                                            {
                                                /* !!! Варианты !!! */
                                                Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.СвИПТип
                                                {
                                                    ИННФЛ = doc.OwnCompany.INN,
                                                    ФИО = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФИОТип
                                                    {
                                                        Фамилия = doc.OwnCompany.Signatory_LastName,
                                                        Имя = doc.OwnCompany.Signatory_FirstName,
                                                        Отчество = doc.OwnCompany.Signatory_MiddleName
                                                    },
                                                    СвГосРегИП = $"ОГРНИП: {doc.OwnCompany.OGRN} от {doc.OwnCompany.OGRN_Date?.ToString("dd.MM.yyyy")}", //"ОГРНИП: 123456789012345 от 21.02.2005 г.",
                                                }
                                            },
                                            Адрес = new Objects.Xsd.Upd.ON_NSCHFDOPPR.АдресТип
                                            {
                                                /* !!! Варианты !!! */
                                                //Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.АдрРФТип
                                                //{
                                                //    Индекс = "",
                                                //    КодРегион = "", // https://www.consultant.ru/document/cons_doc_LAW_181028/0660ba06bb726586e23236f2be0d5a5713820cc8/#dst101160
                                                //    Город = "",
                                                //    Улица = "",
                                                //    Дом = "",
                                                //    Кварт = ""
                                                //}
                                                Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.АдрИнфТип
                                                {
                                                    КодСтр = "643", // 643 = Россия, https://normativ.kontur.ru/document?moduleId=1&documentId=25234#l0
                                                    АдрТекст = doc.OwnCompany.Address
                                                }
                                            },
                                            //Контакт = new Objects.Xsd.Upd.ON_NSCHFDOPPR.КонтактТип
                                            //{
                                            //    Тлф = "+7 987 6543210",
                                            //    ЭлПочта = "test@test.com"
                                            //},
                                            //БанкРекв = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТипБанкРекв { },
                                        }
                            },
                            //ГрузОт = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактГрузОт[]
                            //{
                            //    new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактГрузОт
                            //    {
                            //        /* !!! Варианты !!! */
                            //        Item = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактГрузОтОнЖе.онже
                            //    }
                            //},
                            //ГрузПолуч = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТип[] { },
                            //СвПРД = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактСвПРД { },
                            СвПокуп = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТип[]
                            {
                                        new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТип
                                        {
                                            //ОКПО = "0062753258",
                                            //СтруктПодр = "Основной склад",
                                            //ИнфДляУчаст = "",
                                            КраткНазв = doc.Contragent.Name,
                                            ИдСв = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТипИдСв
                                            {
                                                /* !!! Варианты !!! */
                                                Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТипИдСвСвЮЛУч
                                                {
                                                    НаимОрг = doc.Contragent.Name,
                                                    ИННЮЛ = doc.Contragent.INN,
                                                    КПП = doc.Contragent.KPP
                                                }
                                                //Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.СвИПТип
                                                //{
                                                //    ИННФЛ = doc.Contragent.INN,
                                                //    ФИО = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФИОТип
                                                //    {
                                                //        Фамилия = doc.Contragent.Signatory_LastName,
                                                //        Имя = doc.Contragent.Signatory_FirstName,
                                                //        Отчество = doc.Contragent.Signatory_MiddleName,
                                                //    }
                                                //}
                                            },
                                            Адрес = new Objects.Xsd.Upd.ON_NSCHFDOPPR.АдресТип
                                            {
                                                /* !!! Варианты !!! */
                                                //Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.АдрРФТип
                                                //{
                                                //    Индекс = "",
                                                //    КодРегион = "",
                                                //    Город = "",
                                                //    Улица = "",
                                                //    Дом = ""
                                                //}
                                                Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.АдрИнфТип
                                                {
                                                    КодСтр = "643", // 643 = Россия, https://normativ.kontur.ru/document?moduleId=1&documentId=25234#l0
                                                    АдрТекст = doc.Contragent.Address
                                                }
                                            },
                                            //Контакт = new Objects.Xsd.Upd.ON_NSCHFDOPPR.КонтактТип
                                            //{
                                            //    Тлф = "+7 987 6543210",
                                            //    ЭлПочта = "test@test.com"
                                            //},
                                            //БанкРекв = new Objects.Xsd.Upd.ON_NSCHFDOPPR.УчастникТипБанкРекв { },
                                        }
                            },
                            ДопСвФХЖ1 = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактДопСвФХЖ1
                            {
                                НаимОКВ = "Российский рубль"
                            },
                            //ДокПодтвОтгр = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактДокПодтвОтгр[]
                            //{
                            //    new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактДокПодтвОтгр
                            //    {
                            //        НаимДокОтгр = "",
                            //        НомДокОтгр = "",
                            //        ДатаДокОтгр = ""
                            //    }
                            //},
                            //ИнфПолФХЖ1 = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвСчФактИнфПолФХЖ1
                            //{
                            //    //ИдФайлИнфПол = "",
                            //    ТекстИнф = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ТекстИнфТип[]
                            //    {
                            //        new Objects.Xsd.Upd.ON_NSCHFDOPPR.ТекстИнфТип
                            //        {
                            //            Идентиф = "Комментарий",
                            //            Значен = "Акт передачи прав на использование ПО"
                            //        }
                            //    }
                            //}
                        },
                        ТаблСчФакт = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФакт
                        {
                            СведТов = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФактСведТов[]
                            {
                                        new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФактСведТов
                                        {
                                            НомСтр = "1",
                                            НаимТов = doc.WorkName,
                                            ОКЕИ_Тов = "796", // !!! должно соответствовать НаимЕдИзм ниже // 796 = Штука (шт), 876 = Условная единица (усл. ед)
                                            КолТов = 1, КолТовSpecified = true,
                                            ЦенаТов = docSum, ЦенаТовSpecified = true,
                                            СтТовБезНДС = docSum, СтТовБезНДСSpecified = true,
                                            НалСт = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФактСведТовНалСт.безНДС,
                                            СтТовУчНал = docSum, СтТовУчНалSpecified = true,
                                            Акциз = new Objects.Xsd.Upd.ON_NSCHFDOPPR.СумАкцизТип
                                            {
                                                Item = Objects.Xsd.Upd.ON_NSCHFDOPPR.СумАкцизТипБезАкциз.безакциза
                                            },
                                            СумНал = new Objects.Xsd.Upd.ON_NSCHFDOPPR.СумНДСТип
                                            {
                                                Item = Objects.Xsd.Upd.ON_NSCHFDOPPR.СумНДСТипБезНДС.безНДС // Objects.Xsd.Upd.ON_NSCHFDOPPR.СумНДСТипДефНДС.Item
                                            },
                                            ДопСведТов = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФактСведТовДопСведТов
                                            {
                                                /*
                                                    Признак Товар/Работа/Услуга/Право/Иное    
                                                    Принимает значение:
                                                    1 - имущество |
                                                    2 - работа |
                                                    3 - услуга |
                                                    4 - имущественные права |
                                                    5 - иное
                                                 */
                                                //ПрТовРаб = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФактСведТовДопСведТовПрТовРаб.Item4,
                                                НаимЕдИзм = "шт" // !!! см. код ОКЕИ_Тов выше
                                            }
                                        }
                            },
                            ВсегоОпл = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументТаблСчФактВсегоОпл
                            {
                                СтТовБезНДСВсего = docSum,
                                СтТовБезНДСВсегоSpecified = true,
                                СтТовУчНалВсего = docSum,
                                СтТовУчНалВсегоSpecified = true,
                                СумНалВсего = new Objects.Xsd.Upd.ON_NSCHFDOPPR.СумНДСТип
                                {
                                    Item = Objects.Xsd.Upd.ON_NSCHFDOPPR.СумНДСТипБезНДС.безНДС // Objects.Xsd.Upd.ON_NSCHFDOPPR.СумНДСТипДефНДС.Item
                                },
                            }
                        },
                        СвПродПер = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвПродПер
                        {
                            СвПер = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвПродПерСвПер
                            {
                                СодОпер = "Лицензиаром переданы Лицензиату (Пользователю) неисключительные права на использование экземпляров программного обеспечения",
                                //ДатаПер = doc.Date.ToString("dd.MM.yyyy"),
                                ОснПер = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ОснованиеТип[]
                                {
                                            new Objects.Xsd.Upd.ON_NSCHFDOPPR.ОснованиеТип
                                            {
                                                НаимОсн = !string.IsNullOrWhiteSpace(doc.Dogovor_Num) ? "Договор/Счёт" : "Без документа-основания",
                                                НомОсн = !string.IsNullOrWhiteSpace(doc.Dogovor_Num) ? doc.Dogovor_Num : null,
                                                ДатаОсн = !string.IsNullOrWhiteSpace(doc.Dogovor_Num) ? doc.Dogovor_Date?.ToString("dd.MM.yyyy") : null,
                                                //ДопСвОсн = ""
                                            }
                                },
                                СвЛицПер = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвПродПерСвПерСвЛицПер
                                {
                                    Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументСвПродПерСвПерСвЛицПерРабОргПрод
                                    {
                                        Должность = doc.OwnCompany.Signatory_Position,
                                        ФИО = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФИОТип
                                        {
                                            Фамилия = doc.OwnCompany.Signatory_LastName,
                                            Имя = doc.OwnCompany.Signatory_FirstName,
                                            Отчество = doc.OwnCompany.Signatory_MiddleName
                                        }
                                    }
                                }
                            }
                        },
                        Подписант = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументПодписант[]
                        {
                                    /*
                                     * Полномочия подписанта: рекомендации по заполнению
                                     * https://support.kontur.ru/pages/viewpage.action?pageId=83856418
                                     * */
                                    new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументПодписант
                                    {
                                        /*
                                            Область полномочий
                                            Принимает значение:
                                            0 - лицо, ответственное за подписание счетов-фактур |
                                            1 - лицо, совершившее сделку, операцию |
                                            2 - лицо, совершившее сделку, операцию и ответственное за ее оформление |
                                            3 - лицо, ответственное за оформление свершившегося события |
                                            4 - лицо, совершившее сделку, операцию и ответственное за подписание счетов-фактур |
                                            5 - лицо, совершившее сделку, операцию и ответственное за ее оформление и за подписание счетов-фактур |
                                            6 - лицо, ответственное за оформление свершившегося события и за подписание счетов-фактур                                   
                                        */
                                        ОблПолн = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументПодписантОблПолн.Item2, /* 2 */

                                        /*
                                            Статус
                                            Принимает значение:
                                            1 - работник организации продавца товаров (работ, услуг, имущественных прав) |
                                            2 - работник организации - составителя файла обмена информации продавца, если составитель файла обмена информации не является продавцом |
                                            3 - работник иной уполномоченной организации |
                                            4 - уполномоченное физическое лицо (в том числе индивидуальный предприниматель)
                                        */
                                        Статус = Objects.Xsd.Upd.ON_NSCHFDOPPR.ФайлДокументПодписантСтатус.Item4, /* 4 */
                                        ОснПолн = doc.OwnCompany.OGRN, /* для ИП - ОГРНИП */
                                        /* !!! Варианты !!! */
                                        Item = new Objects.Xsd.Upd.ON_NSCHFDOPPR.СвИПТип
                                        {
                                            ИННФЛ = doc.OwnCompany.INN,
                                            ФИО = new Objects.Xsd.Upd.ON_NSCHFDOPPR.ФИОТип
                                            {
                                                Фамилия = doc.OwnCompany.Signatory_LastName,
                                                Имя = doc.OwnCompany.Signatory_FirstName,
                                                Отчество = doc.OwnCompany.Signatory_MiddleName
                                            },
                                            СвГосРегИП = $"ОГРНИП: {doc.OwnCompany.OGRN} от {doc.OwnCompany.OGRN_Date?.ToString("dd.MM.yyyy")}", //"ОГРНИП: 123456789012345 от 21.02.2005 г.",
                                        }
                                    }
                        }
                    }
                };
                xmlObject = xmlClass;
            }

            string fileName = fileId + ".xml";
            string xmlFile = GetXmlFilePath(doc.Id, fileName);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // https://stackoverflow.com/a/33579717
            using (XmlTextWriter writer = new(xmlFile, Encoding.GetEncoding("windows-1251")))
            //using (XmlWriter writer = XmlWriter.Create(xmlFile))
            //using (TextWriter writer = new StreamWriter(xmlFile))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;

                xmlSerializer.Serialize(writer, xmlObject);
            }

            var xmlDoc = new XmlDoc
            {
                DateAndTime = DateTime.Now,
                Name = $"Акт №{doc.Num} от {doc.Date:d} на сумму {doc.Sum:N2}",
                DocId = doc.Id,
                FileName = fileName
            };

            _db.XmlDocs.Add(xmlDoc);
            //_context.Update(doc);
            await _db.SaveChangesAsync();

            return xmlDoc;
        }

        public async Task DeleteXmlDoc(int docId, int xmlDocId)
        {
            var xmlDoc = await _db.XmlDocs.FindAsync(xmlDocId);
            if (xmlDoc == null || xmlDoc.DocId != docId)
            {
                return;
            }

            _db.Remove(xmlDoc);
            await _db.SaveChangesAsync();

            string xmlFile = GetXmlFilePath(xmlDoc.DocId, xmlDoc.FileName);
            System.IO.File.Delete(xmlFile);
        }

        public async Task<FileInfo?> GetXmlDocFileInfo(int xmlDocId)
        {
            var xmlDoc = await _db.XmlDocs.FindAsync(xmlDocId);
            if (xmlDoc == null || string.IsNullOrWhiteSpace(xmlDoc.FileName))
            {
                return null;
            }

            string xmlFilePath = GetXmlFilePath(xmlDoc.DocId, xmlDoc.FileName);

            if(!File.Exists(xmlFilePath))
                return null;

            return new FileInfo(xmlFilePath);
        }
    }
}

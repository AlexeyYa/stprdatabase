using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDB.Database
{
    class Import
    {
        public static List<Entry> LoadContents(string path)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                List<Entry> entries = new List<Entry>();
                csvParser.SetDelimiters(";");
                csvParser.ReadLine();
                string[] dateformats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "dd-MM-yyyy", "d-MM-yyyy", "yyyy-MM-dd" };

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    DateTime.TryParseExact(fields[1], dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out DateTime stDate);
                    DateTime.TryParseExact(fields[1], dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out DateTime enDate);
                    DateTime.TryParseExact(fields[1], dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out DateTime comDate);
                    Int32.TryParse(fields[19], out int sizecor);
                    Int32.TryParse(fields[9], out int sf);
                    Int32.TryParse(fields[10], out int orig);
                    Int32.TryParse(fields[11], out int cpy);
                    Int32.TryParse(fields[15], out int print);
                    Int32.TryParse(fields[16], out int numer);
                    Int32.TryParse(fields[17], out int scan);
                    Int32.TryParse(fields[18], out int thr);
                    int status = -1;
                    if (fields[14] == "завершено")
                        status = 2;
                    Entry x = new Entry()
                    {
                        Number = 1,
                        StartDate = stDate,
                        CodeType = Convert.ToInt32(fields[2]),
                        User = fields[3],
                        Group = fields[4],
                        Obj = fields[5],
                        DocCode = fields[6],
                        Subs = fields[7],
                        Tasks = fields[8],
                        SizeFormat = sf,
                        NumberOfOriginals = orig,
                        NumberOfCopies = cpy,
                        EndDate = enDate,
                        CompleteDate = comDate,
                        Status = status,
                        Print = print,
                        Numeration = numer,
                        Scan = scan,
                        Threading = thr,
                        Executor = fields[19],
                        SizeCorFormat = sizecor,
                        Corrections = fields[21]
                    };
                    entries.Add(x);

                }
                return entries;
            }
        }
    }
}

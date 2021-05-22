using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;

namespace BPL3_Backend.Services
{
    public class SheetService
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "BPL4";
        public SheetService()
        {

        }

        public List<int> getSheet() 
        {
                UserCredential credential;

                using (var stream =
                    new FileStream("JSONs\\credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define request parameters.
                String spreadsheetId = "1MHVK1B4H6OHnrsZ81O29IUtfM_FY_w3SOcPZ0vJa8Qs";
                String range = "sheet1!B1:B3";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);

                // Prints the names and majors of students in a sample spreadsheet:
                // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                List<int> points = new List<int>();
                if (values != null && values.Count > 0)
                {
                    Console.WriteLine("Name, Major");
                    foreach (var row in response.Values)
                    {
                        var v = row[0].ToString();
                        points.Add(int.Parse(v));
                    }
                }
            return points;
        }
    }
}

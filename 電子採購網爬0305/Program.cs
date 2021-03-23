using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using 電子採購網爬0305.Migrations;
using 電子採購網爬0305.model;

namespace 電子採購網爬0305
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var idHtml = ""; //查詢&primaryKey=53413351"
            Model1 db = new Model1();
            int page = 1;
            //int countsid = 0;
            bool come = false;
            bool savecome = false;
            //關鍵字
            string setting = ConfigurationSettings.AppSettings["Keyworld"];

            //寄信
            string ReceiveEmail = "a9013241@gmail.com"; //收信者email

            string CaseStr =
                @"<table width='100%' border='0' cellspacing='0' cellpadding='0' style='word-break:break-all'>
                <tbody><tr>
                	<th width='5%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>項次&nbsp;</th>
                	<th width='15%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>機關&nbsp;<br>名稱&nbsp;</th>
                	<th width='26%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>標案案號&nbsp;<br>標案名稱&nbsp;</th>
                	<th width='5%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>傳輸&nbsp;<br>次數&nbsp;</th>
                	<th width='10%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>招標&nbsp;<br>方式&nbsp;</th>
                	<th width='8%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>採購&nbsp;<br>性質&nbsp;</th>
                	<th width='9%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>公告&nbsp;<br>日期&nbsp;</th>
                	<th width='9%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>截止&nbsp;<br>投標&nbsp;</th>
                	<th width='13%' align='center' bgcolor='#E0EDF1' class='T12b' nowrap=''>預算&nbsp;<br>金額&nbsp;</th>
                </tr>";

            //寄信項次
            int counts = 1;

            for (int i = 1; i < page + 1; i++)
            {
                string htmlString =
                    GetWebContent(
                        @"https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=basic&isSpdt=&method=search&pageIndex=" +
                        i);
                HtmlDocument htmlDocument = new HtmlDocument();
                string htmlStrArranged = htmlString.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                htmlDocument.LoadHtml(htmlStrArranged);
                HtmlNode htmlNodes = htmlDocument.DocumentNode.SelectSingleNode(
                    @"html/body/table/tr[2]/td[2]/table[1]/tr[5]/td[1]/table[1]/tr[3]/td[1]/table[1]/tbody[1]/tr[1]/td[1]/div[1]/table[1]");
                int a = htmlNodes.LastChild.InnerHtml.LastIndexOf("</b>");
                int b = htmlNodes.LastChild.InnerHtml.LastIndexOf("<b>") + 3;
                page = int.Parse(
                    htmlNodes.LastChild.InnerHtml.Substring(b,
                        a - b));

                //循環
                foreach (HtmlNode node in htmlNodes.ChildNodes)
                {
                    if (node.SelectNodes("td") != null && !node.SelectNodes("td")[0].InnerHtml.Contains("第一頁") &&
                        !node.SelectNodes("td")[0].InnerHtml.Contains("無符合條件資料"))
                    {
                        if (node.SelectNodes("td")[2].InnerHtml
                            .Contains(setting) && node.SelectNodes("td")[2].InnerHtml
                            .Contains("更正公告"))
                        {
                            //如果有更正公告一率刪除
                            string idstr = "&primaryKey=";
                            idHtml = node.ChildNodes[7].InnerHtml
                                .Substring(node.ChildNodes[7].InnerHtml.IndexOf(idstr) + idstr.Length, 8);
                            var countid = db.BidCases.Where(x => x.標案案號標案名稱.Contains(idHtml)).FirstOrDefault();
                            if (countid != null)
                            {
                                db.BidCases.Remove(countid);
                                db.SaveChanges();
                            }
                        }

                        if (node.SelectNodes("td")[2].InnerHtml
                            .Contains(setting))
                        {
                            var h = db.BidCases.Where(x => x.標案案號標案名稱.Contains(setting));
                            //之前已經查詢過 必需跟資料庫比對(有資料)
                            if (h.Count() > 0)
                            {
                                come = true;
                            }
                            //無資料
                            else
                            {
                                //初始化(沒有查詢過)
                                BidCase cases = new BidCase(); //屬性
                                                               // var first = node.ChildNodes[1].InnerHtml;
                                cases.項次 = Convert.ToInt16(
                                    node.SelectNodes("td")[0].InnerHtml.Replace("&nbsp;", ""));
                                cases.機關名稱 = node.SelectNodes("td")[1].InnerHtml.Replace("&nbsp;", "");
                                //string str = node.SelectNodes("td")[2].InnerHtml.Substring(node.SelectNodes("td")[2].InnerHtml.IndexOf("<br>"));
                                //cases.標案案號 = node.SelectNodes("td")[2].InnerHtml.Replace(str, "");
                                //string str2 = node.SelectNodes("td")[3].InnerHtml.Substring(node.SelectNodes("td")[3].InnerHtml.LastIndexOf("<u>") + 3);
                                cases.標案案號標案名稱 = node.SelectNodes("td")[2].InnerHtml
                                    .Replace("..", "https://web.pcc.gov.tw/tps").Replace("&nbsp;", "");
                                //cases.傳輸次數 = str2.Replace("</u>", "").Replace("&nbsp;", "").Replace("</a>", "");
                                cases.傳輸次數 = node.SelectNodes("td")[3].InnerHtml
                                    .Replace("..", "https://web.pcc.gov.tw/tps")
                                    .Replace("&nbsp;", "");
                                cases.招標方式 = node.SelectNodes("td")[4].InnerHtml;
                                cases.採購性質 = node.SelectNodes("td")[5].InnerHtml;
                                cases.公告日期 = Convert.ToDateTime(node.SelectNodes("td")[6].InnerHtml);
                                cases.截止投標 = Convert.ToDateTime(node.SelectNodes("td")[7].InnerHtml);
                                if (node.SelectNodes("td")[8].InnerHtml != "")
                                {
                                    cases.預算金額 = Convert.ToDecimal(node.SelectNodes("td")[8].InnerHtml);
                                }

                                //db名 加入有值屬性
                                db.BidCases.Add(cases);

                                //寄信
                                CaseStr +=
                                    $@"<tr bgcolor='#FFFFFF' onmouseover='overcss(this);' onmouseout='outcss(this,'#FFFFFF');' style='background-color: rgb(255, 255, 255); color: rgb(0, 0, 0);'>
            	                	<!-- 項次 -->
            	                	<td align='center'>{counts}</td>
            	                	<!-- 機關名稱 -->
            	                	<td align='left'>{cases.機關名稱}</td>
            	                	<!-- 標案案號&標案名稱 -->
            	                	<td align='left'>{cases.標案案號標案名稱}
            	                	&nbsp;
            	                	</td>
            	                	<!-- 傳輸次數 -->
            	                	<td align='center'>{cases.傳輸次數}
            	                		&nbsp;
            	                	</td>
            	                	<!-- 招標方式 -->
            	                	<td align='left'>{cases.招標方式}</td>
            	                	<!-- 標的分類 -->
            	                	<td align='left'>{cases.採購性質}</td>
            	                	<!-- 公告日期 -->
            	                	<td align='left'>{string.Format("{0:yyy/MM/dd}", cases.公告日期)}
                                                                  </td>
            	                	<!-- 截止投標 -->
            	                	<td align='left'>{string.Format("{0:yyy/MM/dd}", cases.截止投標)}
                                                                  </td>
            	                	<!-- 預算金額 -->
            	                	<td align='right'>{Convert.ToInt64(cases.預算金額)}
            	                	</td>
            	                </tr>";
                                savecome = true;
                            }

                            //有資料
                            if (come)
                            {
                                string idstr = "&primaryKey=";
                                idHtml = node.ChildNodes[7].InnerHtml
                                    .Substring(node.ChildNodes[7].InnerHtml.IndexOf(idstr) + idstr.Length, 8);

                                var countid = db.BidCases.Any(x => x.標案案號標案名稱.Contains(idHtml));

                                //id沒有資料才新增資料
                                if (!countid)
                                {
                                    BidCase cases = new BidCase(); //屬性
                                                                   // var first = node.ChildNodes[1].InnerHtml;
                                    cases.項次 = Convert.ToInt16(
                                        node.SelectNodes("td")[0].InnerHtml.Replace("&nbsp;", ""));
                                    cases.機關名稱 = node.SelectNodes("td")[1].InnerHtml.Replace("&nbsp;", "");
                                    //string str = node.SelectNodes("td")[2].InnerHtml.Substring(node.SelectNodes("td")[2].InnerHtml.IndexOf("<br>"));
                                    //cases.標案案號 = node.SelectNodes("td")[2].InnerHtml.Replace(str, "");
                                    //string str2 = node.SelectNodes("td")[3].InnerHtml.Substring(node.SelectNodes("td")[3].InnerHtml.LastIndexOf("<u>") + 3);
                                    cases.標案案號標案名稱 = node.SelectNodes("td")[2].InnerHtml
                                        .Replace("..", "https://web.pcc.gov.tw/tps").Replace("&nbsp;", "");
                                    //cases.傳輸次數 = str2.Replace("</u>", "").Replace("&nbsp;", "").Replace("</a>", "");
                                    cases.傳輸次數 = node.SelectNodes("td")[3].InnerHtml
                                        .Replace("..", "https://web.pcc.gov.tw/tps")
                                        .Replace("&nbsp;", "");
                                    cases.招標方式 = node.SelectNodes("td")[4].InnerHtml;
                                    cases.採購性質 = node.SelectNodes("td")[5].InnerHtml;
                                    cases.公告日期 = Convert.ToDateTime(node.SelectNodes("td")[6].InnerHtml);
                                    cases.截止投標 = Convert.ToDateTime(node.SelectNodes("td")[7].InnerHtml);
                                    if (node.SelectNodes("td")[8].InnerHtml != "")
                                    {
                                        cases.預算金額 = Convert.ToDecimal(node.SelectNodes("td")[8].InnerHtml);
                                    }

                                    //db名 加入有值屬性
                                    db.BidCases.Add(cases);
                                    CaseStr +=
                                        $@"<tr bgcolor='#FFFFFF' onmouseover='overcss(this);' onmouseout='outcss(this,'#FFFFFF');' style='background-color: rgb(255, 255, 255); color: rgb(0, 0, 0);'>
            	                	<!-- 項次 -->
            	                	<td align='center'>{counts}</td>
            	                	<!-- 機關名稱 -->
            	                	<td align='left'>{cases.機關名稱}</td>
            	                	<!-- 標案案號&標案名稱 -->
            	                	<td align='left'>{cases.標案案號標案名稱}
            	                	&nbsp;
            	                	</td>
            	                	<!-- 傳輸次數 -->
            	                	<td align='center'>{cases.傳輸次數}
            	                		&nbsp;
            	                	</td>
            	                	<!-- 招標方式 -->
            	                	<td align='left'>{cases.招標方式}</td>
            	                	<!-- 標的分類 -->
            	                	<td align='left'>{cases.採購性質}</td>
            	                	<!-- 公告日期 -->
            	                	<td align='left'>{string.Format("{0:yyy/MM/dd}", cases.公告日期)}
                                                                  </td>
            	                	<!-- 截止投標 -->
            	                	<td align='left'>{string.Format("{0:yyy/MM/dd}", cases.截止投標)}
                                                                  </td>
            	                	<!-- 預算金額 -->
            	                	<td align='right'>{Convert.ToInt64(cases.預算金額)}
            	                	</td>
            	                </tr>";
                                    savecome = true;
                                }
                            }
                            //項次
                            counts++;
                        }
                    }
                }
            }
            if (savecome)
            {
                db.SaveChanges();
                SendEmail(ReceiveEmail, CaseStr + "</table>");
            }
            Console.WriteLine("結束");
            Console.ReadKey();
        }

        /// <summary>
        /// 老師的api--網頁的資料轉化html原始碼
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetWebContent(string Url)
        {
            var uri = new Uri(Url);
            var request = WebRequest.Create(Url) as HttpWebRequest;
            // If required by the server, set the credentials.
            request.UserAgent = "*/*";
            request.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            // 重點是修改這行
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2;
                                                                              // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static void SendEmail(string receiveEmail, string bodycontent)
        {
            //設定smtp主機
            string smtpAddress = "smtp.gmail.com";
            //設定Port
            int portNumber = 587;
            bool enableSSL = true;
            //填入寄送方email和密碼
            string emailFrom = "avonworktest@gmail.com";
            string password = "avon201012";
            //收信方email 可以用逗號區分多個收件人
            string emailTo = receiveEmail;
            //主旨
            string filetime = DateTime.Now.ToString("yyyy.MM.dd");
            string subject = filetime + " 標案查詢通知:";
            //內容
            string body = bodycontent;
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                // 若你的內容是HTML格式，則為True
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail); //如果有錯記得打開低安全
                }
            }
        }
    }
}
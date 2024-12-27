using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using MetalMaxSystem;

namespace SC2Crawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //获取一个包的信息（只需修改包的cookie）
            string url = "http://live.bilibili.com/213";
            //string cookie = "buvid3=0DFEE112-51F6-F856-76B1-FB921343F2E495271infoc; _uuid=D8AC11EE-1D17-81065-DCED-D7DE5553873894300infoc; buvid_fp=0DFEE112-51F6-F856-76B1-FB921343F2E495271infoc; CURRENT_FNVAL=2000; blackside_state=1; rpdid=|(k|~umkuRR)0J'uYJ))lkm~m; PVID=1; b_lsid=D4C984EF_17D947274C1; sid=aox54cdj";
            //string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.289 Safari/537.36";
            //List<string> list = MMCore.GetDanMuWithGet(url, cookie, userAgent);
            //foreach (string item in list)
            //{
            //   MMCore.Tell(item);
            //}

            //无法获取JS动态内容
            //HtmlAgilityPack.HtmlDocument doc = new();
            //doc.LoadHtml(MMCore.CreateGetHttpResponse(url));
            //HtmlNode str = doc.DocumentNode.SelectSingleNode("//div[contains(@class,\"product_main\")]/p[@class=\"price_color\"]");
            //MMCore.Tell(str.InnerText);
            //string strUal = str.Attributes["class"].Value;
            //string content = MMCore.GetWebStr(strUal);
            //MMCore.Tell(content);


            //用url替换为实际的网址
            //HtmlWeb web = new HtmlWeb();
            //web.OverrideEncoding = System.Text.Encoding.UTF8;
            //HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            //string xpath = "//div[@class='danmaku-item-right v-middle pointer ts-dot-2']";
            //HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(xpath);
            //foreach (var node in nodes)
            //{
            //   MMCore.Tell(node.InnerText);
            //}
            //HtmlNode node = doc.DocumentNode.SelectSingleNode(xpath);
            //MMCore.Tell(node.InnerText);

            //try
            //{
            //   IWebDriver webDriver = new ChromeDriver();
            //   webDriver.Url = url;
            //   var pageSource = webDriver.PageSource;
            //   var doc = new HtmlAgilityPack.HtmlDocument();
            //   doc.LoadHtml(pageSource);
            //   HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//html");
            //   foreach (var link in linkNodes)
            //   {
            //       string name = link.GetAttributeValue("class", string.Empty);
            //       MMCore.Tell(name);
            //       MMCore.Tell(link.InnerText);
            //   }

            //}
            //catch (Exception ex)
            //{
            //   MMCore.Tell(ex.Message);
            //}

            try
            {
                ChromeDriver webDriver = new ChromeDriver();
                webDriver.Url = url;
                var pageSource = webDriver.PageSource;
                MMCore.Tell("Found {0} links", GetDanMuLinksA(pageSource).Count);
            }
            catch (Exception ex)
            {
                MMCore.Tell(ex.Message);
            }


            //HtmlAgilityPack.HtmlDocument doc = new();
            //doc.LoadHtml(MMCore.CreateGetHttpResponse("https://ac.qq.com/Comic/ComicInfo/id/542330"));
            //HtmlNode img = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[3]/div[1]/div[1]/div[1]/a/img");
            //string imgUal = img.Attributes["src"].Value;
            //MMCore.Download(imgUal, "123.jpg", @"C:\Users\linsh\Desktop\Download\", true);
            //MMCore.Tell("下载完成！");

            //var bookLinks = GetBookLinks("http://books.toscrape.com/catalogue/category/books/mystery_3/index.html");
            //MMCore.Tell("Found {0} links", bookLinks.Count);
            //var books = GetBookDetails(bookLinks);

            //string url = "https://live.bilibili.com/213?broadcast_type=0&is_room_feed=1&spm_id_from=333.999.to_liveroom.0.click&live_from=86002";
            //MMCore.Tell("Found {0} links", GetDanMuLinks(url).Count);
            //var danMus = GetDanMuDetails(url);

        }

        static List<string> GetDanMuLinksA(string pageSource)
        {
            var danMuLinks = new List<string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(pageSource);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//html/body/div[3]/main/div[1]/section[1]/div[3]/div[9]/div[1]/div[1]");
            foreach (var link in linkNodes)
            {
                string name = link.GetAttributeValue("class", string.Empty);
                MMCore.Tell(name);
                MMCore.Tell(link.InnerText);
                ProcessChildNodes(link, danMuLinks);
            }
            return danMuLinks;
        }

        static List<string> GetDanMuLinksB(string pageSource)
        {
            var danMuLinks = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = GetDocument(pageSource);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//html");
            foreach (var link in linkNodes)
            {
                string name = link.GetAttributeValue("class", string.Empty);
                MMCore.Tell(name);
                MMCore.Tell(link.InnerText);
                ProcessChildNodes(link, danMuLinks);
            }
            return danMuLinks;
        }

        static List<string> GetDanMuLinks(string url)
        {
            var danMuLinks = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = GetDocument(url);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//html");
            foreach (var link in linkNodes)
            {
                string name = link.GetAttributeValue("class", string.Empty);
                MMCore.Tell(name);
                MMCore.Tell(link.InnerText);
                ProcessChildNodes(link, danMuLinks);
            }
            return danMuLinks;
        }

        static void ProcessChildNodes(HtmlNode node, List<string> danMuLinks)
        {
            foreach (var childNode in node.ChildNodes)
            {
                string childName = childNode.Name;
                MMCore.Tell("childName => " + childName);
                MMCore.Tell(childNode.InnerText);

                //如果需要收集特定类型的节点，可以在这里添加逻辑
                //例如，如果childNode是一个<a>标签，并且其属性包含特定的值，您可以将其添加到danMuLinks列表中

                //继续递归处理子节点的子节点
                if (childNode.HasChildNodes)
                {
                    ProcessChildNodes(childNode, danMuLinks);
                }
            }
        }

        static List<DanMu> GetDanMuDetails(string url)
        {
            var danMus = new List<DanMu>();

            HtmlAgilityPack.HtmlDocument document = GetDocument(url);
            var nameXPath = "//div[contains(@class, \"common-nickname-wrapper\")]/span[@class=\"user-name v-middle pointer open-menu\"]";
            var contentXPath = "//div[contains(@class,\"chat-item danmaku-item\")]/span[@class=\"danmaku-item-right v-middle pointer ts-dot-2 open-menu\"]";
            var danMu = new DanMu();
            danMu.Name = document.DocumentNode.SelectSingleNode(nameXPath).InnerText;
            danMu.Content = document.DocumentNode.SelectSingleNode(contentXPath).InnerText;
            MMCore.Tell(danMu.Name);
            MMCore.Tell(danMu.Content);
            danMus.Add(danMu);

            return danMus;
        }

        static List<string> GetBookLinks(string url)
        {
            var bookLinks = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = GetDocument(url);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//h3/a");
            var baseUri = new Uri(url);
            foreach (var link in linkNodes)
            {
                string href = link.Attributes["href"].Value;
                bookLinks.Add(new Uri(baseUri, href).AbsoluteUri);
            }
            return bookLinks;
        }

        static List<Book> GetBookDetails(List<string> urls)
        {
            var books = new List<Book>();
            foreach (var url in urls)
            {
                HtmlAgilityPack.HtmlDocument document = GetDocument(url);
                var titleXPath = "//h1";
                var priceXPath = "//div[contains(@class,\"product_main\")]/p[@class=\"price_color\"]";
                var book = new Book();
                book.Title = document.DocumentNode.SelectSingleNode(titleXPath).InnerText;
                book.Price = document.DocumentNode.SelectSingleNode(priceXPath).InnerText;
                MMCore.Tell(book.Title);
                MMCore.Tell(book.Price);
                books.Add(book);
            }
            return books;
        }

        //Parses the URL and returns HtmlDocument object                         
        static HtmlAgilityPack.HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            return doc;
        }
    }

    public class DanMu
    {
        public Dictionary<string, string> DanMuDictionary { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Price { get; set; }
    }
}


using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using CsvHelper;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;
using System;
using NUnit.Framework.Internal;

namespace WhatCanIReadToday
{
    public class McKinneyLibrary
    {
        protected IWebDriver driver;
        protected Library library;
        
        [SetUp]
        public void Setup()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver();

            // Set Library Info
            library = JsonConvert.DeserializeObject<Library>(File.ReadAllText("C:\\PersonalProjects\\WhatCanIReadToday\\Libraries\\gay.json"));
        }

        [TearDown]
        public void QuitDriver()
        {
            driver.Quit();
        }

        [TestCaseSource(typeof(BookData), nameof(BookData.GetTestData))]
        public void McKinneyLibrary_Book(string bookTitle)
        {
            // Set Test Data
            var libraryURL = library.SearchURL;
            var libraryName = library.Name;
            bool isAvail = false;

            // Format Book Title for matching
            bookTitle = Regex.Replace(bookTitle, "[^A-z^\\s^'^\\-^0-9^,^&^#].*", "").TrimEnd();

            // Go to McKinney Library Search Page
            driver.Navigate().GoToUrl(libraryURL);

            // Identify Search Textbox
            var searchBox = driver.FindElement(By.Name("lookfor"));
            searchBox.Click();

            // Enter search term and search
            searchBox.SendKeys(bookTitle);
            searchBox.SendKeys(Keys.Enter);

            // Identify filter section
            var availableNow = driver.FindElement(By.XPath("//*[@id=\"facet-accordion\"]/div[3]/div[1]"));
            availableNow.Click();

            // Toggle Available @ Library Filter
            var availAtMyLibrary = driver.FindElement(By.PartialLinkText(libraryName));
            availAtMyLibrary.Click();

            // Get first record in results
            var firstRecord = driver.FindElement(By.ClassName("record1"));

            // Confirm #1 has correct title
            var recordTitle = firstRecord.FindElement(By.ClassName("result-title")).Text;

            // TODO: This currently "passes" on The Nightingale, but it's another book by another author
            // TODO: Need to include author in comparison
            Assert.That(recordTitle.ToUpper(), Does.StartWith(bookTitle.ToUpper()), "Not in library");

            // Get availability information
            var availabilityGrid = firstRecord.FindElement(By.ClassName("related-manifestations"));
            var availabilities = availabilityGrid.FindElements(By.ClassName("grouped"));

            // Loop through availabilities
            // Find "Book"
            foreach (var availability in availabilities)
            {
                // Look at results for Book
                if (availability.FindElement(By.LinkText("Book")).Displayed)
                {
                    // Confirm status is "On Shelf"
                    var onShelfEmblem = availability.FindElement(By.ClassName("status-on-shelf")).Text;

                    if (onShelfEmblem == "On Shelf")
                    {
                        isAvail = true;
                        break;
                    }
                }
            }

            // Confirm title is available as book
            Assert.IsTrue(isAvail);
        }

        [TestCaseSource(typeof(BookData), nameof(BookData.GetTestData))]
        [Ignore("Not ready yet")]
        public void McKinneyLibrary_eBook(string bookTitle)
        {
            // Set Test Data
            var libraryURL = library.SearchURL;
            var libraryName = library.Name;
            bool isAvail = false;

            // Format Book Title for matching
            bookTitle = Regex.Replace(bookTitle, "[^A-z^\\s^'^\\-^0-9^,^&^#].*", "").TrimEnd();

            // Go to McKinney Library Search Page
            driver.Navigate().GoToUrl(libraryURL);

            // Identify Search Textbox
            var searchBox = driver.FindElement(By.Name("lookfor"));
            searchBox.Click();

            // Enter search term and search
            searchBox.SendKeys(bookTitle);
            searchBox.SendKeys(Keys.Enter);

            // Identify filter section
            var availableNow = driver.FindElement(By.XPath("//*[@id=\"facet-accordion\"]/div[3]/div[1]"));
            availableNow.Click();

            // Toggle Available @ Library Filter
            var availAtMyLibrary = driver.FindElement(By.PartialLinkText(libraryName));
            availAtMyLibrary.Click();

            // Get first record in results
            var firstRecord = driver.FindElement(By.ClassName("record1"));

            // Confirm #1 has correct title
            var recordTitle = firstRecord.FindElement(By.ClassName("result-title")).Text;

            //var author = firstRecord.FindElement(By.XPath("//*[@id=\"groupedRecordcc9fe276-0dff-d73d-3d55-ff47babfd7bd-eng\"]/div/div[2]/div[3]/a"));
            //Console.WriteLine(author);

            // TODO: This currently "passes" on The Nightingale, but it's another book by another author
            // TODO: Need to include author in comparison
            Assert.That(recordTitle.ToUpper(), Does.StartWith(bookTitle.ToUpper()), "Not in library");

            // Get availability information
            var availabilityGrid = firstRecord.FindElement(By.ClassName("related-manifestations"));
            var availabilities = availabilityGrid.FindElements(By.ClassName("grouped"));

            // Loop through availabilities
            // Find "Book"
            foreach (var availability in availabilities)
            {
                // Look at results for Book
                if (availability.FindElement(By.LinkText("eBook")).Displayed)
                {
                    // Confirm status is "On Shelf"
                    var onShelfEmblem = availability.FindElement(By.ClassName("status-on-shelf")).Text;

                    if (onShelfEmblem == "On Shelf")
                    {
                        isAvail = true;
                        log.Info(bookTitle + " is available as an eBook");
                        break;
                    }
                }
            }

            // Confirm title is available as book
            Assert.IsTrue(isAvail);
        }
    }

    public static class BookData
    {
        public static IEnumerable<string> GetTestData()
        {
            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Replace("-", "").Replace(" ", "")
            };

            using (var reader = new StreamReader("C:\\PersonalProjects\\WhatCanIReadToday\\Books.csv"))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    List<Book> books = csv.GetRecords<Book>().ToList();
                    List<string> bookTitles = books.Where(x => x.Bookshelves.Contains("to-read")).Select(b => b.Title).ToList();
                    return bookTitles;
                }
            }
        }
    }
}
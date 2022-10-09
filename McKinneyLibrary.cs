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
            library = JsonConvert.DeserializeObject<Library>(File.ReadAllText("C:\\PersonalProjects\\WhatCanIReadToday\\Libraries\\johnandjudygaylibrary.json"));
        }

        [TearDown]
        public void QuitDriver()
        {
            driver.Quit();
        }

        [TestCaseSource(typeof(BookData), nameof(BookData.GetTestData))]
        public void McKinneyLibrary_Book(Book book)
        {
            // Set Test Data
            bool isAvail;
            Format format = Format.Book;

            // Get availability information
            var availabilityGrid = GetTopResult(library, book);

            // Check availability
            isAvail = IsAvailableInFormat(availabilityGrid, format);

            Assert.IsTrue(isAvail);
        }

        [TestCaseSource(typeof(BookData), nameof(BookData.GetTestData))]
        public void McKinneyLibrary_eBook(Book book)
        {
            // Set Test Data
            bool isAvail;
            Format format = Format.eBook;

            // Get availability information
            var availabilityGrid = GetTopResult(library, book);

            // Check availability
            isAvail = IsAvailableInFormat(availabilityGrid, format);
            
            // Confirm title is available as book
            Assert.IsTrue(isAvail);
        }

        private IWebElement GetTopResult(Library library, Book book)
        {
            // Sanitize book title search term
            string bookTitle = SanitizeSearch(book.Title);
            
            // Go to McKinney Library Search Page
            driver.Navigate().GoToUrl(library.SearchURL);

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
            var availAtMyLibrary = driver.FindElement(By.PartialLinkText(library.Name));
            availAtMyLibrary.Click();

            // Find all results
            var results = driver.FindElements(By.ClassName("result"));

            // Loop through looking for a match on Title and Author
            foreach (var result in results)
            {
                // Check Title
                string resultTitle = result.FindElement(By.ClassName("result-title")).Text;
                bool titleIsMatch = resultTitle.ToUpper().StartsWith(bookTitle.ToUpper());

                // Check Author
                bool isCorrectAuthor = result.FindElement(By.LinkText(book.Authorlf)).Displayed;

                if (titleIsMatch && isCorrectAuthor)
                {
                    // Return this record
                    return result.FindElement(By.ClassName("related-manifestations"));
                }
            }

            return null;
        }
        
        private string SanitizeSearch(string searchTerm)
        {
            return Regex.Replace(searchTerm, "[^A-z^\\s^'^\\-^0-9^,^&^#].*", "").TrimEnd();
        }

        private bool IsAvailableInFormat(IWebElement availabilityGrid, Format format)
        {
            // Find badge for format
            var availability = availabilityGrid.FindElement(By.LinkText(format.ToString()));

            if (availability.Text == format.ToString())
            {
                return true;
            }

            return false;
        }
    }

    public static class BookData
    {
        public static IEnumerable<Book> GetTestData()
        {
            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Replace("-", "").Replace(" ", "")
            };

            using (var reader = new StreamReader("C:\\PersonalProjects\\WhatCanIReadToday\\Books_LocalTest.csv"))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    List<Book> books = csv.GetRecords<Book>().ToList();
                    return books;
                }
            }
        }
    }
}
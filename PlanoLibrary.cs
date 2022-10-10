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
    public class PlanoLibrary
    {
        protected IWebDriver driver;
        protected Library library;

        [SetUp]
        public void Setup()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver();

            // Set Library Info
            library = JsonConvert.DeserializeObject<Library>(File.ReadAllText("C:\\PersonalProjects\\WhatCanIReadToday\\Libraries\\davislibrary.json"));
        }

        [TearDown]
        public void QuitDriver()
        {
            driver.Quit();
        }

        [TestCaseSource(typeof(BookData), nameof(BookData.GetTestData))]
        public void PlanoLibrary_Book(Book book)
        {
            // Set Test Data
            bool isAvail;
            Format format = Format.Book;

            // Get availability information
            var availabilityGrid = GetTopResult(library, book, format);

            // Check availability
            isAvail = IsAvailableInFormat(availabilityGrid, format);

            Assert.IsTrue(isAvail);
        }

        [TestCaseSource(typeof(BookData), nameof(BookData.GetTestData))]
        public void PlanoLibrary_eBook(Book book)
        {
            // Set Test Data
            bool isAvail;
            Format format = Format.eBook;

            // Get availability information
            var availabilityGrid = GetTopResult(library, book, format);

            // Check availability
            isAvail = IsAvailableInFormat(availabilityGrid, format);

            // Confirm title is available as book
            Assert.IsTrue(isAvail);
        }

        private IWebElement GetTopResult(Library library, Book book, Format format)
        {
            // Sanitize book title search term
            string bookTitle = SanitizeSearch(book.Title);

            // Go to Plano Library Search Page
            driver.Navigate().GoToUrl(library.SearchURL);

            // Identify Search Textbox
            var searchBox = driver.FindElement(By.XPath("//*[@id=\"q\"]"));
            searchBox.Click();

            // Enter search term and search
            searchBox.SendKeys(bookTitle);
            searchBox.SendKeys(Keys.Enter);

            // Toggle Available @ Library Filter
            var libraryFilter = driver.FindElement(By.PartialLinkText(library.Name));
            libraryFilter.Click();

            // Toggle "Only Show Available" filter
            var filterPane = driver.FindElement(By.ClassName("searchLimitsColumn"));

            var onlyShowFilter = filterPane.FindElement(By.Name("submit_0"));
            onlyShowFilter.Click();

            // Need to bind filterPane again (the 'Books' filter fails if we don't have this line)
            filterPane = driver.FindElement(By.ClassName("searchLimitsColumn"));

            // Toggle Format filter
            var formatFilter = filterPane.FindElement(By.PartialLinkText("Books"));
            formatFilter.Click();



            // End of working code

            

            // BEGIN Experiment Code



            // Find all results
            var resultList = driver.FindElement(By.Id("results_wrapper"));
            var results = resultList.FindElements(By.ClassName("cell_wrapper"));

            // Loop through looking for a match on Title and Author
            foreach (var result in results)
            {
                // Check Title
                var resultTitleDiv = result.FindElement(By.ClassName("INITIAL_TITLE_SRCH"));
                var resultTitle = resultTitleDiv.FindElement(By.ClassName("hideIE")).Text;
                bool titleIsMatch = resultTitle.ToUpper().StartsWith(bookTitle.ToUpper());

                // Check Author
                // Same class exists on 3 author divs; need to get all and iterate through

                // TODO: Pick up here later



                //// Check Author
                //bool isCorrectAuthor = result.FindElement(By.LinkText(book.Authorlf)).Displayed;

                //if (titleIsMatch && isCorrectAuthor)
                //{
                //    // Return this record
                //    return result.FindElement(By.ClassName("related-manifestations"));
                //}
            }


            // END Experiment Code









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
}
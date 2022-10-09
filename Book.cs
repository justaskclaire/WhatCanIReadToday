using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatCanIReadToday
{
    public class Book
    {
        public long BookId { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            // Make things look pretty in Test Explorer
            return Title.ToString() + " | " + Author.ToString();
        }

        public string Author { get; set; }
        public string Authorlf { get; set; }
        public string AdditionalAuthors { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public decimal? MyRating { get; set; }
        public decimal? AverageRating { get; set; }
        public string Publisher { get; set; }
        public string Binding { get; set; }
        public int? NumberofPages { get; set; }
        public int? YearPublished { get; set; }
        public int? OriginalPublicationYear { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Bookshelves { get; set; }
        public string Bookshelveswithpositions { get; set; }
        public string ExclusiveShelf { get; set; }
        public string MyReview { get; set; }
    }
}

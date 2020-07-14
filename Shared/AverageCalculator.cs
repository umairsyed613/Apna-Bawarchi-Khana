using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApnaBawarchiKhana.Shared
{
    public class AverageCalculator
    {
        public static double GetAverageRating(List<int> Ratings)
        {
            if (Ratings == null || !Ratings.Any())
            {
                return 1;
            }

            int star5 = Ratings.Where(s => s == 5).Count();
            int star4 = Ratings.Where(s => s == 4).Count();
            int star3 = Ratings.Where(s => s == 3).Count();
            int star2 = Ratings.Where(s => s == 2).Count();
            int star1 = Ratings.Where(s => s == 1).Count();

            double rating = (double)(5 * star5 + 4 * star4 + 3 * star3 + 2 * star2 + 1 * star1) / (star1 + star2 + star3 + star4 + star5);

            rating = Math.Round(rating, 1);

            return rating;
        }
    }
}

namespace UrbanSystem.Common
{
    public static class ValidationConstants
    {
        public static class Suggestion
        {
            public const int TitleMaxLength = 50;
            public const int TitleMinLength = 5;
            public const int CategoryMaxLength = 20;
            public const int CategoryMinLength = 2;
            public const int DescriptionMaxLength = 200;
            public const int DescriptionMinLength = 10;
        }

        public static class Location
        {
            public const int CityNameMaxLength = 30;
            public const int StreetNameMaxLength = 40;
        }

        public static class Comment
        {
            public const int ContentMaxLength = 1000;
        }

        public static class Meeting
        {
            public const int TitleMaxLength = 200;
            public const int DescriptionMaxLength = 1000;
            public const int DurationMaxValue = 8;
        }

        public static class Project
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 500;
            public const int ImageUrlMaxLength = 2048;
            public const int FundsPrecision = 18;
            public const int FundsScale = 2;
            public const string DefaultCreationDateSql = "GETUTCDATE()";
            public const bool DefaultIsCompleted = false;
        }
    }
}
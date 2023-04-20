using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.Models
{
    public class User
    {
        public User(string SqlRes)
        {
            int findIndex = SqlRes.IndexOf("user_id");
            string id = "";
            for(int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                id += SqlRes[i];
            }

            findIndex = SqlRes.IndexOf("nickname");
            string nickname = "";
            for (int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                nickname += SqlRes[i];
            }

            findIndex = SqlRes.IndexOf("surname");
            string surname = "";
            for (int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                surname += SqlRes[i];
            }

            findIndex = SqlRes.IndexOf("name");
            string name = "";
            for (int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                name += SqlRes[i];
            }

            findIndex = SqlRes.IndexOf("patronymic");
            string patronymic = "";
            for (int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                patronymic += SqlRes[i];
            }

            findIndex = SqlRes.IndexOf("country");
            string country = "";
            for (int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                country += SqlRes[i];
            }

            findIndex = SqlRes.IndexOf("birth");
            string birth = "";
            for (int i = findIndex; i < SqlRes.Length; i++)
            {
                if (SqlRes[i] == ';') break;
                birth += SqlRes[i];
            }

            Id = Convert.ToInt64(id);
            Nickname = nickname.Split(':')[1];
            Surname = surname.Split(':')[1];
            Name = name.Split(':')[1];
            Patronymic = patronymic.Split(':')[1];
            BirthDate = Convert.ToDateTime(birth.Split(':')[1]);
            Country = country.Split(':')[1];
        }
        public long Id { get; set; }
        public string SHA512Password { get; set; }
        public string Nickname { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime RegistrationDate { get; set; }

    }
}

using System;
using System.Collections.Generic;

namespace VMindbooke.RainbowKarma
{
    public static class Keki
    {
        private const string Prefix = "[Rainbow] ";
        private static readonly Random Random = new Random();

        private static string Format(string context, IReadOnlyList<string> templates)
        {
            string template = Prefix + templates[Random.Next(templates.Count)];
            string[] words = context.Split();
            string word = words[Random.Next(words.Length)];
            return string.Format(template, context, word);
        }
        
        public static string GetCleverComment(string context) =>
            Format(context, new string[]
            {
                "в таких моментах не стоит ничего говорить а только бросить загадочный взгляд в мекиканской шляпи",
                "Два чаю, аффтар жжот",
                "Аффтар, выпей йаду",
                "И что такое {1} по-твоему?",
                "Аналитик диванный, ты даже не знаешь, что такое {1}"
            });
        
        public static string GetCleverReply(string context) =>
            Format(context, new string[]
            {
                "А может, ты {1}?",
                "Такой чуши про {1} мир еще не слыхивал",
                "Зачем мне твои {1}? Как будто кроме {1} в жизни больше ничего нет",
                ">{0}\nТы правда так считаешь? Ты вообще шаришь за {1}?",
                "Нет, ты"
            });

        public static string GetCleverTitle(string context) =>
            Format(context, new string[]
            {
                "Вы только посмотрите: {0}!",
                "Кто бы мог подумать: {0}!",
                "Вся правда о {1}: {0}",
                "Как говорил {1}: \"{0}\"",
                "Я {1}л, {1}у и буду {1}ть"
            });
    }
}

using System;
using Bogus.DataSets;

namespace Usage
{
    class WordsGenerator : IWordsGenerator
    {

        private readonly Lorem _generator;
            
        public WordsGenerator(Lorem generator)
        {
            _generator = generator;
        }

        public static WordsGenerator Create(string local)
        {
            var generator = new Lorem(local);
            return new WordsGenerator(generator);
        }
        
        public string GetTitle()
        {
            var title = _generator.Sentence(new Random().Next() % 2 + 10);
            return title;
            throw new NotImplementedException();
        }

        public string GetText()
        {
            var title = _generator.Sentence(new Random().Next() % 5 + 30);
            return title;
        }
    }
}